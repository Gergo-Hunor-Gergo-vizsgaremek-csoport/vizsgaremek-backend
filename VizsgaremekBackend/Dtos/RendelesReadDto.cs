namespace VizsgaremekBackend.Dtos;

public class RendelesReadDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid TypeId { get; set; }
}