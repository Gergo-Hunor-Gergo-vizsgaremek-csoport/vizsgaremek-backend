namespace VizsgaremekBackend.Dtos;

public class KolcsonzottPeldanyReadDto
{
    public Guid KolcsonzesId { get; set; }
    
    public Guid PeldanyId { get; set; }
    
    /// <summary>
    /// Time of kolcsonzes creation
    /// </summary>
    public DateTime KolcsonzesDate { get; set; }
    
    /// <summary>
    /// Time when the kolcsonzes ends
    /// </summary>
    public DateTime? ExpirationDate { get; set; }
    
    public string TypeName { get; set; }
    
    public string SerialNumber { get; set; }
    
    public string? Description { get; set; }
}