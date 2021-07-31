using System;
using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class RealGuidProvider : IGuidProvider
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}