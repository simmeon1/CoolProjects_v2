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
        private Type TypeString { get; set; }
        private Type TypeInt32 { get; set; }
        private Type TypeDouble { get; set; }
        private Type TypeBool { get; set; }
        private Type TypeDateTime { get; set; }
        private double AvgLength { get; set; }
        private double AvgCost { get; set; }
        private double AvgCompanyCount { get; set; }
        private bool SkipUndoableJourneys { get; set; }
        private bool SkipNotSameDayFinishJourneys { get; set; }
        private Dictionary<string, Airport> AirportDict { get; set; }
        private Dictionary<SequentialJourneyCollection, double> JourneyExtraCosts { get; set; }

        public DataTableCreator()
        {
            TypeString = Type.GetType("System.String");
            TypeInt32 = Type.GetType("System.Int32");
            TypeDouble = Type.GetType("System.Double");
            TypeBool = Type.GetType("System.Boolean");
            TypeDateTime = Type.GetType("System.DateTime");
        }

        public List<DataTable> GetTables(
            List<Airport> airportList,
            List<SequentialJourneyCollection> sequentialCollections,
            bool skipUndoableJourneys,
            bool skipNotSameDayFinishJourneys,
            string home,
            double transportFromHomeCost,
            double extraCostPerFlight
        )
        {
            InitialiseData(airportList, sequentialCollections, home, transportFromHomeCost, extraCostPerFlight,
                skipUndoableJourneys, skipNotSameDayFinishJourneys);
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
            DataTable mainTable, DataTable subTable
        ) {
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
            row[index++] = seqCollection.GetCountOfBuses();
            if (!SkipUndoableJourneys) row[index++] = seqCollection.SequenceIsDoable();
            if (!SkipNotSameDayFinishJourneys) row[index++] = seqCollection.StartsAndEndsOnSameDay();
            row[index++] = GetShortDateTime(seqCollection.GetStartTime());
            row[index++] = GetShortDateTime(seqCollection.GetEndTime());
            row[index++] = GetShortTimeSpan(seqCollection.GetLength());
            row[index++] = GetFullCostForJourney(seqCollection);
            row[index++] = seqCollection.GetCountOfCompanies();
            row[index++] = GetCountryChanges(seqCollection);
            row[index++] = GetBargainPercentage(seqCollection);
            row[index++] = seqCollection.GetCost();
            row[index++] = JourneyExtraCosts[seqCollection];
            row[index++] = seqCollection.HasJourneyWithZeroCost();
            mainTable.Rows.Add(row);
        }

        private DataTable GetMainTable()
        {
            DataTable mainTable = new("Summary");
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("Same Day Finish", TypeBool);
            mainTable.Columns.AddRange(new List<DataColumn>
            {
                new("Path", TypeString),
                new("Id", TypeInt32),
                new("Flights", TypeInt32),
                new("Buses", TypeInt32),
                doableColumn,
                sameDayFinishColumn,
                new("Start", TypeDateTime),
                new("End", TypeDateTime),
                new("Length", TypeString),
                new("Total Cost £", TypeDouble),
                new("Companies", TypeInt32),
                new("Country Changes", TypeInt32),
                new("Bargain %", TypeDouble),
                new("Cost £", TypeDouble),
                new("Extra Cost £", TypeDouble),
                new("Has 0 Cost Journey", TypeBool)
            }.ToArray());
            if (SkipUndoableJourneys) mainTable.Columns.Remove(doableColumn);
            if (SkipNotSameDayFinishJourneys) mainTable.Columns.Remove(sameDayFinishColumn);
            return mainTable;
        }

        private List<SequentialJourneyCollection> GetReducedAndOrderedList(
            List<SequentialJourneyCollection> sequentialCollections
        ) {
            return sequentialCollections
                .OrderByDescending(c => c.SequenceIsDoable())
                .ThenByDescending(c => c.StartsAndEndsOnSameDay())
                .ThenBy(c => c.GetCountOfFlights())
                .ThenBy(GetCountryChanges)
                .ThenBy(c => c.HasJourneyWithZeroCost())
                .ThenByDescending(GetBargainPercentage)
                .ThenBy(c => c.GetLength())
                .ThenBy(GetFullCostForJourney)
                .ThenBy(c => c.GetStartTime())
                .ToList();
        }

        private void InitialiseData(List<Airport> airportList, List<SequentialJourneyCollection> sequentialCollections,
            string home, double transportFromHomeCost,
            double extraCostPerFlight, bool skipUndoableJourneys, bool skipNotSameDayFinishJourneys)
        {
            AirportDict = GetAirportDict(airportList);
            JourneyExtraCosts =
                GetExtraCostsForJourneys(sequentialCollections, home, transportFromHomeCost, extraCostPerFlight);
            AvgLength = sequentialCollections.Count == 0
                ? 0
                : sequentialCollections.Average(x => x.GetLength().TotalMinutes);
            AvgCost = sequentialCollections.Count == 0 ? 0 : sequentialCollections.Average(GetFullCostForJourney);
            AvgCompanyCount = sequentialCollections.Count == 0
                ? 0
                : sequentialCollections.Average(x => x.GetCountOfCompanies());
            SkipUndoableJourneys = skipUndoableJourneys;
            SkipNotSameDayFinishJourneys = skipNotSameDayFinishJourneys;
        }

        private static Dictionary<string, Airport> GetAirportDict(List<Airport> airportList)
        {
            Dictionary<string, Airport> airportDict = new();
            foreach (Airport airport in airportList)
            {
                if (!airportDict.ContainsKey(airport.Code)) airportDict.Add(airport.Code, airport);
            }
            return airportDict;
        }

        private double GetFullCostForJourney(SequentialJourneyCollection x)
        {
            return x.GetCost() + JourneyExtraCosts[x];
        }

        private static Dictionary<SequentialJourneyCollection, double> GetExtraCostsForJourneys(
            List<SequentialJourneyCollection> sequentialCollections,
            string home,
            double transportFromHomeCost,
            double extraCostPerFlight
        )
        {
            Dictionary<SequentialJourneyCollection, double> journeyExtraCosts = new();
            foreach (SequentialJourneyCollection col in sequentialCollections)
            {
                double extraCost = 0;
                if (!col.GetDepartingLocation().Equals(home)) extraCost += transportFromHomeCost;
                extraCost += col.GetCountOfFlights() * extraCostPerFlight;
                journeyExtraCosts.Add(col, extraCost);
            }

            return journeyExtraCosts;
        }

        private double GetBargainPercentage(SequentialJourneyCollection seqCollection)
        {
            return Math.Round(
                (100 - ((seqCollection.GetLength().TotalMinutes / AvgLength) * 100)) +
                (100 - ((GetFullCostForJourney(seqCollection) / AvgCost) * 100)) +
                (100 - ((seqCollection.GetCountOfCompanies() / AvgCompanyCount) * 100))
            , 2);
        }

        private int GetCountryChanges(SequentialJourneyCollection c)
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
            subTable.Columns.AddRange(new List<DataColumn>
            {
                new("Path", TypeString),
                new("Id", TypeInt32),
                new("Journey #", TypeInt32),
                new("Is Flight", TypeBool),
                new("Departing Time", TypeDateTime),
                new("Arriving Time", TypeDateTime),
                new("Departing City", TypeString),
                new("Arriving City", TypeString),
                new("Departing Country", TypeString),
                new("Arriving Country", TypeString),
                new("Company", TypeString),
                new("Wait Time From Prev", TypeString),
                new("Length", TypeString),
                new("Cost £", TypeDouble)
            }.ToArray());
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
                row[index++] = journey.IsFlight();
                row[index++] = GetShortDateTime(journey.Departing);
                row[index++] = GetShortDateTime(journey.Arriving);
                row[index++] = AirportDict[journey.GetDepartingLocation()].City;
                row[index++] = AirportDict[journey.GetArrivingLocation()].City;
                row[index++] = AirportDict[journey.GetDepartingLocation()].Country;
                row[index++] = AirportDict[journey.GetArrivingLocation()].Country;
                row[index++] = journey.Company;
                row[index++] = GetShortTimeSpan(i == 0 ? new TimeSpan() : (journey.Departing - sequentialCollection[i - 1].Arriving));
                row[index++] = GetShortTimeSpan(journey.Duration);
                row[index++] = journey.Cost;
                subTable.Rows.Add(row);
            }
        }

        public static string GetShortDateTime(DateTime? dt)
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