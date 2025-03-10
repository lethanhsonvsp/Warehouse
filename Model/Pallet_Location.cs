namespace Warehouse.Model;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Pallet_Location
{
    [Key, Column(Order = 0)]
    [StringLength(20)]
    public string? Pallet_ID { get; set; }

    [Key, Column(Order = 1)]
    public DateTime Time_In { get; set; }

    [StringLength(20)]
    public string? Location_ID { get; set; }

    public DateTime? Time_Out { get; set; }

    // Navigation properties
    [ForeignKey("Pallet_ID")]
    public Pallet? Pallet { get; set; }

    [ForeignKey("Location_ID")]
    public Location? Location { get; set; }
}