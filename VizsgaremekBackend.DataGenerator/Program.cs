namespace VizsgaremekBackend.DataGenerator;

static class Program
{
    public static async Task Main(string[] args)
    {
        using HttpClientHandler handler = new()
        {
            // Allow running against a local dev server with a self-signed certificate.
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://localhost:5193")
        };

        Console.WriteLine("Recreating database...");
        var recreateResponse = await httpClient.GetAsync("Admin/RecreateDatabase");
        recreateResponse.EnsureSuccessStatusCode();

        Console.WriteLine("Generating locations...");
        List<Guid> locationIds = await new LocationGenerator(httpClient).GenerateAsync(10);

        Console.WriteLine("Generating users...");
        List<Guid> userIds = await new UserGenerator(httpClient).GenerateAsync(10);

        Console.WriteLine("Generating types...");
        List<Guid> typeIds = await new TypeGenerator(httpClient).GenerateAsync(10);

        Console.WriteLine("Generating peldanyok...");
        PeldanyGenerationResult peldanyResult = await new PeldanyGenerator(httpClient).GenerateAsync(30, typeIds, userIds, locationIds);

        Console.WriteLine("Generating rendelesek...");
        List<Guid> rendelesIds = await new RendelesGenerator(httpClient).GenerateAsync(10, userIds, typeIds);

        Console.WriteLine("Generating kolcsonzesek...");
        await new KolcsonzesGenerator(httpClient).GenerateAsync(12, userIds, peldanyResult.LoanableIds);

        Console.WriteLine("Data generation completed successfully.");
    }
}