namespace Common_ClassLibrary
{
    public interface IArchiveExtractor
    {
        void ExtractTar(string file, string destination);
    }
}