namespace ImageViewer;

public partial class Form1 : Form
{
    private string directory;
    private int currentIndex;
    private int maxIndex;
    private readonly Dictionary<int, string> mappedData = new();
    
    public Form1()
    {
        InitializeComponent();
        PopulateDialog();
    }

    private void PopulateDialog()
    {
        OpenFileDialog dialog = new();
        if (dialog.ShowDialog() != DialogResult.OK) return;

        string imageFile = dialog.FileName;
        directory = Path.GetDirectoryName(imageFile);
        currentIndex = int.Parse(Path.GetFileNameWithoutExtension(imageFile));
        
        List<string> data = File.ReadAllText($"{directory}\\data.txt").Trim().Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries
        ).ToList();
        maxIndex = data.Count;

        mappedData.Clear();
        foreach (string s in data)
        {
            int id = int.Parse(GetValueFromData(s, "id"));
            mappedData.Add(id, s);
        }

        string rowData = mappedData[currentIndex];
        int x = int.Parse(GetValueFromData(rowData, "x"));
        int y = int.Parse(GetValueFromData(rowData, "y"));
        StartPosition = FormStartPosition.Manual;
        Location = new Point(x, y);

        LoadImageFromIndex(currentIndex);
        Image image = pictureBox1.Image;
        ClientSize = new Size(image.Width, image.Height);
    }

    private string GetValueFromData(string s, string prop)
    {
        string[] properties = s.Split(";", StringSplitOptions.RemoveEmptyEntries);
        foreach (string property in properties)
        {
            string[] propAndValue = property.Split("=", StringSplitOptions.RemoveEmptyEntries);
            if (propAndValue[0] == prop)
            {
                return propAndValue[1];
            }
        }
        return "";
    }

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F)
        {
            FormBorderStyle = FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.Sizable : FormBorderStyle.None;
        }
        
        if (e.KeyCode == Keys.Right) LoadImageFromIndex(currentIndex + 1);
        if (e.KeyCode == Keys.Left) LoadImageFromIndex(currentIndex - 1);
        if (e.KeyCode == Keys.O) PopulateDialog();
    }
    
    private void LoadImageFromIndex(int index)
    {
        if (index <= 0 || index > maxIndex) return;

        string[] files =  Directory.GetFiles(directory);
        string file = files.First(f => Path.GetFileNameWithoutExtension(f) == index.ToString());
        Image image = Image.FromFile(file);
        pictureBox1.Image = image;
        currentIndex = index;
    }
}