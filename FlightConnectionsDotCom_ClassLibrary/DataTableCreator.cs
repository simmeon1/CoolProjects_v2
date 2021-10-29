﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class DataTableCreator
    {
        private int ColumnIndexCounter { get; set; }
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
            Dictionary<string, string> airportsAndCountries = new();
            foreach (Airport airport in airportList)
            {
                if (!airportsAndCountries.ContainsKey(airport.Code)) airportsAndCountries.Add(airport.Code, airport.Country);
            }

            List<SequentialFlightCollection> reducedList = sequentialCollections
                                                                    .Where(c => !skipUndoableFlights || c.SequenceIsDoable())
                                                                    .Where(c => !skipNotSameDayFinishFlights || c.StartsAndEndsOnSameDay())
                                                                    .Where(c => c.GetTotalTime() <= noLongerThan).ToList();
            
            double avgLength = reducedList.Count == 0 ? 0 : reducedList.Average(x => x.GetTotalTime());
            double avgCost = reducedList.Count == 0 ? 0 : reducedList.Average(x => x.GetCost());

            List<SequentialFlightCollection> reducedAndOrderedList = reducedList
                                                                    .OrderByDescending(c => c.SequenceIsDoable())
                                                                    .ThenByDescending(c => GetBargainPercentage(c, avgLength, avgCost))
                                                                    .ThenByDescending(c => c.StartsAndEndsOnSameDay())
                                                                    .ThenBy(c => c.GetCountOfFlights())
                                                                    .ThenBy(c => c.GetTotalTime())
                                                                    .ThenBy(c => GetCountryChanges(airportsAndCountries, c))
                                                                    .ThenBy(c => c.GetCost())
                                                                    .ToList();

            List<DataTable> tables = new();
            DataTable mainTable = new("Summary");
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("SameDayFinish", TypeBool);
            mainTable.Columns.AddRange(new List<DataColumn> {
                new("Path", TypeString),
                new("Id", TypeInt32),
                new("Count of Flights", TypeInt32),
                doableColumn,
                sameDayFinishColumn,
                new("Start", TypeString),
                new("End", TypeString),
                new("Length", TypeDouble),
                new("Country Changes", TypeInt32),
                new("Cost", TypeDouble),
                new("Bargain %", TypeDouble)
            }.ToArray());
            if (skipUndoableFlights) mainTable.Columns.Remove(doableColumn);
            if (skipNotSameDayFinishFlights) mainTable.Columns.Remove(sameDayFinishColumn);

            DataTable subTable = GetSubTable();

            for (int i = 0; i < reducedAndOrderedList.Count; i++)
            {
                SequentialFlightCollection seqCollection = reducedAndOrderedList[i];
                int id = i + 1;
                ColumnIndexCounter = 0;
                DataRow row = mainTable.NewRow();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetFullPath();
                row[ReturnColumnIndexCounterAndIncrementIt()] = id;
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetCountOfFlights();
                if (!skipUndoableFlights) row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.SequenceIsDoable();
                if (!skipNotSameDayFinishFlights) row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.StartsAndEndsOnSameDay();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetStartTime().ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetEndTime().ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetTotalTime();
                row[ReturnColumnIndexCounterAndIncrementIt()] = GetCountryChanges(airportsAndCountries, seqCollection);
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetCost();
                row[ReturnColumnIndexCounterAndIncrementIt()] = GetBargainPercentage(seqCollection, avgLength, avgCost);
                mainTable.Rows.Add(row);
                AddRowsToSubTable(seqCollection, id, subTable);
            }
            tables.Add(mainTable);
            tables.Add(subTable);
            return tables;
        }

        private static double GetBargainPercentage(SequentialFlightCollection seqCollection, double avgLength, double avgCost)
        {
            return (100 - (seqCollection.GetTotalTime() / avgLength) * 100) + (100 - (seqCollection.GetCost() / avgCost * 100));
        }

        private static int GetCountryChanges(Dictionary<string, string> airportsAndCountries, SequentialFlightCollection c)
        {
            int changes = 0;
            Flight previousFlight = c.FlightCollection[0];
            if (!airportsAndCountries[previousFlight.GetDepartingAirport()].Equals(airportsAndCountries[previousFlight.GetArrivingAirport()])) changes++;
            for (int i = 1; i < c.FlightCollection.Count(); i++)
            {
                Flight currentFlight = c.FlightCollection[i];
                if (!airportsAndCountries[currentFlight.GetArrivingAirport()].Equals(airportsAndCountries[previousFlight.GetArrivingAirport()])) changes++;
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
                new DataColumn("Departing", TypeString),
                new DataColumn("Arriving", TypeString),
                new DataColumn("Duration", TypeString),
                new DataColumn("Airline", TypeString),
                new DataColumn("Cost", TypeDouble)
            }.ToArray());
            return subTable;
        }

        private void AddRowsToSubTable(SequentialFlightCollection sequentialCollection, int id, DataTable subTable)
        {
            for (int i = 0; i < sequentialCollection.Count(); i++)
            {
                Flight flightCollection = sequentialCollection[i];
                ColumnIndexCounter = 0;
                DataRow row = subTable.NewRow();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Path;
                row[ReturnColumnIndexCounterAndIncrementIt()] = id;
                row[ReturnColumnIndexCounterAndIncrementIt()] = i + 1;
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Departing.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Arriving.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Duration.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Airline;
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Cost;
                subTable.Rows.Add(row);
            }
        }

        private int ReturnColumnIndexCounterAndIncrementIt()
        {
            int result = ColumnIndexCounter;
            ColumnIndexCounter++;
            return result;
        }
    }
}