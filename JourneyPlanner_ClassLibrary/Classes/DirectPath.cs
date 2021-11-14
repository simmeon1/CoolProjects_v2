using Common_ClassLibrary;
using System;
using System.Collections.Generic;

namespace JourneyPlanner_ClassLibrary
{
    public class DirectPath
    {
        public Path Path { get; set; }

        public DirectPath()
        {
        }
        
        public DirectPath(string start, string end)
        {
            Path = new Path(new List<string>() { start, end });
        }

        public string GetStart()
        {
            return Path[0];
        }

        public string GetEnd()
        {
            return Path[1];
        }

        public override string ToString()
        {
            return Path.ToString();
        }
    }
}