using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class UserWriteDto
{
    [MaxLength(50)]
    [Required]
    public string Name { get; set; }
    
    [MaxLength(50)]
    [Required]
    public string Email { get; set; }
    
    /// <summary>
    /// Gives the user unlimited permissions (some actions are only possible with this)
    /// </summary>
    [Required]
    public bool IsSysAdmin { get; set; }
    
    /// <summary>
    /// Can asssign peldanys to users
    /// Can add new peldanys
    /// Can mark peldanys for "selejt"
    /// </summary>
    [Required]
    public bool IsDeviceAdmin { get; set; }
    
    /// <summary>
    /// Can add users
    /// Can view any user data
    /// Can modify user data
    /// Can NOT modify permitions
    /// </summary>
    [Required]
    public bool IsUserAdmin { get; set; }
}