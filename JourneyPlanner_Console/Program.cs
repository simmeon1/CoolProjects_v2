using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JourneyPlanner_ClassLibrary.Classes;
using JourneyPlanner_ClassLibrary.FlightConnectionsDotCom;
using JourneyPlanner_ClassLibrary.Workers;

namespace JourneyPlanner_Console
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = System.IO.File.ReadAllText(parametersPath).DeserializeObject<Parameters>();

            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument("--log-level=3");
            chromeOptions.AddArgument("window-size=1280,800");
            if (parameters.Headless) chromeOptions.AddArgument("headless");
            ChromeDriver driver = new(chromeOptions);

            Logger_Console logger = new();
            RealWebDriverWaitProvider webDriverWait = new(driver);
            FlightConnectionsDotComWorker worker = new(logger, driver, webDriverWait);
            RealHttpClient httpClient = new();
            
            JourneyRetrieverComponents components = new(
                driver,
                logger,
                webDriverWait,
                new RealDelayer(),
                httpClient,
                driver
            );

            FullRunner runner = new(
                components,
                fileIo: new RealFileIO(),
                dateTimeProvider: new RealDateTimeProvider(),
                printer: new ExcelPrinter(),
                airportCollector: new FlightConnectionsDotComWorkerAirportCollector(worker),
                airportPopulator: new FlightConnectionsDotComWorkerAirportPopulator(worker)
            );

            bool success = false;
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
                if ((success || parameters.Headless) && driver != null) driver.Quit();
                Console.WriteLine("Run finished. Press any key to continue");
                Console.ReadKey();
            }
        }
    }
}
