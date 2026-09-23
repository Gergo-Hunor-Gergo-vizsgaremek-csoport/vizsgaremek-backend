using System.ComponentModel.DataAnnotations;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

/// <summary>
/// "típus"
/// Stores a single type of items
/// </summary>
public class TypeReadDto
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    /// <summary>
    /// Link to the icon of the type
    /// </summary>
    [Required]
    public string Icon { get; set; }
    
}
