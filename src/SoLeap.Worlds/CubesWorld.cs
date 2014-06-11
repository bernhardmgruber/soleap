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

        public override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            CreateAndAddRigidBody(0.0f, Matrix.Identity, groundShape, "ground");

            // create cubes
            var cubeShape = new BoxShape(CubeSize / 2f);

            var random = new Random();

            Func<float> nextCoord = () => (float)(random.NextDouble() - 0.5) * 300f;

            for (int i = 0; i < NumberOfCubes; i++) {
                var body = CreateAndAddRigidBody(1f, Matrix.Translation(new Vector3(nextCoord(), 2 * FloorHeight, nextCoord())), cubeShape, "cube " + i);

                var renderable = new WorldObject(body, Colors.Red);
                Renderables.Add(renderable);
            }

            base.SetupScene();
        }
    }
}