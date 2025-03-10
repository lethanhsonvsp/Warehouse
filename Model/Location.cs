namespace Warehouse.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Location
{
    [Key]
    [StringLength(50)] // Tăng độ dài lên 50 để chứa "Z1/R01/L01/B01" (dài tối đa khoảng 20-30 ký tự)
    public string? Location_ID { get; set; }

    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(50)] // Tăng độ dài tương ứng với Location_ID
    public string? Parent_Location_ID { get; set; }

    // Navigation properties
    [ForeignKey("Parent_Location_ID")]
    public Location? Parent_Location { get; set; }

    public ICollection<Location> Child_Locations { get; set; } = [];
    public ICollection<Pallet_Location> Pallet_Locations { get; set; } = [];
}