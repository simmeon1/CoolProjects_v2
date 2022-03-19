using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        private SpectatorDataUseCase useCase;
        private LeagueAPIClient leagueClient;
        private readonly Bitmap screenPixel = new(1, 1, PixelFormat.Format32bppArgb);
        private bool soundsPlaying;

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
            // return await Task.Run(() => text.DeserializeObject<T>());
            return text.DeserializeObject<T>();
        }

        private void SetButtons(bool enabled)
        {
            foreach (Control control in new List<Control> {damageButton})
            {
                control.Enabled = enabled;
            }
        }

        private void clearLog_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (soundsPlaying) return;
            
            soundsPlaying = true;
            List<CheckBox> reminderButtons = GetReminderButtons();
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
                await Task.Run(() => new SoundPlayer(Path.Combine(parameters.WavLocation, $"{letter}.wav")).PlaySync());
            }
            soundsPlaying = false;
        }

        private List<CheckBox> GetReminderButtons()
        {
            return new List<CheckBox> {buttonR, buttonD, buttonF, button1, button2, button3};
        }

        private bool SpellAtLocationIsAvailable(int x, int y, float brightnessValue)
        {
            return GetColorAt(new Point(x, y)).GetBrightness() == brightnessValue;
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
            // while (true)
            // {
            //     Point cursor = new();
            //     GetCursorPos(ref cursor);
            //     Color c = GetColorAt(cursor);
            //     Debug.WriteLine($"{cursor.X},{cursor.Y},{c.GetBrightness()}");
            //     await Task.Delay(1000);
            // }
            
            timer1.Start();
            SetButtons(false);

            Log("Creating objects...");
            leagueClient = new LeagueAPIClient(
                new RealHttpClient(),
                parameters.Token,
                new RealDelayer(),
                this
            );

            Log("Reading matches file...");
            
            useCase = new SpectatorDataUseCase(await DeserializeJsonFile<List<LeagueMatch>>(parameters.MatchesPath));
            Log("Loaded!");
            SetButtons(true);
        }

        private void reminderToggleButton_Click(object sender, EventArgs e)
        {
            List<CheckBox> boxes = GetReminderButtons();
            bool setting = boxes.Any(b => b.Checked);
            foreach (CheckBox checkBox in boxes)
            {
                checkBox.Checked = !setting;
            }
        }
    }
}