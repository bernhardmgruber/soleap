using BulletSharp;
using System.Windows.Media;

namespace SoLeap.Worlds
{
    public class BowlWorld : AbstractWorld
    {
        private const float BowlDiameter = 150f;
        private const float BowlHeight = 40f;
        private const float BowlThickness = 7f;
        private const float BowlDistance = 100;

        private const float BallRadius = 25f;

        private const float FloorHeight = 100f;

        public BowlWorld()
            : base("Bowl", new Vector3(0f, -500, 0f)) { }

        protected override void SetupScene()
        {
            // create static ground
            var groundShape = new StaticPlaneShape(Vector3.UnitY, FloorHeight);
            Add(groundShape);

            CreateAndAddRigidBodyAndRenderable(0f, Matrix.Identity, groundShape, Colors.LightSlateGray, "Ground");

            // create two bowls
            float innerDiameter2 = (BowlDiameter - BowlThickness) / 2.0f;
            float diameter2 = BowlDiameter / 2.0f;
            float thickness2 = BowlThickness / 2.0f;
            float height2 = BowlHeight / 2.0f;
            float dist2 = BowlDistance / 2.0f + diameter2;

            CompoundShape bowlShape = new CompoundShape();
            bowlShape.AddChildShape(Matrix.Translation(-innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(+innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, -innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, +innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, -(BowlHeight + BowlThickness) / 2.0f, 0), new BoxShape(diameter2, thickness2, diameter2));
            Add(bowlShape);

            CreateAndAddRigidBodyAndRenderable(30.0f, Matrix.Translation(-dist2, BowlHeight + BowlThickness + FloorHeight, 0), bowlShape, Colors.DarkRed, "Left bowl");
            CreateAndAddRigidBodyAndRenderable(30.0f, Matrix.Translation(+dist2, BowlHeight + BowlThickness + FloorHeight, 0), bowlShape, Colors.DarkRed, "Right bowl");

            // create the ball
            SphereShape ballShape = new SphereShape(BallRadius);
            Add(ballShape);

            CreateAndAddRigidBodyAndRenderable(10.0f, Matrix.Translation(-BowlDiameter, BowlHeight * 2.0f + FloorHeight, 0), ballShape, Colors.LightGreen, "Ball");
        }
    }
}