using System.ComponentModel.DataAnnotations;
using Warehouse.Model;

public class Pallet
{
    [Key]
    [StringLength(10)]
    public string Pallet_ID { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

    [StringLength(50)]
    public string? Size { get; set; }

    public DateTime? Creation_Date { get; set; }

    public string Robot_ID { get; set; } // Khóa ngoại (foreign key)
    public Robot Robot { get; set; } // Navigation property

    public ICollection<Pallet_Location> Pallet_Locations { get; set; } = new List<Pallet_Location>();
}