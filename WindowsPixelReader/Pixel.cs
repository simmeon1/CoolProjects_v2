using System.Drawing;

namespace WindowsPixelReader
{
    public class Pixel
    {
        public readonly int X;
        public readonly int Y;
        public readonly Color PixelColor;

        public Pixel(int x, int y, Color pixelColor)
        {
            X = x;
            Y = y;
            PixelColor = pixelColor;
        }

        public override string ToString()
        {
            return $"X - {X}, Y - {Y}, ARGB - {PixelColor.ToString()}, Brightness - {PixelColor.GetBrightness()}";
        }
    }
}