using System;
using System.Collections.Generic;
using System.Linq;

namespace JourneyPlanner_ClassLibrary
{
    public class PathsAndWorkerGroup
    {
        public List<Path> Paths { get; set; }
        public string Worker { get; set; }
        public PathsAndWorkerGroup(List<Path> paths, string worker)
        {
            if (paths.Any(p => p.Count() != 2)) throw new Exception("Paths contains a path with entries count different than 2.");
            Paths = paths;
            Worker = worker;
        }
    }
}