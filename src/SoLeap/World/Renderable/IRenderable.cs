using System.Windows.Media;
using BulletSharp;

namespace SoLeap.World
{
    public interface IRenderable
    {
        bool Visible { get; set; }

        Color Color { get; set; }

        Matrix WorldTransform { get; }
    }
}
