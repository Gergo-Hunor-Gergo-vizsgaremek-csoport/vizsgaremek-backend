using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VizsgaremekBackend.Models;

/// <summary>
/// "példány"
/// A simgle instance of a type of item
/// </summary>
public class Peldany
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    public Guid TypeId { get; set; }
    
    
    public Guid? ParentId { get; set; }
    
    /// <summary>
    /// The peldany description, that users can append
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Date of the Peldany being added to the system
    /// </summary>
    [Required]
    public DateTime AddDate { get; set; }
    
    public DateTime? ManufacturingDate { get; set; }
    
    /// <summary>
    /// Id of the user responsible for this peldany
    /// </summary>
    public Guid FelelosId { get; set; }
    
    [Required]
    public Guid LocationId { get; set; }
    
    
    [ForeignKey(nameof(TypeId))]
    public Type Type { get; set; }
    
    [ForeignKey(nameof(FelelosId))]
    public User FelelosUser { get; set; }
    
    [ForeignKey(nameof(ParentId))]
    public Peldany Parent { get; set; }
    
    [ForeignKey(nameof(LocationId))]
    public Location Location { get; set; }
    
    public bool IsHibas { get; set; }
    
    /// <summary>
    /// This példány has been marked as selejt, by an administrator
    /// </summary>
    public bool IsSelejt { get; set; }
    
    /// <summary>
    /// This áéldány has been recommended for selejt
    /// </summary>
    public bool IsSelejtSugg { get; set; }
    
    /// <summary>
    /// Date when this példány was marked (not recommended) as selejt
    /// The példány will be deleted after a fixed time after this
    /// </summary>
    public DateTime SelejtedDate { get; set; }
    
    
    [InverseProperty(nameof(Kolcsonzes.Peldany))]
    public ICollection<Kolcsonzes>  Kolcsonzeses { get; set; }
    
    [InverseProperty(nameof(Parent))]
    public ICollection<Peldany> Children { get; set; }
}