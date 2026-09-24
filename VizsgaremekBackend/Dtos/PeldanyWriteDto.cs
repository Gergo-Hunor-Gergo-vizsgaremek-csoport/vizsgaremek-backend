namespace VizsgaremekBackend.Dtos;

public class PeldanyWriteDto
{
    
    public string? Description { get; set; }
    
    public DateTime AddDate { get; set; }
    
    public DateTime? ManufacturingDate { get; set; }
    
    public bool IsHibas { get; set; }
    
    public bool IsSelejt { get; set; }
    
    public bool IsSelejtSugg { get; set; }
    
    public DateTime SelejtedDate { get; set; }
    
    
    public Guid TypeId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public Guid FelelosId { get; set; }
    
    public Guid LocationId { get; set; }
}