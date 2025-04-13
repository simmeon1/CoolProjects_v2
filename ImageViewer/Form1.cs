namespace ImageViewer;

public partial class Form1 : Form
{
    private string directory;
    private int currentIndex;
    private int minIndex;
    private int maxIndex;
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
        int GetIndexFromFileName(string file) =>
            int.Parse(Path.GetFileNameWithoutExtension(file).Split("-", StringSplitOptions.RemoveEmptyEntries)[0]);
        currentIndex = GetIndexFromFileName(imageFile);
        directory = Path.GetDirectoryName(imageFile);
        allFiles = Directory.GetFiles(directory).OrderBy(GetIndexFromFileName).ToArray();
        minIndex = GetIndexFromFileName(allFiles.First());
        maxIndex = GetIndexFromFileName(allFiles.Last());
        LoadImageFromIndex(currentIndex);
    }
    
    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        Keys key = e.KeyCode;
        if (key == Keys.F)
        {
            FormBorderStyle = FormBorderStyle == FormBorderStyle.None ? FormBorderStyle.Sizable : FormBorderStyle.None;
        }
        
        if (key == Keys.Right) LoadImageFromIndex(currentIndex + 1);
        if (key == Keys.Left) LoadImageFromIndex(currentIndex - 1);
        if (key == Keys.O) PopulateDialog();
    }
    
    private void LoadImageFromIndex(int index)
    {
        if (index < minIndex || index > maxIndex) return;

        string file = allFiles.First(f => Path.GetFileNameWithoutExtension(f).StartsWith($"{index}-"));
        string fileName = Path.GetFileName(file);
        Image image = Image.FromFile(file);
        ClientSize = new Size(image.Width, image.Height);
        Text = fileName;
        if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
        pictureBox1.Image = image;
        currentIndex = index;
    }
}