using System.Collections.Generic;
using System.IO;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class MatchAddedHandler : IMatchAddedHandler
    {
        private readonly IFileIO fileIo;
        private readonly IMatchSaver matchSaver;
        private readonly string lockFile;

        public MatchAddedHandler(IFileIO fileIo, IMatchSaver matchSaver, string outputDirectory)
        {
            this.fileIo = fileIo;
            this.matchSaver = matchSaver;
            lockFile = Path.Combine(outputDirectory, "deleteMeToSaveCurrentMatches.txt");
            if (!LockFileExists()) CreateLockFile();
        }

        private bool LockFileExists()
        {
            return fileIo.FileExists(lockFile);
        }

        private void CreateLockFile()
        {
            fileIo.CreateFile(lockFile).Close();
        }

        public void MatchAdded(List<LeagueMatch> matches)
        {
            if (LockFileExists()) return;
            matchSaver.SaveMatches(matches);
            CreateLockFile();
        }
    }
}