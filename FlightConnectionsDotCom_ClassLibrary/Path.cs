using Common_ClassLibrary;
using System.Collections.Generic;

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