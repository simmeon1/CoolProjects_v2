using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;

namespace LeagueGui
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger_Console logger = new();
            logger.Log("Reading parameters file...");
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = DeserializeJsonFile<Parameters>(parametersPath);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(parameters));
        }
        
        private static T DeserializeJsonFile<T>(string parametersPath)
        {
            return File.ReadAllText(parametersPath).DeserializeObject<T>();
        }
    }
    
    public class Parameters
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string MatchesPath { get; set; }
        public string WavLocation { get; set; }
    }
}
