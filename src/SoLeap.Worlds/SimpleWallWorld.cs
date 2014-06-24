using BulletSharp;
using System.Windows.Media;

namespace SoLeap.Worlds
{
    public class SimpleWallWorld : AbstractWorld
    {
        private const float SceneHeight = 100f;

        private Vector3 wallDimensions2 = new Vector3(20f, 200f, 200f);

        public SimpleWallWorld()
            : base("Simple Wall", new Vector3(0f, -500f, 0f))
        { }

        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(1000f, 10f, 1000f);
            Add(groundShape);

            CreateAndAddRigidBodyAndRenderable(0f, Matrix.Translation(0, SceneHeight, 0), groundShape, Colors.LightSlateGray, "Ground");

            // create walls
            BoxShape wallShape = new BoxShape(wallDimensions2);
            Add(wallShape);

            CreateAndAddRigidBodyAndRenderable(0f, Matrix.Translation(-200f, SceneHeight + wallDimensions2.Y, 0), wallShape, Colors.Orange, "static wall");
            CreateAndAddRigidBodyAndRenderable(10f, Matrix.Translation(+200f, SceneHeight + wallDimensions2.Y, 0), wallShape, Colors.Green, "dynamic wall");
        }
    }
}