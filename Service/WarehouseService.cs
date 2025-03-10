using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Controller;
using Warehouse.Db;
using Warehouse.Model;

namespace Warehouse.Service;

public class WarehouseService(WarehouseDbContext dbContext, RobotController robotController)
{
    private readonly WarehouseDbContext _dbContext = dbContext;
    private readonly RobotController _robotController = robotController;

    // Phương thức Pallet
    public async Task<List<PalletViewModel>> GetPalletsAsync()
    {
        return await _dbContext.Pallets
            .Select(p => new PalletViewModel
            {
                Pallet_ID = p.Pallet_ID,
                Status = p.Status,
                Current_Location = p.Pallet_Locations
                    .Where(pl => pl.Time_Out == null)
                    .Select(pl => pl.Location!.Name)
                    .FirstOrDefault(),
                Robot_ID = p.Robot_ID // Corrected to string
            })
            .ToListAsync();
    }

    public async Task AddPalletAsync(Pallet pallet, string initialLocationId, Robot robotId)
    {
        if (string.IsNullOrEmpty(initialLocationId))
            throw new ArgumentException("Vị trí ban đầu không được để trống.");

        pallet.Robot_ID = robotId.Robot_ID; // Gán khóa ngoại
        pallet.Robot = robotId; // Gán navigation property (tùy chọn, EF Core sẽ tự động xử lý nếu chỉ gán Robot_ID)
        _dbContext.Pallets.Add(pallet);

        var initialLocation = new Pallet_Location
        {
            Pallet_ID = pallet.Pallet_ID,
            Location_ID = initialLocationId,
            Time_In = DateTime.Now
        };
        _dbContext.Pallet_Locations.Add(initialLocation);

        await _dbContext.SaveChangesAsync();
    }

