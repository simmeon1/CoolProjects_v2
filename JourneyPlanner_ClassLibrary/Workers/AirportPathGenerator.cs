using System.Collections.Generic;
using System.Linq;
using Common_ClassLibrary;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class AirportPathGenerator
    {
        private readonly List<Airport> airportsList;

        public AirportPathGenerator(
            ILogger logger,
            Dictionary<string, HashSet<string>> airportsAndDestinations,
            List<Airport> airportsList
        )
        {
            this.airportsList = airportsList;
            Logger = logger;
            AirportsAndDestinations = airportsAndDestinations;
        }

        private Dictionary<string, HashSet<string>> AirportsAndDestinations { get; set; }
        private LinkedList<string> CurrentPath { get; set; }
        private int MaxFlights { get; set; }
        private List<Path> Paths { get; set; }
        private ILogger Logger { get; set; }

        public List<Path> GeneratePaths(List<string> origins, List<string> targets, int maxFlights)
        {
            origins = ParseAirports(origins);
            targets = ParseAirports(targets);

            Paths = new List<Path>();
            MaxFlights = maxFlights;

            var total = origins.Count * targets.Count;
            var counter = 0;
            foreach (var origin in origins)
            {
                foreach (var target in targets)
                {
                    counter++;
                    Logger.Log(
                        $"Generating paths from {origin} to {target} {Globals.GetPercentageAndCountString(counter, total)}"
                    );
                    CurrentPath = new LinkedList<string>();
                    UpdateCurrentPathAndScanItIfNeeded(origin, target);
                }
            }

            return Paths.OrderBy(GetCountOfJourneys).ThenBy(p => p.Count()).ThenBy(p => p.ToString()).ToList();
        }

        private List<string> ParseAirports(List<string> places) => places.GroupJoin(
            airportsList,
            x => x,
            x => x.Country,
            (x, y) =>
            {
                y = y.ToList();
                return y.Any() ? y.Select(z => z.Code) : new[] { x };
            }
        ).SelectMany(x => x).ToList();

        private static int GetCountOfJourneys(Path p) => p.Count() - 1;

        private void UpdateCurrentPathAndScanItIfNeeded(string origin, string target)
        {
            CurrentPath.AddLast(origin);
            Path path = new(CurrentPath.ToList());
            var flightCount = GetCountOfJourneys(path);
            var maxCountsNotPassed = flightCount <= MaxFlights;
            if (maxCountsNotPassed && !CurrentPathContainsRepeatedAirport())
            {
                if (origin.Equals(target))
                {
                    Paths.Add(new Path(new List<string>(CurrentPath)));
                }
                ScanCurrentPath(origin, target);
            }
            CurrentPath.RemoveLast();
        }

        private bool CurrentPathContainsRepeatedAirport()
        {
            HashSet<string> airportOccurrences = new();
            foreach (var airport in CurrentPath)
            {
                if (airportOccurrences.Contains(airport))
                {
                    return true;
                }
                airportOccurrences.Add(airport);
            }
            return false;
        }

        private void ScanCurrentPath(string origin, string target)
        {
            var destinations = AirportsAndDestinations[origin];
            foreach (var destination in destinations)
            {
                UpdateCurrentPathAndScanItIfNeeded(destination, target);
            }
        }
    }
}