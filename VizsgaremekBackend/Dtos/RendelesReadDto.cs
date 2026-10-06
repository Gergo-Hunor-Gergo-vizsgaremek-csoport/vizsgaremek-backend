namespace VizsgaremekBackend.Dtos;

public class RendelesReadDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TypeId { get; set; }
    public DateTime Date { get; set; }
    public int Quantity { get; set; }
    public int CompletedQuantity { get; set; }
}