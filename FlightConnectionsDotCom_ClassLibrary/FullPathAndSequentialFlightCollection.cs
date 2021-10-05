using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FlightConnectionsDotCom_ClassLibrary
{
    public class FullPathAndListOfPathsAndFlightCollections
    {
        public Path Path { get; set; }

        public List<PathAndFlightCollection> PathsAndFlightCollections { get; set; }

        public FullPathAndListOfPathsAndFlightCollections(Path path, List<PathAndFlightCollection> pathsAndFlightCollections)
        {
            Path = path;
            PathsAndFlightCollections = pathsAndFlightCollections;
        }

        public override string ToString()
        {
            return $"{Path}, {PathsAndFlightCollections.Count} paths with flights";
        }
    }
}