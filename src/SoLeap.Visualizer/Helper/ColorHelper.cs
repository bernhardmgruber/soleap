using SharpDX;
using Color = System.Windows.Media.Color;

namespace SoLeap.Visualizer
{
    public static class ColorHelper
    {
        public static int ToInt(this Color c)
        {
            return c.R << 24 | c.G << 16 | c.B << 8 | c.A;
        }

        public static Vector3 ToVector3(this Color c)
        {
            return new Vector3(c.R / 255.0f, c.G / 255.0f, c.B / 255.0f);
        }
    }
}
