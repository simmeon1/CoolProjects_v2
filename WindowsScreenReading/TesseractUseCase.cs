using System.Drawing;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace WindowsScreenReading
{
    public class TesseractUseCase
    {
        // private readonly PixelReader pr;
        private readonly BitmapWorker bw;
        private readonly TesseractEngine engine;

        public TesseractUseCase()
        {
            // this.pr = pr;
            bw = new BitmapWorker();
            engine = new TesseractEngine(@"C:\D\Apps\Vigem\tessdata", "eng", EngineMode.Default);
        }
        
        public bool ClientContainsTextInRect(
            string processName,
            string text,
            int clientStartX,
            int clientStartY,
            int clientEndX,
            int clientEndY
        ) {
            return bw.ProcessBitmap(
                processName,
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
                            var pageText = page.GetText();
                            return pageText.Contains(text);
                        }
                    }
                }
            );
        }
    }
}