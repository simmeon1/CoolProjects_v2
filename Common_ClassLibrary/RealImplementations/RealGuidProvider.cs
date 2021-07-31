using System;
using System.IO;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealGuidProvider : IGuidProvider
    {
        public string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}