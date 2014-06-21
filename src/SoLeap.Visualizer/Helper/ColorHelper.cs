using System.Windows.Media;

namespace SoLeap.Visualizer
{
    public static class ColorHelper
    {
        public static int ToInt(this Color c)
        {
            return c.R << 24 | c.G << 16 | c.B << 8 | c.A;
        }
    }
}
