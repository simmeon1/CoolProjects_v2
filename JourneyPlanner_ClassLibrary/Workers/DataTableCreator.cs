using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using JourneyPlanner_ClassLibrary.Classes;

namespace JourneyPlanner_ClassLibrary.Workers
{
    public class DataTableCreator
    {
        private Type TypeString { get; set; } = Type.GetType("System.String");
        private Type TypeInt32 { get; set; } = Type.GetType("System.Int32");
        private Type TypeDouble { get; set; } = Type.GetType("System.Double");
        private Type TypeBool { get; set; } = Type.GetType("System.Boolean");
        private Type TypeDateTime { get; set; } = Type.GetType("System.DateTime");
        private double AvgLength { get; set; }
        private double AvgInitialCost { get; set; }
        private double AvgAirlineCount { get; set; }
        private double AvgFlightCount { get; set; }
        private Dictionary<string, Airport> AirportDict { get; set; }

        public List<DataTable> GetTables(
            List<Airport> airportList,
            List<SequentialJourneyCollection> sequentialCollections
        )
        {
            InitialiseData(airportList, sequentialCollections);
            List<SequentialJourneyCollection> reducedAndOrderedList = GetReducedAndOrderedList(sequentialCollections);
            return GetPopulatedTables(reducedAndOrderedList);
        }

        private List<DataTable> GetPopulatedTables(List<SequentialJourneyCollection> reducedAndOrderedList)
        {
            DataTable mainTable = GetMainTable();
            DataTable subTable = GetSubTable();
            List<DataTable> tables = PopulateTables(reducedAndOrderedList, mainTable, subTable);
            return tables;
        }

        private List<DataTable> PopulateTables(
            List<SequentialJourneyCollection> reducedAndOrderedList,
            DataTable mainTable,
            DataTable subTable
        )
        {
            List<DataTable> tables = new();
            for (int i = 0; i < reducedAndOrderedList.Count; i++)
            {
                SequentialJourneyCollection seqCollection = reducedAndOrderedList[i];
                int id = i + 1;
                AddRowToMainTable(seqCollection, id, mainTable);
                AddRowsToSubTable(seqCollection, id, subTable);
            }

            tables.Add(mainTable);
            tables.Add(subTable);
            return tables;
        }

        private void AddRowToMainTable(SequentialJourneyCollection seqCollection, int id, DataTable mainTable)
        {
            int index = 0;
            DataRow row = mainTable.NewRow();
            row[index++] = seqCollection.GetFullPath();
            row[index++] = id;
            row[index++] = seqCollection.GetCountOfFlights();
            row[index++] = seqCollection.StartsAndEndsOnSameDay();
            row[index++] = GetShortDateTime(seqCollection.GetStartTime());
            row[index++] = GetShortDateTime(seqCollection.GetEndTime());
            row[index++] = GetShortTimeSpan(seqCollection.GetLength());
            row[index++] = seqCollection.GetCountOfAirlines();
            row[index++] = GetCountryChanges(seqCollection);
            row[index++] = seqCollection.GetCost();
            row[index++] = GetBargainPercentage(seqCollection);
            row[index++] = seqCollection.GetCompaniesString();
            row[index++] = seqCollection.HasJourneyWithZeroCost();
            mainTable.Rows.Add(row);
        }

        private DataTable GetMainTable()
        {
            DataTable mainTable = new("Summary");
            DataColumn sameDayFinishColumn = new("Same Day Finish", TypeBool);
            mainTable.Columns.AddRange(
                new List<DataColumn>
                {
                    new("Path", TypeString),
                    new("Id", TypeInt32),
                    new("Flights", TypeInt32),
                    sameDayFinishColumn,
                    new("Start", TypeDateTime),
                    new("End", TypeDateTime),
                    new("Length", TypeString),
                    new("Airline Count", TypeInt32),
                    new("Country Changes", TypeInt32),
                    new("Cost £", TypeDouble),
                    new("Bargain %", TypeDouble),
                    new("Companies", TypeString),
                    new("Has 0 Cost Journey", TypeBool)
                }.ToArray()
            );
            return mainTable;
        }

        private List<SequentialJourneyCollection> GetReducedAndOrderedList(
            List<SequentialJourneyCollection> sequentialCollections
        )
        {
            return sequentialCollections
                .OrderByDescending(c => c.SequenceIsDoable())
                .ThenByDescending(c => c.StartsAndEndsOnSameDay())
                .ThenBy(c => c.GetCountOfFlights())
                .ThenBy(GetCountryChanges)
                .ThenBy(c => c.HasJourneyWithZeroCost())
                .ThenByDescending(GetBargainPercentage)
                .ThenBy(c => c.GetLength())
                .ThenBy(c => c.GetCost())
                .ThenBy(c => c.GetStartTime())
                .ToList();
        }

