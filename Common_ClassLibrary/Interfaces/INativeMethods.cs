using System.Drawing;

namespace Common_ClassLibrary.Interfaces
{
    public interface INativeMethods
    {
        Color GetColorAtCursor();
        Color GetColorAtLocation(Point location);
        Point GetCursorLocation();
    }
}