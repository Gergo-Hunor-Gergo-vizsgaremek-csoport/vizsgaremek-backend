using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VizsgaremekBackend.Models;

public class Log
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public string Type { get; set; }
    
    [Required]
    public string Message { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    /// <summary>
    /// User responsible for the logged action
    /// </summary>
    [Required]
    public Guid UserId { get; set; }
    
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}