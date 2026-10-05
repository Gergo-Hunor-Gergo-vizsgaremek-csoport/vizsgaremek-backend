using System.ComponentModel.DataAnnotations;

namespace VizsgaremekBackend.Models;

/// <summary>
/// A rendeles a user has created. A kolcsonzes is created when the rendeles is accepted.
/// </summary>
public class Rendeles
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public Guid TypeId { get; set; }
    
    public User User { get; set; }
    public Type Type { get; set; }
}