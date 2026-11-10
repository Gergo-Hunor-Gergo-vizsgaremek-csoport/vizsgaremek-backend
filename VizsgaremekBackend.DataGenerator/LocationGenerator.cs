using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class LocationGenerator(HttpClient httpClient)
{
    private static readonly string[] Types = ["Terem ", "PC", "Labor ", "Tanári ", "Iroda ", "Raktár "];
    private const string Url = "Location";

    public async Task<List<Guid>> GenerateAsync(int n)
    {
        List<Guid> ids = [];

        for (int i = 1; i <= n; i++)
        {
            string type = Types[Random.Shared.Next(Types.Length)];
            string name = $"{type} {i}";

            LocationWriteDto data = new() { Name = name };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            LocationReadDto? created = await response.Content.ReadFromJsonAsync<LocationReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}