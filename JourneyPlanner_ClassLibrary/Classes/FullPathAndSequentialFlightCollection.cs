using Common_ClassLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JourneyPlanner_ClassLibrary
{
    public class FullPathAndListOfPathsAndJourneyCollections
    {
        public Path Path { get; set; }

        public List<PathAndJourneyCollection> PathsAndJourneyCollections { get; set; }

        public FullPathAndListOfPathsAndJourneyCollections(Path path, List<PathAndJourneyCollection> pathsAndJourneyCollections)
        {
            Path = path;
            PathsAndJourneyCollections = pathsAndJourneyCollections;
        }

        public override string ToString()
        {
            return $"{Path}, {PathsAndJourneyCollections.Count} paths with journeys";
        }
    }
}