using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class UserWriteDto
{
    [MaxLength(50)]
    [Required]
    public string UserName { get; set; }
    
    [MaxLength(50)]
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    /// <summary>
    /// Required when creating a new user. Ignored when updating.
    /// </summary>
    [MinLength(8)]
    public string? Password { get; set; }
}