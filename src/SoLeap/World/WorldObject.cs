using System.Diagnostics.Contracts;
using System.Windows.Media;
using BulletSharp;

namespace SoLeap.World
{
    public class WorldObject
        : IRenderable
    {
        public bool Visible { get; set; }

        public Color Color { get; set; }

        public CollisionObject CollisionObject { get; private set; }

        public Matrix WorldTransform
        {
            get { return CollisionObject.WorldTransform; }
        }

        public WorldObject(CollisionObject collisionObject, Color color)
        {
            Contract.Requires(collisionObject != null);

            CollisionObject = collisionObject;
            Visible = true;
            Color = color;
        }
    }
}
