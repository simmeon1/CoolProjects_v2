namespace ImageViewer;

public partial class Form1 : Form
{
    private readonly string directory = "C:\\D\\Apps\\Vigem\\Recordings\\2023-05-15--11-13-09";
    private int currentIndex;
    private readonly int maxIndex;
    private readonly Dictionary<int, string> mappedData = new();
    
    public Form1()
    {
        InitializeComponent();

        List<string> data = File.ReadAllText($"{directory}\\data.txt").Trim().Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries
        ).ToList();
        maxIndex = data.Count;

        foreach (string s in data)
        {
            int id = int.Parse(GetValueFromData(s, "id"));
            mappedData.Add(id, s);
        }
        
        int index = 1;
        string rowData = mappedData[index];
        int x = int.Parse(GetValueFromData(rowData, "x"));
        int y = int.Parse(GetValueFromData(rowData, "y"));
        StartPosition = FormStartPosition.Manual;
        Location = new Point(x, y);

        LoadImageFromIndex(index);
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
    }
    
    private void LoadImageFromIndex(int index)
    {
        if (index <= 0 || index > maxIndex) return;
        
        Image image = Image.FromFile($"{directory}\\{index}.jpg");
        pictureBox1.Image = image;
        currentIndex = index;
    }
}