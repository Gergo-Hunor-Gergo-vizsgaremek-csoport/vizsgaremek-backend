using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class LocationWriteDto
{
    public Guid Id { get; set; }
    
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(50)]
    [InverseProperty(nameof(Peldany.Location))]
    public ICollection<Peldany> Peldanys { get; set; }
}