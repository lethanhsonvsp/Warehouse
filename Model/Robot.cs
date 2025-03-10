namespace Warehouse.Model;

public class Robot
{
    public string? Robot_ID { get; set; }
    public ICollection<Pallet> Pallets { get; set; } = [];
}
