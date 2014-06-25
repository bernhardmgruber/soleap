using System.Windows.Media;
using BulletSharp;
using System;
using SoLeap.World;

namespace SoLeap.Worlds
{
    public class JengaWorld : AbstractWorld
    {
        private const float BrickLength = 100.0f;
        private const float BrickHeight = 20.0f;
        private const int TowerHeight = 2;

        private const float FloorHeight = 100f;

        public JengaWorld()
            : base("Jenga", new Vector3(0, -500, 0))
        { }

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            var body = CreateAndAddRigidBodyAndRenderable(0.0f, Matrix.Identity, groundShape, Colors.LightSlateGray, "ground");

            // tower
            const float brickLength2 = BrickLength / 2.0f;
            const float brickLength3 = BrickLength / 3.0f;
            const float brickHeight2 = BrickHeight / 2.0f;

            var brick = new BoxShape(brickLength2, brickHeight2, brickLength2 / 3.0f);

            for (int i = 0; i < TowerHeight; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var transform = Matrix.Translation(0, FloorHeight + i * BrickHeight, -brickLength3 + brickLength3 * j);
                    if ((i & 1) == 0)
                        transform = Matrix.Multiply(transform, Matrix.RotationY((float)Math.PI / 2.0f));
                    CreateAndAddRigidBodyAndRenderable(1.0f, transform, brick, Colors.BurlyWood, "brick " + i + " " + j);
                }
            }
        }
    }
}