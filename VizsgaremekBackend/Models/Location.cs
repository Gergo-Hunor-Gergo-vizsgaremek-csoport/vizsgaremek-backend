using System.ComponentModel.DataAnnotations.Schema;

namespace VizsgaremekBackend.Models;

/// <summary>
/// "hely"
/// Location where peldans can be
/// </summary>
public class Location
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    
    [InverseProperty(nameof(Peldany.Location))]
    public ICollection<Peldany> Peldanys { get; set; }
}