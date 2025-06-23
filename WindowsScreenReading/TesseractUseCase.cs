using Tesseract;

namespace WindowsScreenReading;

public class TesseractUseCase
{
    private readonly BitmapWorker bw = new();
    private readonly TesseractEngine engine;

    public TesseractUseCase()
    {
        engine = new TesseractEngine(@"C:\D\Apps\Vigem_scripts\tessdata", "eng", EngineMode.Default);
        // Prevent output as performance can get affected
        // This might be better fixed by adding borders (empty page + invalid box errors)
        // https://github.com/tesseract-ocr/tessdoc/blob/main/ImproveQuality.md#borders
        // https://groups.google.com/forum/?utm_medium=email&utm_source=footer#!msg/tesseract-ocr/v26a-RYPSOE/2Sppq61GBwAJ
        Console.WriteLine("Disabling tess output for performance");
        engine.SetVariable("debug_file", "NUL");
    }

    public string GetTextFromClient(
        string processName,
        int clientStartX,
        int clientStartY,
        int clientEndX,
        int clientEndY
    ) {
        return bw.ProcessBitmap(
            clientStartX,
            clientStartY,
            clientEndX,
            clientEndY,
            img =>
            {
                // img.Save("tessTest.png", ImageFormat.Png);
                // using (var pix = Pix.LoadFromFile("tessTest.png"))
                using (var pix = PixConverter.ToPix(img))
                {
                    using (var page = engine.Process(pix))
                    {
                        return page.GetText();
                    }
                }
            },
            processName
        );
    }
    
    public bool ClientContainsTextInRect(
        string processName,
        string text,
        int clientStartX,
        int clientStartY,
        int clientEndX,
        int clientEndY
    )
    {
        return GetTextFromClient(
            processName,
            clientStartX,
            clientStartY,
            clientEndX,
            clientEndY
        ).Contains(text);
    }
}