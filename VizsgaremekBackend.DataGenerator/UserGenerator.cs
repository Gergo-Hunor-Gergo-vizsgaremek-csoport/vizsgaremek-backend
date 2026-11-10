using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class UserGenerator(HttpClient httpClient)
{
    private const string Url = "User";

    private static readonly string[] FirstNames =
    [
        "Anna", "Bence", "Csilla", "Dávid", "Eszter", "Ferenc",
        "Gábor", "Hanna", "István", "Júlia", "Katalin", "László"
    ];

    private static readonly string[] LastNames =
    [
        "Kovács", "Nagy", "Tóth", "Szabó", "Horváth", "Varga",
        "Kiss", "Molnár", "Németh", "Farkas", "Balogh", "Papp"
    ];

    public async Task<List<Guid>> GenerateAsync(int n)
    {
        List<Guid> ids = [];

        for (int i = 0; i < n; i++)
        {
            string first = FirstNames[Random.Shared.Next(FirstNames.Length)];
            string last = LastNames[Random.Shared.Next(LastNames.Length)];

            string baseName = $"{first}.{last}".ToLower()
                .Replace('á', 'a').Replace('é', 'e').Replace('í', 'i')
                .Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o')
                .Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');

            UserWriteDto data = new()
            {
                UserName = $"{baseName}{i + 1}",
                Email = $"{baseName}{i + 1}@example.com",
                Password = "Password123!"
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            UserReadDto? created = await response.Content.ReadFromJsonAsync<UserReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}