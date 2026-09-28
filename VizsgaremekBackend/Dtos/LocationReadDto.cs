using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VizsgaremekBackend.Models;

namespace VizsgaremekBackend.Dtos;

public class LocationReadDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    
    [InverseProperty(nameof(Peldany.Location))]
    public ICollection<Peldany> Peldanys { get; set; }
}