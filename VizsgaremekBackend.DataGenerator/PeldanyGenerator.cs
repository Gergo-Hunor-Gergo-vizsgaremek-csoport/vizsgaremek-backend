using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class PeldanyGenerator(HttpClient httpClient)
{
    private const string Url = "Peldany";

    private static readonly string[] Descriptions =
    [
        "Jó állapotú eszköz, mindennapi használatra alkalmas.",
        "Újonnan beszerzett példány, tesztelve és üzemkész.",
        "Használt, de megfelelően karbantartott eszköz.",
        "Tartalék példány az oktatási termek számára.",
        "Kölcsönözhető eszköz diákok és tanárok részére."
    ];

    public async Task<List<Guid>> GenerateAsync(int n, List<Guid> typeIds, List<Guid> userIds, List<Guid> locationIds)
    {
        if (typeIds.Count == 0) throw new InvalidOperationException("At least one type is required to generate peldanyok.");
        if (userIds.Count == 0) throw new InvalidOperationException("At least one user is required to generate peldanyok.");
        if (locationIds.Count == 0) throw new InvalidOperationException("At least one location is required to generate peldanyok.");

        List<Guid> ids = [];

        for (int i = 1; i <= n; i++)
        {
            DateTime addDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365));
            DateTime? manufacturingDate = addDate.AddDays(-Random.Shared.Next(30, 1200));

            PeldanyWriteDto data = new()
            {
                Description = Descriptions[Random.Shared.Next(Descriptions.Length)],
                AddDate = addDate,
                ManufacturingDate = manufacturingDate,
                IsHibas = false,
                IsSelejt = false,
                IsSelejtSugg = false,
                SelejtedDate = default,
                SerialNumber = $"SN-{i:0000}-{Random.Shared.Next(1000, 9999)}",
                TypeId = typeIds[Random.Shared.Next(typeIds.Count)],
                ParentId = null,
                FelelosId = userIds[Random.Shared.Next(userIds.Count)],
                LocationId = locationIds[Random.Shared.Next(locationIds.Count)]
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            PeldanyReadDto? created = await response.Content.ReadFromJsonAsync<PeldanyReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}