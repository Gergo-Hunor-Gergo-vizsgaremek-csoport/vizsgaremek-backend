using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class RendelesGenerator(HttpClient httpClient)
{
    private const string Url = "Rendeles";

    public async Task<List<Guid>> GenerateAsync(int n, List<Guid> userIds, List<Guid> typeIds)
    {
        if (userIds.Count == 0) throw new InvalidOperationException("At least one user is required to generate rendelesek.");
        if (typeIds.Count == 0) throw new InvalidOperationException("At least one type is required to generate rendelesek.");

        List<Guid> ids = [];

        for (int i = 0; i < n; i++)
        {
            int quantity = Random.Shared.Next(1, 6);

            RendelesWriteDto data = new()
            {
                UserId = userIds[Random.Shared.Next(userIds.Count)],
                TypeId = typeIds[Random.Shared.Next(typeIds.Count)],
                Date = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30)),
                Quantity = quantity,
                CompletedQuantity = 0
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            RendelesReadDto? created = await response.Content.ReadFromJsonAsync<RendelesReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}