using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.FlightConnectionsDotCom;
using JourneyPlanner_ClassLibrary.Workers;
using OpenQA.Selenium.Chrome;
using ChromeDriverService = Common_ClassLibrary.ChromeDriverService;

namespace JourneyPlanner_Console;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var parametersPath = "";
        foreach (var arg in args)
        {
            var match = Regex.Match(arg, "parametersPath-(.*)");
            if (match.Success)
            {
                parametersPath = match.Groups[1].Value;
            }
        }
        var parameters = (await File.ReadAllTextAsync(parametersPath)).DeserializeObject<Parameters>();

        RealHttpClient httpClient = new ();
        Logger_Console logger = new ();

        var fileIo = new RealFileIO();
        var savePath = parameters.FileSavePath;
        await ChromeDriverService.GetLatestChromeDriver(logger, httpClient, savePath, fileIo);

        ChromeOptions chromeOptions = new ();
        chromeOptions.AddArgument("--log-level=3");
        chromeOptions.AddArgument("window-size=1280,800");
        if (parameters.Headless)
        {
            chromeOptions.AddArgument("headless");
        }
        ChromeDriver driver = new ($"{savePath}\\chromeDriverFolder\\chromedriver-win64", chromeOptions);
        driver.Manage().Window.Maximize();

        RealWebDriverWaitProvider webDriverWait = new (driver);
        FlightConnectionsDotComWorker worker = new (logger, driver, webDriverWait);

        FullRunner runner = new (
            driver,
            logger,
            webDriverWait,
            new RealDelayer(),
            httpClient,
            driver,
            fileIo: fileIo,
            dateTimeProvider: new RealDateTimeProvider(),
            printer: new ExcelPrinter(),
            airportCollector: new FlightConnectionsDotComWorkerAirportCollector(worker),
            airportPopulator: new FlightConnectionsDotComWorkerAirportPopulator(worker)
        );

        var success = false;
        try
        {
            await runner.DoRun(parameters);
            success = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception appeared during run. Details:");
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            if (success || parameters.Headless)
            {
                driver.Quit();
                Console.WriteLine("Run finished.");
            }
            else
            {
                Console.WriteLine("Run failed. Press any key to close browser.");
                Console.ReadKey();
                driver.Quit();
            }
        }
    }
}