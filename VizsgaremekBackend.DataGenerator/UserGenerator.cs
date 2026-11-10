using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class UserGenerator(HttpClient httpClient)
{
    private const string Url = "User";

    private static readonly (string First, string Last)[] People =
    [
        ("Anna", "Kovács"),
        ("Bence", "Nagy"),
        ("Csilla", "Tóth"),
        ("Dávid", "Szabó"),
        ("Eszter", "Horváth"),
        ("Ferenc", "Varga"),
        ("Gábor", "Kiss"),
        ("Hanna", "Molnár"),
        ("István", "Németh"),
        ("Júlia", "Farkas"),
        ("Katalin", "Balogh"),
        ("László", "Papp")
    ];

    public async Task<List<Guid>> GenerateAsync(int n)
    {
        List<Guid> ids = [];
        int count = Math.Min(n, People.Length);

        for (int i = 0; i < count; i++)
        {
            var (first, last) = People[i];
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