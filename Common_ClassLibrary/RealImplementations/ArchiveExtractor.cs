using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.IO;

namespace Common_ClassLibrary
{
    public class ArchiveExtractor : IArchiveExtractor
    {
        public void ExtractTar(string file, string destination)
        {
            Stream inStream = File.OpenRead($"{file}");
            Stream gzipStream = new GZipInputStream(inStream);

            TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
            tarArchive.ExtractContents($"{destination}", true);
            tarArchive.Close();

            gzipStream.Close();
            inStream.Close();
        }
    }
}