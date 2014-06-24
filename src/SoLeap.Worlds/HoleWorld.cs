using System;
using System.Windows.Media;
using BulletSharp;

namespace SoLeap.Worlds
{
    public class HoleWorld
        : AbstractWorld
    {
        private const float FloorHeight = 100.0f;
        private static readonly Vector3 FloorBoxDimensions = new Vector3(300.0f, 10.0f, 200.0f);
        private const float CubeSize = 20.0f;
        private const int NumberOfCubes = 10;

        public HoleWorld()
            : base("Hole", new Vector3(0.0f, -500.0f, 0.0f))
        {

        }

        protected override void SetupScene()
        {
            // ground layout contains of 4 3x2 boxes
            // 1  1  1  2  2
            // 1  1  1  2  2
            // 4  4     2  2
            // 4  4  3  3  3
            // 4  4  3  3  3
            var floorBoxShape = new BoxShape(FloorBoxDimensions / 2.0f);
            const float pi_2 = (float)(Math.PI / 2.0);
            var floorTransformations = new Matrix[4] {
                Matrix.RotationY(0.0f) * Matrix.Translation(-100.0f, 0.0f, -150.0f), // 1
                Matrix.RotationY(pi_2) * Matrix.Translation(+150.0f, 0.0f, -100.0f), // 2
                Matrix.RotationY(0.0f) * Matrix.Translation(+100.0f, 0.0f, +150.0f), // 3
                Matrix.RotationY(pi_2) * Matrix.Translation(-150.0f, 0.0f, +100.0f)  // 4
            };

            foreach (var floorTransformation in floorTransformations) {
                CreateAndAddRigidBodyAndRenderable(0.0f,
                    floorTransformation * Matrix.Translation(0.0f, FloorHeight, 0.0f),
                    floorBoxShape, Colors.LightSteelBlue);
            }

            // create cubes
            var cubeShape = new BoxShape(CubeSize / 2f);

            var random = new Random();

            Func<float> nextCoord = () => (float)(random.NextDouble() - 0.5) * 300f;
            Func<float> nextHeight = () => (float)(random.NextDouble() * 50.0 + 4 * FloorHeight);
            Func<float> nextAngle = () => (float)(random.NextDouble() * Math.PI);

            for (int i = 0; i < NumberOfCubes; i++) {
                CreateAndAddRigidBodyAndRenderable(1.0f,
                    Matrix.RotationYawPitchRoll(nextAngle(), nextAngle(), nextAngle()) *
                    Matrix.Translation(new Vector3(nextCoord(), nextHeight(), nextCoord())),
                    cubeShape, Colors.Red, "cube " + i);
            }

        }
    }
}
