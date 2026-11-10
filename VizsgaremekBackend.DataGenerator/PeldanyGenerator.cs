using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public record PeldanyGenerationResult(List<Guid> AllIds, List<Guid> LoanableIds);

public class PeldanyGenerator(HttpClient httpClient)
{
    private const string Url = "Peldany";

    private static readonly string[] RootDescriptions =
    [
        "Jó állapotú eszköz, mindennapi használatra alkalmas.",
        "Újonnan beszerzett példány, tesztelve és üzemkész.",
        "Használt, de megfelelően karbantartott eszköz.",
        "Tartalék példány az oktatási termek számára.",
        "Kölcsönözhető eszköz diákok és tanárok részére."
    ];

    private static readonly string[] ChildDescriptions =
    [
        "Tartozék a szülő eszközhöz rendelve.",
        "Cserealkatrész a hozzá tartozó példányhoz.",
        "Kiegészítő elem, amely a fő eszközzel együtt kezelendő.",
        "A szülő példány részegysége."
    ];

    public async Task<PeldanyGenerationResult> GenerateAsync(int n, List<Guid> typeIds, List<Guid> userIds, List<Guid> locationIds, bool generateChildren = true)
    {
        if (n <= 0) return new PeldanyGenerationResult([], []);
        if (typeIds.Count == 0) throw new InvalidOperationException("At least one type is required to generate peldanyok.");
        if (userIds.Count == 0) throw new InvalidOperationException("At least one user is required to generate peldanyok.");
        if (locationIds.Count == 0) throw new InvalidOperationException("At least one location is required to generate peldanyok.");

        List<PeldanyReadDto> createdPeldanys = [];

        // If children are disabled, create only top-level (loanable) peldanys.
        if (!generateChildren)
        {
            for (int i = 1; i <= n; i++)
            {
                PeldanyReadDto? created = await CreatePeldanyAsync(
                    serialIndex: i,
                    typeIds: typeIds,
                    userIds: userIds,
                    locationId: locationIds[Random.Shared.Next(locationIds.Count)],
                    parent: null);

                if (created is not null)
                    createdPeldanys.Add(created);
            }
        }
        else
        {
            // Create a base set of top-level (root) peldanys first so children can reference existing parents.
            int rootCount = Math.Max(1, (int)Math.Ceiling(n * 0.6));
            rootCount = Math.Min(rootCount, n);

            for (int i = 1; i <= rootCount; i++)
            {
                PeldanyReadDto? created = await CreatePeldanyAsync(
                    serialIndex: i,
                    typeIds: typeIds,
                    userIds: userIds,
                    locationId: locationIds[Random.Shared.Next(locationIds.Count)],
                    parent: null);

                if (created is not null)
                    createdPeldanys.Add(created);
            }

            // Create remaining peldanys as children of existing peldanys (roots or other children).
            // A child must have the same location as its parent and must not receive an active kolcsonzes.
            for (int i = rootCount + 1; i <= n; i++)
            {
                PeldanyReadDto parent = createdPeldanys[Random.Shared.Next(createdPeldanys.Count)];

                PeldanyReadDto? created = await CreatePeldanyAsync(
                    serialIndex: i,
                    typeIds: typeIds,
                    userIds: userIds,
                    locationId: parent.LocationId,
                    parent: parent);

                if (created is not null)
                    createdPeldanys.Add(created);
            }
        }

        List<Guid> allIds = createdPeldanys.Select(p => p.Id).ToList();
        List<Guid> loanableIds = createdPeldanys
            .Where(p => p.ParentId is null)
            .Select(p => p.Id)
            .ToList();

        return new PeldanyGenerationResult(allIds, loanableIds);
    }

    private async Task<PeldanyReadDto?> CreatePeldanyAsync(
        int serialIndex,
        List<Guid> typeIds,
        List<Guid> userIds,
        Guid locationId,
        PeldanyReadDto? parent)
    {
        DateTime addDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365));
        DateTime? manufacturingDate = addDate.AddDays(-Random.Shared.Next(30, 1200));

        string description = parent is null
            ? RootDescriptions[Random.Shared.Next(RootDescriptions.Length)]
            : ChildDescriptions[Random.Shared.Next(ChildDescriptions.Length)];

        PeldanyWriteDto data = new()
        {
            Description = description,
            AddDate = addDate,
            ManufacturingDate = manufacturingDate,
            IsHibas = false,
            IsSelejt = false,
            IsSelejtSugg = false,
            SelejtedDate = default,
            SerialNumber = $"SN-{serialIndex:0000}-{Random.Shared.Next(1000, 9999)}",
            TypeId = typeIds[Random.Shared.Next(typeIds.Count)],
            ParentId = parent?.Id,
            FelelosId = userIds[Random.Shared.Next(userIds.Count)],
            LocationId = locationId
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PeldanyReadDto>();
    }
}