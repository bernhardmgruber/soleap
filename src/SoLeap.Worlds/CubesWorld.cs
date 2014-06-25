using System.Windows.Media;
using BulletSharp;
using System;
using SoLeap.World;

namespace SoLeap.Worlds
{
    public class CubesWorld : AbstractWorld
    {
        private const float CubeSize = 30.0f;
        private const int NumberOfCubes = 40;

        private const float FloorHeight = 100f;

        public CubesWorld()
            : base("Cubes", new Vector3(0, -500, 0))
        { }

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            var body = CreateAndAddRigidBodyAndRenderable(0.0f, Matrix.Identity, groundShape, Colors.LightSlateGray, "ground");

            // create cubes
            var cubeShape = new BoxShape(CubeSize / 2f);

            var random = new Random();

            Func<float> nextCoord = () => (float)(random.NextDouble() - 0.5) * 300f;
            Func<float> nextHeight = () => (float)(random.NextDouble() * 50.0 + 4 * FloorHeight);
            Func<float> nextAngle = () => (float)(random.NextDouble() * Math.PI);

            for (int i = 0; i < NumberOfCubes; i++) {
                CreateAndAddRigidBodyAndRenderable(1f,
                    Matrix.RotationYawPitchRoll(nextAngle(), nextAngle(), nextAngle()) *
                    Matrix.Translation(new Vector3(nextCoord(), nextHeight(), nextCoord())), cubeShape, Colors.Red, "cube " + i);
            }
        }
    }
}