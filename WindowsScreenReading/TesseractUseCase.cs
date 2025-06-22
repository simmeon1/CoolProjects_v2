using Tesseract;

namespace WindowsScreenReading;

public class TesseractUseCase
{
    private readonly BitmapWorker bw = new();
    private readonly TesseractEngine engine = new(@"C:\D\Apps\Vigem\tessdata", "eng", EngineMode.Default);
    
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