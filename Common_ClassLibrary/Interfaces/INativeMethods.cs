using System.Drawing;

namespace Common_ClassLibrary.Interfaces
{
    public interface INativeMethods
    {
        Color GetColorAt(Point location);
        Point GetCursorPosition();
    }
}