        private void InitialiseData(List<Airport> airportList, List<SequentialJourneyCollection> sequentialCollections)
        {
            AirportDict = GetAirportDict(airportList);

            AvgLength = GetAverage(sequentialCollections, x => x.GetLength().TotalMinutes);
            GetAverage(sequentialCollections, c => c.GetCost());
            AvgInitialCost = GetAverage(sequentialCollections, x => x.GetCost());
            AvgAirlineCount = GetAverage(sequentialCollections, x => x.GetCountOfAirlines());
            GetAverage(sequentialCollections, GetCountryChanges);
            AvgFlightCount = GetAverage(sequentialCollections, c => c.GetCountOfFlights());
        }

        private static double GetAverage(
            IReadOnlyCollection<SequentialJourneyCollection> sequentialCollections,
            Func<SequentialJourneyCollection, double> selector
        )
        {
            return sequentialCollections.Count == 0 ? 0 : sequentialCollections.Average(selector);
        }

        private static Dictionary<string, Airport> GetAirportDict(List<Airport> airportList)
        {
            Dictionary<string, Airport> airportDict = new();
            foreach (Airport airport in airportList)
            {
                airportDict.TryAdd(airport.Code, airport);
            }

            return airportDict;
        }
        
        private double GetBargainPercentage(SequentialJourneyCollection seqCollection)
        {
            return GetBargainPercentage(seqCollection, seqCollection.GetCost(), AvgInitialCost);
        }

        private double GetBargainPercentage(
            SequentialJourneyCollection seqCollection,
            double journeyCost,
            double avgCost
        )
        {
            return Math.Round(
                (100 - seqCollection.GetLength().TotalMinutes / AvgLength * 100) +
                (100 - journeyCost / avgCost * 100) +
                (100 - seqCollection.GetCountOfFlights() / AvgFlightCount * 100) +
                (100 - seqCollection.GetCountOfAirlines() / AvgAirlineCount * 100),
                2
            );
        }

        private double GetCountryChanges(SequentialJourneyCollection c)
        {
            int changes = 0;
            Journey previousJourney = c.JourneyCollection[0];
            if (!AirportDict[previousJourney.GetDepartingLocation()].Country
                .Equals(AirportDict[previousJourney.GetArrivingLocation()].Country)) changes++;

            for (int i = 1; i < c.JourneyCollection.GetCount(); i++)
            {
                Journey currentJourney = c.JourneyCollection[i];
                if (!AirportDict[currentJourney.GetArrivingLocation()].Country
                    .Equals(AirportDict[previousJourney.GetArrivingLocation()].Country)) changes++;
                previousJourney = currentJourney;
            }

            return changes;
        }

        private DataTable GetSubTable()
        {
            DataTable subTable = new("Details");
            subTable.Columns.AddRange(
                new List<DataColumn>
                {
                    new("Path", TypeString),
                    new("Id", TypeInt32),
                    new("Journey #", TypeInt32),
                    new("Departing Time", TypeDateTime),
                    new("Arriving Time", TypeDateTime),
                    new("Departing Airport", TypeString),
                    new("Arriving Airport", TypeString),
                    new("Departing Country", TypeString),
                    new("Arriving Country", TypeString),
                    new("Company", TypeString),
                    new("Wait Time From Prev", TypeString),
                    new("Length", TypeString),
                    new("Cost £", TypeDouble)
                }.ToArray()
            );
            return subTable;
        }

        private void AddRowsToSubTable(SequentialJourneyCollection sequentialCollection, int id, DataTable subTable)
        {
            for (int i = 0; i < sequentialCollection.Count(); i++)
            {
                Journey journey = sequentialCollection[i];
                int index = 0;
                DataRow row = subTable.NewRow();
                row[index++] = journey.Path;
                row[index++] = id;
                row[index++] = i + 1;
                row[index++] = GetShortDateTime(journey.Departing);
                row[index++] = GetShortDateTime(journey.Arriving);
                row[index++] = AirportDict[journey.GetDepartingLocation()].Name;
                row[index++] = AirportDict[journey.GetArrivingLocation()].Name;
                row[index++] = AirportDict[journey.GetDepartingLocation()].Country;
                row[index++] = AirportDict[journey.GetArrivingLocation()].Country;
                row[index++] = journey.Company;
                row[index++] = GetShortTimeSpan(
                    i == 0 ? new TimeSpan() : (journey.Departing - sequentialCollection[i - 1].Arriving)
                );
                row[index++] = GetShortTimeSpan(journey.Duration);
                row[index++] = journey.Cost;
                subTable.Rows.Add(row);
            }
        }

        private static string GetShortDateTime(DateTime? dt)
        {
            return dt.Value.ToString("dd/MM/yyyy HH:mm:ss");
        }

        public static string GetShortTimeSpan(TimeSpan? ts)
        {
            StringBuilder sb = new($"{Math.Floor(ts.Value.TotalHours)}:{ts.Value.Minutes}");
            if (ts.Value.TotalHours < 10) sb.Insert(0, "0");
            if (ts.Value.Minutes < 10) sb.Append(0);
            return sb.ToString();
        }
    }
}