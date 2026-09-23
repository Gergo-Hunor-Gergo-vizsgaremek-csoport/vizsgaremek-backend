using System.ComponentModel.DataAnnotations;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

/// <summary>
/// "típus"
/// Stores a single type of items
/// </summary>
public class TypeWriteDto
{
    [MaxLength(50)]
    [Required]
    public string Name { get; set; }
    
    [MaxLength(500)]
    [Required]
    public string Description { get; set; }
    
    /// <summary>
    /// Link to the icon of the type
    /// </summary>
    [Required]
    public string Icon { get; set; }
    
}