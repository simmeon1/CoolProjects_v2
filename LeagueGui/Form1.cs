using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;

namespace LeagueGui
{
    public partial class Form1 : Form, ILogger
    {
        private readonly Parameters parameters;
        private LeagueAPIClient leagueClient;
        private bool soundsPlaying;
        private List<CheckBox> reminderButtons;
        private readonly WindowsNativeMethods windowsNativeMethods = new();
        private readonly SpectatorDataUseCase useCase = new(new List<LeagueMatch>());

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(Parameters parameters)
        {
            InitializeComponent();
            this.parameters = parameters;
        }

        public void Log(string message)
        {
            log.Items.Add(message);
        }

        public bool Contains(string message)
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            throw new NotImplementedException();
        }

        private async void damageButton_Click(object sender, EventArgs e)
        {
            Log("Retrieving spectator data...");
            string encryptedSummonerId = parameters.Id;
            SpectatorData spectatorData = await leagueClient.GetSpectatorDataByEncryptedSummonerId(encryptedSummonerId);
            Log(
                spectatorData == null
                    ? "User is not in game."
                    : useCase.GetDamagePlayerIsPlayingAgainst(spectatorData, encryptedSummonerId)
            );
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            log.Items.Clear();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (soundsPlaying || reminderButtons.All(b => !b.Checked)) return;
            
            soundsPlaying = true;

            await Task.Run(
                () =>
                {
                    foreach (CheckBox reminderButton in reminderButtons)
                    {
                        if (!reminderButton.Checked) continue;
                        
                        string tag = (string)reminderButton.Tag;
                        string[] coordinates = tag.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        int x = int.Parse(coordinates[0]);
                        int y = int.Parse(coordinates[1]);
                        float expectedBrightness = coordinates.Length == 2 ? (float) 0.72156864 : float.Parse(coordinates[2]);
                        if (!SpellAtLocationIsAvailable(x, y, expectedBrightness)) continue;
                        
                        char letter = reminderButton.Name.Last();
                        new SoundPlayer(Path.Combine(parameters.WavLocation, $"{letter}.wav")).PlaySync();
                    }
                }).ConfigureAwait(false);
            
            soundsPlaying = false;
        }

        private bool SpellAtLocationIsAvailable(int x, int y, float brightnessValue)
        {
            return windowsNativeMethods.GetColorAtLocation(new Point(x, y)).GetBrightness() == brightnessValue;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            // while (true)
            // {
            //     Point cursor = new();
            //     GetCursorPos(ref cursor);
            //     Color c = GetColorAt(cursor);
            //     Debug.WriteLine($"{cursor.X},{cursor.Y},{c.GetBrightness()}");
            //     await Task.Delay(1000);
            // }
            
            reminderButtons = new List<CheckBox> {buttonR, buttonD, buttonF, button1, button2, button3};
            damageButton.Enabled = false;
            Log("Creating objects...");
            leagueClient = new LeagueAPIClient(
                new RealHttpClient(),
                parameters.Token,
                new RealDelayer(),
                this
            );

            Log("Reading matches file...");
            
            using (StreamReader sr = new(parameters.MatchesPath))
            {
                string line = await sr.ReadLineAsync();
                while (!line.IsNullOrEmpty())
                {
                    LeagueMatch match = line.DeserializeObject<LeagueMatch>();
                    useCase.AddMatchData(match);
                    line = await sr.ReadLineAsync();
                }
            }
            Log("Loaded!");
            damageButton.Enabled = true;
            timer1.Start();
        }

        private void reminderToggleButton_Click(object sender, EventArgs e)
        {
            bool setting = reminderButtons.Any(b => b.Checked);
            foreach (CheckBox checkBox in reminderButtons)
            {
                checkBox.Checked = !setting;
            }
        }
    }
}