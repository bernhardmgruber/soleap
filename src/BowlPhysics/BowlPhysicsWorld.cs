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
        public BowlPhysicsWorld()
            : base(new Vector3(0, -50, 0)) { }
        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(10, 0.5f, 10);
            CollisionShapes.Add(groundShape);

            var ground = CreateRigidBody(0, Matrix.Identity, groundShape);
            ground.UserObject = "Ground";
            World.AddRigidBody(ground);

            // create two bowls
            const float diameter = 3.0f;
            const float height = 1.2f;
            const float thickness = 0.2f;

            float innerDiameter2 = (diameter - thickness) / 2.0f;
            float diameter2 = diameter / 2.0f;
            float thickness2 = thickness / 2.0f;
            float height2 = height / 2.0f;

            for (int i = 0; i < 2; i++)
            {
                CompoundShape bowlShape = new CompoundShape();
                bowlShape.AddChildShape(Matrix.Translation(-innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
                bowlShape.AddChildShape(Matrix.Translation(+innerDiameter2, 0, 0), new BoxShape(thickness2, height2, diameter2));
                bowlShape.AddChildShape(Matrix.Translation(0, 0, -innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
                bowlShape.AddChildShape(Matrix.Translation(0, 0, +innerDiameter2), new BoxShape(diameter2 - 2 * thickness2, height2, thickness2));
                bowlShape.AddChildShape(Matrix.Translation(0, -(height + thickness) / 2.0f, 0), new BoxShape(diameter2, thickness2, diameter2));
                CollisionShapes.Add(bowlShape);

                var bowl = CreateRigidBody(2.0f, Matrix.Translation(-5 + i * 10, 2, 0), bowlShape);
                bowl.UserObject = "Bowl " + i;
                World.AddRigidBody(bowl);
            }

            // create the ball
            SphereShape ballShape = new SphereShape(0.5f);
            CollisionShapes.Add(ballShape);

            var ballBody = CreateRigidBody(1.0f, Matrix.Translation(-5, 5, 0), ballShape);
            ballBody.UserObject = "Ball";
            World.AddRigidBody(ballBody);

        }
    }
}
