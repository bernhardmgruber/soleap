using BulletSharp;

namespace SoLeap.Worlds
{
    internal class SimpleWallWorld : AbstractWorld
    {
        private const float SceneHeight = 100f;

        private Vector3 wallDimensions2 = new Vector3(20f, 200f, 200f);

        public SimpleWallWorld()
            : base("Simple Wall", new Vector3(0f, -500f, 0f))
        { }

        public override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(1000f, 10f, 1000f);
            Add(groundShape);

            CreateAndAddRigidBody(0f, Matrix.Translation(0, SceneHeight, 0), groundShape, "Ground");

            // create walls
            BoxShape wallShape = new BoxShape(wallDimensions2);
            Add(wallShape);

            CreateAndAddRigidBody(0f, Matrix.Translation(-200f, SceneHeight + wallDimensions2.Y, 0), wallShape, "static wall");
            CreateAndAddRigidBody(10f, Matrix.Translation(+200f, SceneHeight + wallDimensions2.Y, 0), wallShape, "dynamic wall");
        }
    }
}