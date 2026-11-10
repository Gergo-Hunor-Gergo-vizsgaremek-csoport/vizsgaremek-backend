namespace VizsgaremekBackend.DataGenerator;

static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Contains("--help", StringComparer.OrdinalIgnoreCase) ||
            args.Contains("-h", StringComparer.OrdinalIgnoreCase))
        {
            PrintHelp();
            return;
        }

        bool recreateDb = args.Contains("--recreate", StringComparer.OrdinalIgnoreCase);
        bool generateChildren = !args.Contains("--no-children", StringComparer.OrdinalIgnoreCase);
        int amount = GetIntArg(args, "--amount", 10);

        if (amount < 1)
        {
            Console.WriteLine("Amount must be at least 1.");
            return;
        }

        if (recreateDb)
        {
            Console.Write("This will recreate the database and delete all existing data. Continue? [y/N]: ");
            string? confirmation = Console.ReadLine();
            if (!string.Equals(confirmation, "y", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(confirmation, "yes", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Aborted.");
                return;
            }
        }

        using HttpClientHandler handler = new()
        {
            // Allow running against a local dev server with a self-signed certificate.
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://localhost:5193")
        };

        if (recreateDb)
        {
            Console.WriteLine("Recreating database...");
            var recreateResponse = await httpClient.GetAsync("Admin/RecreateDatabase");
            recreateResponse.EnsureSuccessStatusCode();
        }

        int locationCount = amount;
        int userCount = amount;
        int typeCount = amount;
        int peldanyCount = amount * 3;
        int rendelesCount = amount;
        int kolcsonzesCount = Math.Max(1, amount);

        Console.WriteLine($"Generating locations ({locationCount})...");
        List<Guid> locationIds = await new LocationGenerator(httpClient).GenerateAsync(locationCount);

        Console.WriteLine($"Generating users ({userCount})...");
        List<Guid> userIds = await new UserGenerator(httpClient).GenerateAsync(userCount);

        Console.WriteLine($"Generating types ({typeCount})...");
        List<Guid> typeIds = await new TypeGenerator(httpClient).GenerateAsync(typeCount);

        Console.WriteLine($"Generating peldanyok ({peldanyCount}, children: {(generateChildren ? "yes" : "no")})...");
        PeldanyGenerationResult peldanyResult = await new PeldanyGenerator(httpClient)
            .GenerateAsync(peldanyCount, typeIds, userIds, locationIds, generateChildren);

        Console.WriteLine($"Generating rendelesek ({rendelesCount})...");
        _ = await new RendelesGenerator(httpClient).GenerateAsync(rendelesCount, userIds, typeIds);

        Console.WriteLine($"Generating kolcsonzesek ({kolcsonzesCount})...");
        await new KolcsonzesGenerator(httpClient).GenerateAsync(kolcsonzesCount, userIds, peldanyResult.LoanableIds);

        Console.WriteLine("Data generation completed successfully.");
    }

    private static int GetIntArg(string[] args, string name, int defaultValue)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (!string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                continue;

            if (i + 1 < args.Length && int.TryParse(args[i + 1], out int value))
                return value;

            Console.WriteLine($"Invalid or missing value for {name}, using default {defaultValue}.");
            return defaultValue;
        }

        return defaultValue;
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Vizsgaremek Data Generator");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project VizsgaremekBackend.DataGenerator/VizsgaremekBackend.DataGenerator.csproj -- [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -h, --help         Show this help message and exit.");
        Console.WriteLine("  --recreate         Recreate the database before generating data.");
        Console.WriteLine("                     Prompts for confirmation before deleting existing data.");
        Console.WriteLine("  --no-children      Do not generate child peldanyok (all peldanyok will be top-level).");
        Console.WriteLine("                     By default, child peldany generation is enabled.");
        Console.WriteLine("  --amount <N>       Base amount of data to generate (default: 10, minimum: 1).");
        Console.WriteLine("                     Derived counts:");
        Console.WriteLine("                       locations/users/types/orders: N");
        Console.WriteLine("                       peldanyok: 3*N");
        Console.WriteLine("                       kolcsonzesek: max(1, N)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project VizsgaremekBackend.DataGenerator/VizsgaremekBackend.DataGenerator.csproj -- --recreate --amount 20");
        Console.WriteLine("  dotnet run --project VizsgaremekBackend.DataGenerator/VizsgaremekBackend.DataGenerator.csproj -- --recreate --no-children --amount 5");
    }
}