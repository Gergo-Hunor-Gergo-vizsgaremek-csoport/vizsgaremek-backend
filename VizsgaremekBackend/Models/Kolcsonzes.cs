using System.ComponentModel.DataAnnotations;

namespace VizsgaremekBackend.Models;

public class Kolcsonzes
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid PeldanyId { get; set; }
    
    /// <summary>
    /// Is the item still rented
    /// </summary>
    [Required]
    public bool IsActive { get; set; }
    
    /// <summary>
    /// Time of kolcsonzes creation
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Time when the kolcsonzes ends
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
}