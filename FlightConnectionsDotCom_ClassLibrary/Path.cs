using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class Path
    {
        public List<string> Entries { get; set; }

        public Path(List<string> path)
        {
            Entries = path;
        }

        public override string ToString()
        {
            return Extensions.ConcatenateListOfStringsToDashString(Entries);
        }
    }
}