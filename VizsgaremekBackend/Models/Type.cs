using System.ComponentModel.DataAnnotations;

namespace VizsgaremekBackend.Models;

/// <summary>
/// "típus"
/// Stores a single type of items
/// </summary>
public class Type
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    /// <summary>
    /// Link to the icon of the type
    /// </summary>
    [Required]
    public string Icon { get; set; }
    
    
    public ICollection<Peldany> Peldanys { get; set; }
}