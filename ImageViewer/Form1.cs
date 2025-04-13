namespace ImageViewer;

public partial class Form1 : Form
{
    private int currentIndex;
    private string[] allFiles;

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
        currentIndex = GetIndexFromFileName(imageFile);
        allFiles = Directory.GetFiles(Path.GetDirectoryName(imageFile)).OrderBy(GetIndexFromFileName).ToArray();
        LoadImageFromIndex(currentIndex);
    }

    private int GetIndexFromFileName(string file) =>
        int.Parse(Path.GetFileNameWithoutExtension(file).Split("-", StringSplitOptions.RemoveEmptyEntries)[0]);

    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        Keys key = e.KeyCode;
        if (key == Keys.F)
        {
            FormBorderStyle = FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.Sizable : FormBorderStyle.None;
        }

        var modifier = e.Modifiers switch
        {
            Keys.Shift => 10,
            Keys.Control => 100,
            _ => 1
        };
        if (key == Keys.Right) LoadImageFromIndex(currentIndex + modifier);
        if (key == Keys.Left) LoadImageFromIndex(currentIndex - modifier);
        if (key == Keys.O) PopulateDialog();
    }
    
    private void LoadImageFromIndex(int index)
    {
        var minIndex = GetIndexFromFileName(allFiles.First());
        var maxIndex = GetIndexFromFileName(allFiles.Last());
        if (index < minIndex)
        {
            index = minIndex;
        }
        if (index > maxIndex)
        {
            index = maxIndex;
        }

        string file = allFiles.First(f => GetIndexFromFileName(f) == index);
        string fileName = Path.GetFileName(file);
        Image image = Image.FromFile(file);
        ClientSize = new Size(image.Width, image.Height);
        Text = fileName;
        if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
        pictureBox1.Image = image;
        currentIndex = index;
    }
}