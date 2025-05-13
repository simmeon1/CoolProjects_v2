using System.Collections.ObjectModel;
using System.Text.Json;
using Common_ClassLibrary;

namespace Spotify_ClassLibrary;

public class UkSinglesScrapperUseCase(
    IFileIO fileIo,
    ILogger logger,
    IDelayer delayer,
    IWebDriverWrapper driver,
    IHttpClient http
)
{
    // private readonly ILogger logger = logger;
    // private readonly IDelayer delayer = delayer;

    public async Task Scrap(string resultPath)
    {
        var list = new List<BillboardList>();
        
        var initialDate = DateTime.Now;
        while (initialDate.Year > 1953)
        {
            var address = $"https://www.officialcharts.com/charts/singles-chart/{initialDate:yyyyMMdd}/7501/";
            var message = new HttpRequestMessage(HttpMethod.Get, address);
            var response = await http.SendRequest(message);
            var responseContent = await response.Content.ReadAsStringAsync();
            fileIo.WriteAllText("temp.html", responseContent);
            driver.GoToUrl("file:///C:/Users/simme/source/repos/CoolProjects_v2/Spotify_Console/bin/Debug/net7.0/temp.html");

            var script = @"
var entries = Array.from(document.querySelectorAll('.chart-item-content'));
return entries.map(e => ({
    song: e.querySelector('.chart-name').querySelector('span:last-child').innerText,
    artist: e.querySelector('.chart-artist').querySelector('span:last-child').innerText,
    this_week: e.querySelector('.position').innerText.substring(7).trim(),
    last_week: e.querySelector('[title=""Last week""]').querySelector('span:last-child').innerText,
    peak_position: e.querySelector('.peak').querySelector('span:first-child').innerText,
    weeks_on_chart: e.querySelector('.weeks').querySelector('span:last-child').innerText
}));
";
            var entries = ((ReadOnlyCollection<object>) driver.ExecuteScript(script))
                .Select(e => (Dictionary<string, object>) e)
                .Select(e => new BillboardSong
                {
                    song = (string) e["song"],
                    artist = (string) e["artist"],
                    this_week = int.Parse((string) e["this_week"]),
                    last_week = int.TryParse((string) e["last_week"], out int lw) ? lw : null,
                    peak_position = int.Parse((string) e["peak_position"]),
                    weeks_on_chart = int.Parse((string) e["weeks_on_chart"])
                });
            
            list.Add(new BillboardList()
            {
                date = initialDate.ToString("yyyy-MM-dd"),
                data = entries.ToList()
            });
            
            //Trim, remove debug path
            initialDate = initialDate.AddDays(-7);
        }
        fileIo.WriteAllText("ukSingles.json", JsonSerializer.Serialize(list));
    }
}