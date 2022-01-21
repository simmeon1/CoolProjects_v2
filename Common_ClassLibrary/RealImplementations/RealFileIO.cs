using System.IO;
using System.Threading.Tasks;

namespace Common_ClassLibrary
{
    public class RealFileIO : IFileIO
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
        
        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
        
        public void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
        }
        
        public void Copy(string path, string destFileName, bool overwrite)
        {
            File.Copy(path, destFileName, overwrite);
        }
    }
}