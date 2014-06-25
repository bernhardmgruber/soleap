using System.Windows.Media;
using BulletSharp;
using System;
using SoLeap.World;

namespace SoLeap.Worlds
{
    public class LeverWorld : AbstractWorld
    {
        private const float LeverHeadWidth = 80;
        private const float LeverHeadThickness = 20;
        private const float LeverLength = 160;
        private const float LeverThickness = 10;
        private const float FloorHeight = 100f;

        public LeverWorld()
            : base("Levers", new Vector3(0, -500, 0))
        { }

        protected override void SetupScene()
        {
            // ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);
            CreateAndAddRigidBodyAndRenderable(0.0f, Matrix.Identity, groundShape, Colors.LightSlateGray, "ground");

            const float headWidth2 = LeverHeadWidth / 2.0f;
            const float headThickness2 = LeverHeadThickness / 2.0f;
            const float leverThickness2 = LeverThickness / 2.0f;
            const float leverLength2 = LeverLength / 2.0f;

            // create levers
            var leverStick = new BoxShape(leverThickness2, leverLength2, leverThickness2);
            var leverHead = new BoxShape(headWidth2, headThickness2, headThickness2);

            var lever = new CompoundShape();
            lever.AddChildShape(Matrix.Identity, leverStick);
            lever.AddChildShape(Matrix.Translation(0, leverLength2, 0), leverHead);

            for (var i = 0; i < 3; i++)
            {
                var body = CreateAndAddRigidBodyAndRenderable(1, Matrix.Translation(-LeverHeadWidth + 5 + (LeverHeadWidth + 5) * i, FloorHeight + leverLength2 + leverThickness2, 0), lever, Colors.Red, "lever");

                var constraint = new HingeConstraint(body, new Vector3(0, -leverLength2, 0), Vector3.UnitX);

                float limit = (float)Math.PI / 5.0f;
                constraint.SetLimit(-limit, +limit);
                Add(constraint);
            }
        }
    }
}