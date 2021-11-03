using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class DataTableCreator
    {
        private Type TypeString { get; set; }
        private Type TypeInt32 { get; set; }
        private Type TypeDouble { get; set; }
        private Type TypeBool { get; set; }

        public DataTableCreator()
        {
            TypeString = Type.GetType("System.String");
            TypeInt32 = Type.GetType("System.Int32");
            TypeDouble = Type.GetType("System.Double");
            TypeBool = Type.GetType("System.Boolean");
        }

        public List<DataTable> GetTables(List<Airport> airportList, List<SequentialFlightCollection> sequentialCollections, bool skipUndoableFlights, bool skipNotSameDayFinishFlights, int noLongerThan)
        {
            Dictionary<string, Airport> airportDict = new();
            foreach (Airport airport in airportList)
            {
                if (!airportDict.ContainsKey(airport.Code)) airportDict.Add(airport.Code, airport);
            }

            List<SequentialFlightCollection> reducedList = sequentialCollections
                                                                    .Where(c => !skipUndoableFlights || c.SequenceIsDoable())
                                                                    .Where(c => !skipNotSameDayFinishFlights || c.StartsAndEndsOnSameDay())
                                                                    .Where(c => c.GetLength().TotalHours <= noLongerThan).ToList();

            double avgLength = reducedList.Count == 0 ? 0 : reducedList.Average(x => x.GetLength().TotalMinutes);
            double avgCost = reducedList.Count == 0 ? 0 : reducedList.Average(x => x.GetCost());

            List<SequentialFlightCollection> reducedAndOrderedList = reducedList
                                                                    .OrderByDescending(c => c.SequenceIsDoable())
                                                                    .ThenByDescending(c => c.StartsAndEndsOnSameDay())
                                                                    .ThenBy(c => c.GetCountOfFlights())
                                                                    .ThenBy(c => GetCountryChanges(airportDict, c))
                                                                    .ThenBy(c => c.HasFlightWithZeroCost())
                                                                    .ThenByDescending(c => GetBargainPercentage(c, avgLength, avgCost))
                                                                    .ThenBy(c => c.GetLength())
                                                                    .ThenBy(c => c.GetCost())
                                                                    .ThenBy(c => c.GetStartTime())
                                                                    .ToList();

            List<DataTable> tables = new();
            DataTable mainTable = new("Summary");
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("Same Day Finish", TypeBool);
            mainTable.Columns.AddRange(new List<DataColumn> {
                new("Path", TypeString),
                new("Id", TypeInt32),
                new("Count of Flights", TypeInt32),
                doableColumn,
                sameDayFinishColumn,
                new("Start", TypeString),
                new("End", TypeString),
                new("Length", TypeString),
                new("Country Changes", TypeInt32),
                new("Cost", TypeDouble),
                new("Bargain %", TypeDouble),
                new("Has Flight With 0 Cost", TypeBool)
            }.ToArray());
            if (skipUndoableFlights) mainTable.Columns.Remove(doableColumn);
            if (skipNotSameDayFinishFlights) mainTable.Columns.Remove(sameDayFinishColumn);

            DataTable subTable = GetSubTable();

            for (int i = 0; i < reducedAndOrderedList.Count; i++)
            {
                SequentialFlightCollection seqCollection = reducedAndOrderedList[i];
                int id = i + 1;
                int index = 0;
                DataRow row = mainTable.NewRow();
                row[index++] = seqCollection.GetFullPath();
                row[index++] = id;
                row[index++] = seqCollection.GetCountOfFlights();
                if (!skipUndoableFlights) row[index++] = seqCollection.SequenceIsDoable();
                if (!skipNotSameDayFinishFlights) row[index++] = seqCollection.StartsAndEndsOnSameDay();
                row[index++] = GetShortDateTime(seqCollection.GetStartTime());
                row[index++] = GetShortDateTime(seqCollection.GetEndTime());
                row[index++] = GetShortTimeSpan(seqCollection.GetLength());
                row[index++] = GetCountryChanges(airportDict, seqCollection);
                row[index++] = seqCollection.GetCost();
                row[index++] = GetBargainPercentage(seqCollection, avgLength, avgCost);
                row[index++] = seqCollection.HasFlightWithZeroCost();
                mainTable.Rows.Add(row);
                AddRowsToSubTable(seqCollection, id, subTable, airportDict);
            }
            tables.Add(mainTable);
            tables.Add(subTable);
            return tables;
        }

        private static double GetBargainPercentage(SequentialFlightCollection seqCollection, double avgLength, double avgCost)
        {
            return Math.Round((100 - ((seqCollection.GetLength().TotalMinutes / avgLength) * 100)) + (100 - ((seqCollection.GetCost() / avgCost) * 100)), 2);
        }

        private static int GetCountryChanges(Dictionary<string, Airport> airportDict, SequentialFlightCollection c)
        {
            int changes = 0;
            Flight previousFlight = c.FlightCollection[0];
            if (!airportDict[previousFlight.GetDepartingAirport()].Country.Equals(airportDict[previousFlight.GetArrivingAirport()].Country)) changes++;
            for (int i = 1; i < c.FlightCollection.Count(); i++)
            {
                Flight currentFlight = c.FlightCollection[i];
                if (!airportDict[currentFlight.GetArrivingAirport()].Country.Equals(airportDict[previousFlight.GetArrivingAirport()].Country)) changes++;
                previousFlight = currentFlight;
            }
            return changes;
        }

        private DataTable GetSubTable()
        {
            DataTable subTable = new("Details");

            subTable.Columns.AddRange(new List<DataColumn> {
                new DataColumn("Path", TypeString),
                new DataColumn("Id", TypeInt32),
                new DataColumn("Flight #", TypeInt32),
                new DataColumn("Departing Time", TypeString),
                new DataColumn("Arriving Time", TypeString),
                new DataColumn("Departing City", TypeString),
                new DataColumn("Arriving City", TypeString),
                new DataColumn("Departing Country", TypeString),
                new DataColumn("Arriving Country", TypeString),
                new DataColumn("Duration", TypeString),
                new DataColumn("Wait Time From Prev", TypeString),
                new DataColumn("Airline", TypeString),
                new DataColumn("Cost", TypeDouble)
            }.ToArray());
            return subTable;
        }

        private static void AddRowsToSubTable(SequentialFlightCollection sequentialCollection, int id, DataTable subTable, Dictionary<string, Airport> airportDict)
        {
            for (int i = 0; i < sequentialCollection.Count(); i++)
            {
                Flight flight = sequentialCollection[i];
                int index = 0;
                DataRow row = subTable.NewRow();
                row[index++] = flight.Path;
                row[index++] = id;
                row[index++] = i + 1;
                row[index++] = GetShortDateTime(flight.Departing);
                row[index++] = GetShortDateTime(flight.Arriving);
                row[index++] = airportDict[flight.GetDepartingAirport()].City;
                row[index++] = airportDict[flight.GetArrivingAirport()].City;
                row[index++] = airportDict[flight.GetDepartingAirport()].Country;
                row[index++] = airportDict[flight.GetArrivingAirport()].Country;
                row[index++] = GetShortTimeSpan(flight.Duration);
                row[index++] = GetShortTimeSpan(i == 0 ? new TimeSpan() : (flight.Departing - sequentialCollection[i - 1].Arriving));
                row[index++] = flight.Airline;
                row[index++] = flight.Cost;
                subTable.Rows.Add(row);
            }
        }

        public static string GetShortDateTime(DateTime? dt)
        {
            return dt.Value.ToString("dd/MM") + " " + dt.Value.ToString("t");
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