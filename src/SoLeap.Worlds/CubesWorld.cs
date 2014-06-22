using System.Windows.Media;
using BulletSharp;
using System;
using SoLeap.World;

namespace SoLeap.Worlds
{
    public class CubesWorld : AbstractWorld
    {
        private const float CubeSize = 40f;
        private const int NumberOfCubes = 5;

        private const float FloorHeight = 100f;

        public CubesWorld()
            : base("Cubes", new Vector3(0, -500, 0))
        { }

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            var body = CreateAndAddRigidBody(0.0f, Matrix.Identity, groundShape, "ground");
            Renderables.Add(new RigidBodyRenderable(body.MotionState, body.CollisionShape, Colors.SlateGray));

            // create cubes
            var cubeShape = new BoxShape(CubeSize / 2f);

            var random = new Random();

            Func<float> nextCoord = () => (float)(random.NextDouble() - 0.5) * 300f;

            for (int i = 0; i < NumberOfCubes; i++) {
                body = CreateAndAddRigidBody(1f, Matrix.Translation(new Vector3(nextCoord(), 2 * FloorHeight, nextCoord())), cubeShape, "cube " + i);

                Renderables.Add(new RigidBodyRenderable(body.MotionState, body.CollisionShape, Colors.Red));
            }
        }
    }
}