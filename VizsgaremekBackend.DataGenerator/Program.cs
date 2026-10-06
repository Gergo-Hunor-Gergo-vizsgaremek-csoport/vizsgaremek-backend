namespace VizsgaremekBackend.DataGenerator;

static class Program
{
    public static void Main(string[] args)
    {
        using HttpClient httpClient = new();
        httpClient.BaseAddress = new Uri("https://localhost:5193");

        _ = httpClient.GetAsync("Admin/RecreateDatabase");
        
        new LocationGenerator(httpClient).Generate(20);
    }
}