    public async Task MovePalletAsync(string palletId, string newLocationId)
    {
        var pallet = await _dbContext.Pallets
            .Include(p => p.Robot)
            .FirstOrDefaultAsync(p => p.Pallet_ID == palletId);

        if (pallet == null || string.IsNullOrEmpty(pallet.Robot_ID))
            throw new Exception("Không tìm thấy pallet hoặc robot được gán.");

        var robot = pallet.Robot;
        var currentLocationId = await GetCurrentLocationAsync(palletId);
        var message = BuildVDA5050MoveMessage(pallet.Robot!.Robot_ID!, palletId, currentLocationId, newLocationId);
        await SendVDA5050MessageAsync(pallet.Robot, message);

        var currentRecord = await _dbContext.Pallet_Locations
            .FirstAsync(pl => pl.Pallet_ID == palletId && pl.Time_Out == null);
        currentRecord.Time_Out = DateTime.Now;

        var newRecord = new Pallet_Location
        {
            Pallet_ID = palletId,
            Location_ID = newLocationId,
            Time_In = DateTime.Now
        };
        _dbContext.Pallet_Locations.Add(newRecord);

        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePalletAsync(Pallet pallet)
    {
        var existingPallet = await _dbContext.Pallets.FindAsync(pallet.Pallet_ID) ?? throw new Exception("Không tìm thấy pallet để cập nhật.");
        existingPallet.Status = pallet.Status;
        existingPallet.Type = pallet.Type;
        existingPallet.Size = pallet.Size;
        existingPallet.Creation_Date = pallet.Creation_Date;
        existingPallet.Robot_ID = pallet.Robot_ID;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePalletAsync(string palletId)
    {
        var pallet = await _dbContext.Pallets
            .Include(p => p.Pallet_Locations)
            .FirstOrDefaultAsync(p => p.Pallet_ID == palletId) ?? throw new Exception("Không tìm thấy pallet để xóa.");
        _dbContext.Pallet_Locations.RemoveRange(pallet.Pallet_Locations);
        _dbContext.Pallets.Remove(pallet);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pallet> GetPalletByIdAsync(string palletId)
    {
        return await _dbContext.Pallets.FindAsync(palletId)
            ?? throw new Exception("Không tìm thấy pallet.");
    }

    private async Task<string> GetCurrentLocationAsync(string palletId)
    {
        var currentLocation = await _dbContext.Pallet_Locations
            .Where(pl => pl.Pallet_ID == palletId && pl.Time_Out == null)
            .Select(pl => pl.Location_ID)
            .FirstOrDefaultAsync();

        return currentLocation ?? throw new Exception("Không tìm thấy pallet.");
    }

    // Phương thức Location
    public async Task<List<Location>> GetLocationsAsync(int page = 1, int pageSize = 10)
    {
        return await _dbContext.Locations
            .Select(l => new Location
            {
                Location_ID = l.Location_ID,
                Name = l.Name,
                Parent_Location_ID = l.Parent_Location_ID,
                Parent_Location = l.Parent_Location != null ? new Location { Name = l.Parent_Location.Name } : null
            })
            .OrderBy(l => l.Location_ID) // Add ordering for consistency
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // New method to get total count
    public async Task<int> GetLocationCountAsync()
    {
        return await _dbContext.Locations.CountAsync();
    }
    public async Task<List<Location>> GetLocationsAsync()
    {
        return await _dbContext.Locations.ToListAsync();
    }

    public async Task AddLocationAsync(Location location)
    {
        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLocationAsync(Location location)
    {
        var existingLocation = await _dbContext.Locations.FindAsync(location.Location_ID) ?? throw new Exception("Không tìm thấy vị trí để cập nhật.");
        existingLocation.Name = location.Name;
        existingLocation.Parent_Location_ID = location.Parent_Location_ID;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLocationAsync(string locationId)
    {
        var location = await _dbContext.Locations
            .Include(l => l.Pallet_Locations)
            .Include(l => l.Child_Locations)
            .FirstOrDefaultAsync(l => l.Location_ID == locationId) ?? throw new Exception("Không tìm thấy vị trí để xóa.");
        if (location.Pallet_Locations.Count != 0 || location.Child_Locations.Count != 0)
            throw new Exception("Không thể xóa vị trí đang được sử dụng.");

        _dbContext.Locations.Remove(location);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Location> GetLocationByIdAsync(string locationId)
    {
        return await _dbContext.Locations.FindAsync(locationId)
            ?? throw new Exception("Không tìm thấy vị trí.");
    }

    // Phương thức Pallet_Location
    public async Task<List<Pallet_Location>> GetPalletLocationsAsync()
    {
        return await _dbContext.Pallet_Locations
            .Include(pl => pl.Pallet)
            .Include(pl => pl.Location)
            .ToListAsync();
    }

    public async Task AddPalletLocationAsync(Pallet_Location palletLocation)
    {
        _dbContext.Pallet_Locations.Add(palletLocation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePalletLocationAsync(Pallet_Location palletLocation)
    {
        var existing = await _dbContext.Pallet_Locations
            .FirstOrDefaultAsync(pl => pl.Pallet_ID == palletLocation.Pallet_ID && pl.Time_In == palletLocation.Time_In) ?? throw new Exception("Không tìm thấy bản ghi Pallet_Location để cập nhật.");
        existing.Location_ID = palletLocation.Location_ID;
        existing.Time_Out = palletLocation.Time_Out;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePalletLocationAsync(string palletId, DateTime timeIn)
    {
        var palletLocation = await _dbContext.Pallet_Locations
            .FirstOrDefaultAsync(pl => pl.Pallet_ID == palletId && pl.Time_In == timeIn) ?? throw new Exception("Không tìm thấy bản ghi Pallet_Location để xóa.");
        _dbContext.Pallet_Locations.Remove(palletLocation);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Pallet_Location> GetPalletLocationByIdAsync(string palletId, DateTime timeIn)
    {
        return await _dbContext.Pallet_Locations
            .FirstOrDefaultAsync(pl => pl.Pallet_ID == palletId && pl.Time_In == timeIn)
            ?? throw new Exception("Không tìm thấy bản ghi Pallet_Location.");
    }

    // Phương thức Robot
    public async Task<List<Robot>> GetRobotsAsync()
    {
        return await _dbContext.Robots.ToListAsync();
    }

    public async Task AddRobotAsync(Robot robot)
    {
        _dbContext.Robots.Add(robot);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteRobotAsync(string robotId)
    {
        var robot = await _dbContext.Robots.FindAsync(robotId);
        if (robot != null)
        {
            _dbContext.Robots.Remove(robot);
            await _dbContext.SaveChangesAsync();
        }
    }

    // Phương thức VDA5050
    public async Task SendVDA5050MessageAsync(Robot robotId, string message)
    {
        await _robotController.SendMessageAsync(robotId, message);
    }

    private static string BuildVDA5050MoveMessage(string robotId, string palletId, string pickupLocationId, string dropoffLocationId)
    {
        var message = new
        {
            header = new
            {
                timestamp = DateTime.UtcNow.ToString("o"),
                version = "2.1.0",
                manufacturer = "Example",
                serialNumber = robotId
            },
            order = new
            {
                orderId = Guid.NewGuid().ToString(),
                orderUpdateId = 0,
                nodes = new[]
                {
                    new
                    {
                        nodeId = pickupLocationId,
                        sequenceId = 0,
                        actions = new[]
                        {
                            new
                            {
                                actionType = "pick",
                                actionId = "pick_" + palletId,
                                actionParameters = new[]
                                {
                                    new { key = "palletId", value = palletId }
                                }
                            }
                        }
                    },
                    new
                    {
                        nodeId = dropoffLocationId,
                        sequenceId = 1,
                        actions = new[]
                        {
                            new
                            {
                                actionType = "drop",
                                actionId = "drop_" + palletId,
                                actionParameters = new[]
                                {
                                    new { key = "palletId", value = palletId }
                                }
                            }
                        }
                    }
                },
                edges = new[]
                {
                    new
                    {
                        edgeId = "edge1",
                        sequenceId = 0,
                        startNodeId = pickupLocationId,
                        endNodeId = dropoffLocationId,
                        actions = Array.Empty<object>()
                    }
                }
            }
        };
        return System.Text.Json.JsonSerializer.Serialize(message);
    }
}

public class PalletViewModel
{
    public string? Pallet_ID { get; set; }
    public string? Status { get; set; }
    public string? Current_Location { get; set; }
    public string? Robot_ID { get; set; } // Chỉ lưu ID thay vì toàn bộ Robot
}