﻿using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Path
    {
        public List<string> Entries { get; set; }        
        public Path(List<string> path)
        {
            Entries = path;
        }

        public string this[int index]
        {
            get
            {
                return Entries[index];
            }

            set
            {
                Entries[index] = value;
            }
        }

        public override string ToString()
        {
            return Extensions.ConcatenateListOfStringsToDashString(Entries);
        }
        
        public int Count()
        {
            return Entries.Count;
        }
    }
}