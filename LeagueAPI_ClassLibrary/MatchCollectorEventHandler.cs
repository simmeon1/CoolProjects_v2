using System.Collections.Generic;
using System.IO;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class MatchCollectorEventHandler : IMatchCollectorEventHandler
    {
        private readonly IFileIO fileIo;
        private readonly IMatchSaver matchSaver;
        private readonly string lockFile;

        public MatchCollectorEventHandler(IFileIO fileIo, IMatchSaver matchSaver, string outputDirectory)
        {
            this.fileIo = fileIo;
            this.matchSaver = matchSaver;
            lockFile = Path.Combine(outputDirectory, "deleteMeToSaveCurrentMatches.txt");
        }

        public void CollectingStarted()
        {
            if (!LockFileExists()) CreateLockFile();
        }

        public void MatchAdded(List<LeagueMatch> matches)
        {
            if (LockFileExists()) return;
            matchSaver.SaveMatches(matches);
            CreateLockFile();
        }

        public void CollectingFinished()
        {
            if (LockFileExists()) fileIo.DeleteFile(lockFile);
        }

        private bool LockFileExists()
        {
            return fileIo.FileExists(lockFile);
        }

        private void CreateLockFile()
        {
            fileIo.CreateFile(lockFile).Close();
        }
    }
}