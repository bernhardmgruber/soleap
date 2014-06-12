using System.Diagnostics.Contracts;
using System.Windows.Media;
using BulletSharp;

namespace SoLeap.World
{
    public class RigidBodyRenderable
        : IRenderable
    {
        private MotionState motionState;

        public bool Visible { get; set; }

        public Color Color { get; set; }

        public CollisionShape CollisionShape { get; private set; }

        public Matrix WorldTransform
        {
            get { return motionState.WorldTransform; }
        }

        public RigidBodyRenderable(MotionState motionState, CollisionShape collisionShape, Color color)
        {
            Contract.Requires(motionState != null);
            Contract.Requires(collisionShape != null);

            this.motionState = motionState;
            this.CollisionShape = collisionShape;
            Visible = true;
            Color = color;
        }
    }
}
