using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using System.Diagnostics;

namespace BowlPhysics
{
    public class BowlPhysicsWorld : PhysicsWorld
    {
        const float bowlDiameter = 150f;
        const float bowlHeight = 40f;
        const float bowlThickness = 7f;

        const float ballRadius = 25f;

        const float sceneHeight = 100f;

        const float gravity = 500f;

        public BowlPhysicsWorld()
            : base(new Vector3(0f, -gravity, 0f)) { }
        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(1000f, 10f, 1000f);
            CollisionShapes.Add(groundShape);

            CreateRigidBody(0f, Matrix.Translation(0, sceneHeight, 0), groundShape, "Ground");
   
            // create two bowls
            float innerDiameter2 = (bowlDiameter - bowlThickness) / 2.0f;
            float diameter2 = bowlDiameter / 2.0f;
            float thickness2 = bowlThickness / 2.0f;
            float height2 = bowlHeight / 2.0f;

            CompoundShape bowlShape = new CompoundShape();
            bowlShape.AddChildShape(Matrix.Translation(-innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(+innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, -innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, 0, +innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
            bowlShape.AddChildShape(Matrix.Translation(0, -(bowlHeight + bowlThickness) / 2.0f, 0), new BoxShape(diameter2, thickness2, diameter2));
            CollisionShapes.Add(bowlShape);

            CreateRigidBody(30.0f, Matrix.Translation(-bowlDiameter, bowlHeight + bowlThickness + sceneHeight, 0), bowlShape, "Left bowl");
            CreateRigidBody(30.0f, Matrix.Translation(+bowlDiameter, bowlHeight + bowlThickness + sceneHeight, 0), bowlShape, "Right bowl");

            // create the ball
            SphereShape ballShape = new SphereShape(ballRadius);
            CollisionShapes.Add(ballShape);

            CreateRigidBody(10.0f, Matrix.Translation(-bowlDiameter, bowlHeight * 2.0f + sceneHeight, 0), ballShape, "Ball");
        }
    }
}
