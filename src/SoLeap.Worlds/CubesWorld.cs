using BulletSharp;
using System;

namespace SoLeap.Worlds
{
    public class CubesWorld : AbstractPhysicsWorld
    {
        public CubesWorld()
            : base(new Vector3(0, -500, 0)) { }

        private const float cubeSize = 40f;
        private const int numberOfCubes = 5;

        private const float floorHeight = 100f;

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, floorHeight);
            Add(groundShape);
            CreateAndAddRigidBody(0.0f, Matrix.Identity, groundShape, "ground");

            // create cubes
            var cubeShape = new BoxShape(cubeSize / 2f);

            var random = new Random();

            Func<float> nextCoord = () => (float)(random.NextDouble() - 0.5) * 300f;

            for (int i = 0; i < numberOfCubes; i++)
                CreateAndAddRigidBody(1f, Matrix.Translation(new Vector3(nextCoord(), 2 * floorHeight, nextCoord())), cubeShape, "cube " + i);
        }
    }
}