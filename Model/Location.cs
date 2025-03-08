namespace Warehouse.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Location
{
    [Key]
    [StringLength(10)]
    public string Location_ID { get; set; }

    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(10)]
    public string? Parent_Location_ID { get; set; }

    // Navigation properties
    [ForeignKey("Parent_Location_ID")]
    public Location? Parent_Location { get; set; }

    public ICollection<Location> Child_Locations { get; set; } = [];
    public ICollection<Pallet_Location> Pallet_Locations { get; set; } = [];
}