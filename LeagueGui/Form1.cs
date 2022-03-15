using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;

namespace LeagueGui
{
    public partial class Form1 : Form, ILogger
    {
        private int reminderInterval;
        private readonly Parameters parameters;
        private List<LeagueMatch> matches;
        private SpectatorDataUseCase useCase;
        private LeagueAPIClient leagueClient;
        private bool playReminders;
        private DateTime lastReminder = DateTime.Now;

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
            listBox1.Items.Add(message);
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

        private static async Task<T> DeserializeJsonFile<T>(string parametersPath)
        {
            string text = await File.ReadAllTextAsync(parametersPath);
            return await Task.Run(() => text.DeserializeObject<T>());
        }
        
        private void SetButtons(bool enabled)
        {
            foreach (Button button in new List<Button>() { reminderButton, clearLogButton, damageButton })
            {
                button.Enabled = enabled;
            }
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void reminderButton_Click(object sender, EventArgs e)
        {
            playReminders = !playReminders;
            string action = playReminders ? "Disable" : "Enable";
            Log($"Reminders {action.ToLower()}d.");
            if (playReminders) Log($"Playing reminders every {reminderInterval} seconds.");
            reminderButton.Text = action + " Reminders";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (!playReminders || (now - lastReminder).Seconds < reminderInterval) return;
            lastReminder = now;
            SoundPlayer player = new(@"c:\Windows\Media\chimes.wav");
            player.Play();
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            SetButtons(false);
            reminderInterval = parameters.ReminderInterval;

            Log("Creating objects...");
            leagueClient = new LeagueAPIClient(
                new RealHttpClient(),
                parameters.Token,
                new RealDelayer(),
                this
            );
            
            Log("Reading matches file...");
            matches = await DeserializeJsonFile<List<LeagueMatch>>(parameters.MatchesPath);
            
            useCase = new SpectatorDataUseCase(matches);
            Log("Loaded!");
            SetButtons(true);
            timer1.Start();
        }
    }
}