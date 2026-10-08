using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VizsgaremekBackend.Models;

public class User : IdentityUser<Guid>
{
    //Do not add "[Key]"! It is defined by IdentityUser.
    public override Guid Id { get; set; } = Guid.NewGuid();
    
    public ICollection<Log> Logs { get; set; }
    
    [InverseProperty(nameof(Peldany.FelelosUser))]
    public ICollection<Peldany> Peldanys { get; set; }
    
    [InverseProperty(nameof(Kolcsonzes.KolcsonzoUser))]
    public ICollection<Kolcsonzes> Kolcsonzesek { get; set; }
    
    public ICollection<Rendeles> Rendeleses { get; set; }
}