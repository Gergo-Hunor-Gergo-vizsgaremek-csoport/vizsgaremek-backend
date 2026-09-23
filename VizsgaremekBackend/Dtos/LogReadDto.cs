namespace VizsgaremekBackend.Dtos;

public class LogReadDto
{
    public Guid Id { get; set; }
    
    public string Type { get; set; }
    
    public string Message { get; set; }
    
    public DateTime Date { get; set; }
    
    public Guid UserId { get; set; }
}