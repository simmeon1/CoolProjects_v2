using Tesseract;

namespace WindowsScreenReading;

public class TesseractUseCase
{
    private readonly BitmapWorker bw = new();
    private readonly TesseractEngine engine;

    public TesseractUseCase()
    {
        engine = new TesseractEngine(@"C:\D\Apps\Vigem_scripts\tessdata", "eng", EngineMode.Default);
        // Use appropriate page segmentation mode to properly parse text
        // Default mode is (3 Auto) regularly results in invalid box/coordinates errors which affects performance
        // through regular logging.
        // Default is set to 7 Single line as that are the most expected use cases.
        // The mode can also be provided to the Process method
        // https://pyimagesearch.com/2021/11/15/tesseract-page-segmentation-modes-psms-explained-how-to-improve-your-ocr-accuracy/
        engine.DefaultPageSegMode = PageSegMode.SingleLine;
        Console.WriteLine($"Using PSM (page segmentation mode) {engine.DefaultPageSegMode}");
        
        // Prevent debug output as performance can get affected
        // Includes metadata/total count=0 messages when using single line psm
        engine.SetVariable("debug_file", "NUL");
        Console.WriteLine("Disabled tess debug output for performance benefits");
        
        // PSM setting fixed empty page and invalid box errors.
        // If those start appearing, adding borders to the images might be a good solution.
        // This might be better fixed by adding borders (empty page + invalid box errors)
        // https://github.com/tesseract-ocr/tessdoc/blob/main/ImproveQuality.md#borders
        // https://github.com/tesseract-ocr/tesseract/issues/427
        // https://groups.google.com/forum/?utm_medium=email&utm_source=footer#!msg/tesseract-ocr/v26a-RYPSOE/2Sppq61GBwAJ
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