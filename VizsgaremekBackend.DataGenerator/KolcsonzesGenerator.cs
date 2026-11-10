using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class KolcsonzesGenerator(HttpClient httpClient)
{
    private const string Url = "Kolcsonzes";

    public async Task<List<Guid>> GenerateAsync(int n, List<Guid> userIds, List<Guid> peldanyIds)
    {
        if (userIds.Count == 0) throw new InvalidOperationException("At least one user is required to generate kolcsonzesek.");
        if (peldanyIds.Count == 0) throw new InvalidOperationException("At least one peldany is required to generate kolcsonzesek.");

        List<Guid> ids = [];
        int count = Math.Min(n, peldanyIds.Count);

        // Use each peldany at most once to avoid conflicts with active loans.
        List<Guid> availablePeldanyIds = [.. peldanyIds];

        for (int i = 0; i < count; i++)
        {
            int index = Random.Shared.Next(availablePeldanyIds.Count);
            Guid peldanyId = availablePeldanyIds[index];
            availablePeldanyIds.RemoveAt(index);

            DateTime date = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 20));

            KolcsonzesWriteDto data = new()
            {
                PeldanyId = peldanyId,
                UserId = userIds[Random.Shared.Next(userIds.Count)],
                Date = date,
                ExpirationDate = date.AddDays(Random.Shared.Next(3, 30))
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            KolcsonzesReadDto? created = await response.Content.ReadFromJsonAsync<KolcsonzesReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}