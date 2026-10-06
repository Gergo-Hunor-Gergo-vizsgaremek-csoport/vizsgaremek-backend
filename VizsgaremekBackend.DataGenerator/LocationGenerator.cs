using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class LocationGenerator(HttpClient httpClient)
{
    private static readonly string[] Types = ["Terem ", "PC", "Labor ", "Tanári ", "Iroda ", "Raktár "];
    private const string Url = "Location";
    
    public void Generate(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            string type = Types[Random.Shared.Next(Types.Length)];

            string name = type + i;

            LocationWriteDto data =  new() { Name = name };
            
            _ = httpClient.PostAsJsonAsync(Url, data);
        }
    }
}