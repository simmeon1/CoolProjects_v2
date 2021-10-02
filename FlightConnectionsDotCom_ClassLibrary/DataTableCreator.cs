using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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

        public List<DataTable> GetTables(List<SequentialFlightCollection> sequentialCollections)
        {
            List<DataTable> tables = new();

            DataTable mainTable = new("0");
            DataColumn pathColumn = new("Path", TypeString);
            DataColumn idColumn = new("Id", TypeInt32);
            DataColumn doableColumn = new("Doable", TypeBool);
            DataColumn sameDayFinishColumn = new("SameDayFinish", TypeBool);
            DataColumn startColumn = new("Start", TypeString);
            DataColumn endColumn = new("End", TypeString);
            DataColumn costColumn = new("Cost", TypeDouble);
            mainTable.Columns.AddRange(new List<DataColumn> { pathColumn, idColumn, doableColumn, sameDayFinishColumn, startColumn, endColumn, costColumn }.ToArray());

            for (int i = 0; i < sequentialCollections.Count; i++)
            {
                SequentialFlightCollection seqCollection = sequentialCollections[i];
                int id = i + 1;
                ColumnIndexCounter = 0;
                DataRow row = mainTable.NewRow();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetFullPath();
                row[ReturnColumnIndexCounterAndIncrementIt()] = id;
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.SequenceIsDoable();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.StartsAndEndsOnSameDay();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetStartTime().ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetEndTime().ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = seqCollection.GetCost();
                mainTable.Rows.Add(row);
                tables.Add(GetSubTable(seqCollection, id));
            }
            tables.Add(mainTable);
            return tables.OrderBy(t => t.TableName).ToList();
        }

        private DataTable GetSubTable(SequentialFlightCollection sequentialCollection, int id)
        {
            DataTable subTable = new(id.ToString());
            DataColumn pathColumn = new("Path", TypeString);
            DataColumn departingColumn = new("Departing", TypeString);
            DataColumn arrivingColumn = new("Arriving", TypeString);
            DataColumn durationColumn = new("Duration", TypeString);
            DataColumn airlineColumn = new("Airline", TypeString);
            DataColumn costColumn = new("Cost", TypeDouble);
            subTable.Columns.AddRange(new List<DataColumn> { pathColumn, departingColumn, arrivingColumn, durationColumn, airlineColumn, costColumn }.ToArray());

            for (int i = 0; i < sequentialCollection.Count(); i++)
            {
                Flight flightCollection = sequentialCollection[i];
                ColumnIndexCounter = 0;
                DataRow row = subTable.NewRow();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Path;
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Departing.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Arriving.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Duration.ToString();
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Airline;
                row[ReturnColumnIndexCounterAndIncrementIt()] = flightCollection.Cost;
                subTable.Rows.Add(row);
            }
            return subTable;
        }

        private int ReturnColumnIndexCounterAndIncrementIt()
        {
            int result = ColumnIndexCounter;
            ColumnIndexCounter++;
            return result;
        }
    }
}