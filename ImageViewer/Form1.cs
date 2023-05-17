namespace ImageViewer;

public partial class Form1 : Form
{
    private string directory;
    private int currentIndex;
    private int maxIndex;

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
        maxIndex = Directory.GetFiles(directory).Length;
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
        if (index <= 0 || index > maxIndex) return;

        string[] files =  Directory.GetFiles(directory);
        string file = files.First(f => Path.GetFileNameWithoutExtension(f) == index.ToString());
        string fileName = Path.GetFileName(file);
        Image image = Image.FromFile(file);
        ClientSize = new Size(image.Width, image.Height);
        Text = fileName;
        if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
        pictureBox1.Image = image;
        currentIndex = index;
    }
}