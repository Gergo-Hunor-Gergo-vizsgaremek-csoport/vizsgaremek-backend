using System.Net.Http.Json;
using VizsgaremekBackend.Dtos;

namespace VizsgaremekBackend.DataGenerator;

public class TypeGenerator(HttpClient httpClient)
{
    private const string Url = "Type";

    private static readonly (string Name, string Description, string Icon)[] Items =
    [
        ("Laptop", "Hordozható számítógép oktatási és irodai feladatokhoz.", "https://example.com/icons/laptop.png"),
        ("Projektor", "Terembe szerelhető vagy hordozható kivetítő prezentációkhoz.", "https://example.com/icons/projector.png"),
        ("Monitor", "Asztali kijelző számítógépekhez és munkaállomásokhoz.", "https://example.com/icons/monitor.png"),
        ("Billentyűzet", "Vezetékes vagy vezeték nélküli beviteli eszköz.", "https://example.com/icons/keyboard.png"),
        ("Egér", "Optikai egér számítógépes használathoz.", "https://example.com/icons/mouse.png"),
        ("Nyomtató", "Lézernyomtató dokumentumok nyomtatásához.", "https://example.com/icons/printer.png"),
        ("Router", "Hálózati eszköz internetkapcsolat megosztásához.", "https://example.com/icons/router.png"),
        ("Switch", "Hálózati kapcsoló több eszköz vezetékes összekötéséhez.", "https://example.com/icons/switch.png"),
        ("Tablet", "Érintőképernyős táblagép mobil feladatokhoz.", "https://example.com/icons/tablet.png"),
        ("Webkamera", "Kamera online megbeszélésekhez és rögzítéshez.", "https://example.com/icons/webcam.png"),
        ("Fejhallgató", "Zárt fejhallgató oktatáshoz és kommunikációhoz.", "https://example.com/icons/headphones.png"),
        ("Mikrofon", "Asztali mikrofon hangfelvételhez és konferenciákhoz.", "https://example.com/icons/microphone.png")
    ];

    public async Task<List<Guid>> GenerateAsync(int n)
    {
        List<Guid> ids = [];
        int count = Math.Min(n, Items.Length);

        for (int i = 0; i < count; i++)
        {
            var (name, description, icon) = Items[i];

            TypeWriteDto data = new()
            {
                Name = name,
                Description = description,
                Icon = icon
            };

            HttpResponseMessage response = await httpClient.PostAsJsonAsync(Url, data);
            response.EnsureSuccessStatusCode();

            TypeReadDto? created = await response.Content.ReadFromJsonAsync<TypeReadDto>();
            if (created is not null)
                ids.Add(created.Id);
        }

        return ids;
    }
}