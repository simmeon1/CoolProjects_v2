using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class RealFileIO : IFileIO
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}