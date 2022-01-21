using System.IO;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public interface IFileIO
    {
        string ReadAllText(string path);
        void WriteAllText(string path, string contents);
        bool DirectoryExists(string path);
        DirectoryInfo CreateDirectory(string path);
        void AppendAllText(string path, string contents);
        void DeleteFile(string path);
        void DeleteFolder(string path);
        void Copy(string path, string destFileName, bool overwrite);
    }
}