using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using System.Diagnostics;

namespace BowlPhysics
{
    public class RubicsPhysicsWorld : PhysicsWorld
    {
        public RubicsPhysicsWorld()
            : base(new Vector3(0, -50, 0)) { }

        const float subCubeSize = 1.0f;
        const float subCubeMass = 1.0f;
        const float bevel = 0.2f;

        const float subCubeGap = 2f;

        private CollisionShape CreateSubCubeShape()
        {
            return new BoxShape(subCubeSize / 2.0f);
        }

        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(10, 0.5f, 10);
            CollisionShapes.Add(groundShape);

            CreateRigidBody(0, Matrix.Translation(0, -3, 0), groundShape, "Ground");

            // create basic shape for a small sub cube
            var shape = CreateSubCubeShape();

            // create the 27 rigid bodies
            var bodies = new RigidBody[3, 3, 3];

            for (int z = 0; z < 3; z++)
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                        if((x == 1 && y == 1) /*|| (x == 1 && z == 1) || (y == 1 && z == 1)*/)
                        bodies[z, y, x] = CreateRigidBody(
                            subCubeMass,
                            Matrix.Translation((x - 1) * (1.0f + subCubeGap), (y - 1) * (1.0f + subCubeGap), (z - 1) * (1.0f + subCubeGap)),
                            shape,
                            String.Format("subcube {0} {1} {2}", x, y, z)
                            );

            // add hinge constraints for center sub cubes
            // see http://bulletphysics.org/mediawiki-1.5.8/index.php/Constraints
            // and https://www.panda3d.org/manual/index.php/Bullet_Constraints
            //World.AddConstraint(new HingeConstraint(bodies[1, 1, 0], bodies[1, 1, 1], Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitX));
            //World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[1, 1, 2], Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitX));
                                                                                                                                               
            //World.AddConstraint(new HingeConstraint(bodies[1, 0, 1], bodies[1, 1, 1], Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitX));
            //World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[1, 2, 1], Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitX));
                                                                                                                                               
            World.AddConstraint(new HingeConstraint(bodies[0, 1, 1], bodies[1, 1, 1], Vector3.Zero, Vector3.Zero, Vector3.UnitZ, Vector3.UnitZ));
   
            //World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[2, 1, 1], Vector3.Zero, Vector3.Zero, Vector3.UnitX, Vector3.UnitX));
        }
    }
}
