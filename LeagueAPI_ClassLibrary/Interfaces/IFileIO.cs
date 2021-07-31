using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public interface IFileIO
    {
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
    }
}