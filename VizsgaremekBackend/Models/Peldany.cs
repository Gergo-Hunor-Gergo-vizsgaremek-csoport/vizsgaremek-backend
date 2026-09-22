using System.ComponentModel.DataAnnotations;

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
}