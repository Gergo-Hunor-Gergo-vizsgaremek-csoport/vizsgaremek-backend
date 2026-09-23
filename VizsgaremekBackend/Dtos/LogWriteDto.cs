namespace VizsgaremekBackend.Dtos;

public class LogWriteDto
{
    public string Type { get; set; }
    
    public string Message { get; set; }
    
    public DateTime Date { get; set; }
    
    public Guid UserId { get; set; }
}