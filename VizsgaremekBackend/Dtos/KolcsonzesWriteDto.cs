using System.ComponentModel.DataAnnotations;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class KolcsonzesWriteDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [MaxLength(15)]
    [Required]
    public Guid PeldanyId { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public DateTime? ExpirationDate { get; set; }
}