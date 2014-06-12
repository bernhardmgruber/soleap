using BulletSharp;

namespace SoLeap.Worlds
{
    public class BowlWorld : AbstractWorld
    {
        private const float BowlDiameter = 150f;
        private const float BowlHeight = 40f;
        private const float BowlThickness = 7f;

        private const float BallRadius = 25f;

        private const float SceneHeight = 100f;

        public BowlWorld()
            : base("Bowl", new Vector3(0f, -500, 0f)) { }

        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(1000f, 10f, 1000f);
            Add(groundShape);

            CreateAndAddRigidBody(0f, Matrix.Translation(0, SceneHeight, 0), groundShape, "Ground");

            // create two bowls
            float innerDiameter2 = (BowlDiameter - BowlThickness) / 2.0f;
            float diameter2 = BowlDiameter / 2.0f;
            float thickness2 = BowlThickness / 2.0f;
            float height2 = BowlHeight / 2.0f;

            CompoundShape bowlShape = new CompoundShape();
            bowlShape.AddChildShape(Matrix.Translation(-innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(+innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, -innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, +innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, -(BowlHeight + BowlThickness) / 2.0f, 0), new BoxShape(diameter2, thickness2, diameter2));
            Add(bowlShape);

            CreateAndAddRigidBody(30.0f, Matrix.Translation(-BowlDiameter, BowlHeight + BowlThickness + SceneHeight, 0), bowlShape, "Left bowl");
            CreateAndAddRigidBody(30.0f, Matrix.Translation(+BowlDiameter, BowlHeight + BowlThickness + SceneHeight, 0), bowlShape, "Right bowl");

            // create the ball
            SphereShape ballShape = new SphereShape(BallRadius);
            Add(ballShape);

            CreateAndAddRigidBody(10.0f, Matrix.Translation(-BowlDiameter, BowlHeight * 2.0f + SceneHeight, 0), ballShape, "Ball");
        }
    }
}