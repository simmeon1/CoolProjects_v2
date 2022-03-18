using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;

namespace LeagueGui
{
    public partial class Form1 : Form, ILogger
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(
            IntPtr hDC,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hSrcDC,
            int xSrc,
            int ySrc,
            int dwRop
        );

        private readonly Parameters parameters;
        private List<LeagueMatch> matches;
        private SpectatorDataUseCase useCase;
        private LeagueAPIClient leagueClient;
        private bool playReminders;
        private DateTime lastReminder = DateTime.Now;
        private readonly Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);

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
            foreach (Button button in new List<Button> {reminderButton, clearLogButton, damageButton})
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
            string enableText = "Enable";
            string disableText = "Disable";
            string action = playReminders ? enableText : disableText;
            Log($"Reminders {action.ToLower()}d.");
            reminderButton.Text = (playReminders ? disableText : enableText).Trim() + " Reminders";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            if (!playReminders || (now - lastReminder).Seconds < 2) return;
            lastReminder = now;

            bool hasD = SpellAtLocationIsAvailable(987, 1019);
            bool hasF = SpellAtLocationIsAvailable(1022, 1023);

            string file = "";
            if (hasD && !hasF) file = "D";
            else if (!hasD && hasF) file = "F";
            else if (hasD && hasF) file = "D+F";

            if (!file.IsNullOrEmpty()) new SoundPlayer(Path.Combine(parameters.WavLocation, $"{file}.wav")).Play();
        }

        private bool SpellAtLocationIsAvailable(int x, int y)
        {
            return GetColorAt(new Point(x, y)).GetBrightness() == (float)0.72156864;
        }

        private Color GetColorAt(Point location)
        {
            using Graphics gdest = Graphics.FromImage(screenPixel);
            using Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr hSrcDC = gsrc.GetHdc();
            IntPtr hDC = gdest.GetHdc();
            int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int) CopyPixelOperation.SourceCopy);
            gdest.ReleaseHdc();
            gsrc.ReleaseHdc();
            return screenPixel.GetPixel(0, 0);
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            SetButtons(false);

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