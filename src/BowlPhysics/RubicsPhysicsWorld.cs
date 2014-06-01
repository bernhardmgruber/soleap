﻿using System;
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
            : base(new Vector3(0, -10, 0)) { }

        const float subCubeSize = 1.5f;
        const float subCubeMass = 1.0f;
        const float bevel = 0.2f;

        const float subCubeGap = 0.1f;

        private CollisionShape CreateSubCubeShape()
        {
            return new BoxShape(subCubeSize / 2.0f);
        }

        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(20, 0.5f, 20);
            CollisionShapes.Add(groundShape);

            CreateRigidBody(0, Matrix.Translation(0, -4, 0), groundShape, "Ground");

            // create basic shape for a small sub cube
            var shape = CreateSubCubeShape();

            // create the 27 rigid bodies
            var bodies = new RigidBody[3, 3, 3];

            for (int z = 0; z < 3; z++)
                for (int y = 0; y < 3; y++)
                    for (int x = 0; x < 3; x++)
                        //if ((x == 1 && y == 1) || (x == 1 && z == 1) || (y == 1 && z == 1))
                        //if(x == y && y == z) 
                        bodies[x, y, z] = CreateRigidBody(
                            subCubeMass,
                            Matrix.Translation((x - 1) * (1.0f + subCubeGap), (y - 1) * (1.0f + subCubeGap), (z - 1) * (1.0f + subCubeGap)),
                            shape,
                            String.Format("subcube {0} {1} {2}", x, y, z)
                            );

            // add hinge constraints for center sub cubes
            // see http://bulletphysics.org/mediawiki-1.5.8/index.php/Constraints
            // and https://www.panda3d.org/manual/index.php/Bullet_Constraints

            float cubeDist2 = (subCubeSize + subCubeGap) / 2.0f;

            World.AddConstraint(new HingeConstraint(bodies[0, 1, 1], bodies[1, 1, 1], new Vector3(-cubeDist2, 0, 0), new Vector3(+cubeDist2, 0, 0), Vector3.UnitX, Vector3.UnitX));
            World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[2, 1, 1], new Vector3(-cubeDist2, 0, 0), new Vector3(+cubeDist2, 0, 0), Vector3.UnitX, Vector3.UnitX));

            World.AddConstraint(new HingeConstraint(bodies[1, 0, 1], bodies[1, 1, 1], new Vector3(0, -cubeDist2, 0), new Vector3(0, +cubeDist2, 0), Vector3.UnitY, Vector3.UnitY));
            World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[1, 2, 1], new Vector3(0, -cubeDist2, 0), new Vector3(0, +cubeDist2, 0), Vector3.UnitY, Vector3.UnitY));

            World.AddConstraint(new HingeConstraint(bodies[1, 1, 0], bodies[1, 1, 1], new Vector3(0, 0, -cubeDist2), new Vector3(0, 0, +cubeDist2), Vector3.UnitZ, Vector3.UnitZ));
            World.AddConstraint(new HingeConstraint(bodies[1, 1, 1], bodies[1, 1, 2], new Vector3(0, 0, -cubeDist2), new Vector3(0, 0, +cubeDist2), Vector3.UnitZ, Vector3.UnitZ));

            // add distance constraints between center and corners

            float cubeDist2_r3 = cubeDist2 * (float)Math.Sqrt(3);

            World.AddConstraint(new Point2PointConstraint(bodies[0, 0, 0], bodies[1, 1, 1], new Vector3(-cubeDist2_r3), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 1, 1], bodies[2, 2, 2], Vector3.Zero, new Vector3(+cubeDist2_r3)));

            World.AddConstraint(new Point2PointConstraint(bodies[2, 0, 0], bodies[1, 1, 1], new Vector3(+cubeDist2_r3, -cubeDist2_r3, -cubeDist2_r3), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 1, 1], bodies[0, 2, 2], Vector3.Zero, new Vector3(-cubeDist2_r3, +cubeDist2_r3, +cubeDist2_r3)));

            World.AddConstraint(new Point2PointConstraint(bodies[2, 2, 0], bodies[1, 1, 1], new Vector3(+cubeDist2_r3, +cubeDist2_r3, -cubeDist2_r3), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 1, 1], bodies[0, 0, 2], Vector3.Zero, new Vector3(-cubeDist2_r3, -cubeDist2_r3, +cubeDist2_r3)));

            World.AddConstraint(new Point2PointConstraint(bodies[0, 2, 0], bodies[1, 1, 1], new Vector3(-cubeDist2_r3, +cubeDist2_r3, -cubeDist2_r3), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 1, 1], bodies[2, 0, 2], Vector3.Zero, new Vector3(+cubeDist2_r3, -cubeDist2_r3, +cubeDist2_r3)));

            // add distance constraints between center and edges
            float cubeDist2_r2 = cubeDist2 * (float)Math.Sqrt(2);

            World.AddConstraint(new Point2PointConstraint(bodies[0, 1, 0], bodies[1, 1, 1], new Vector3(-cubeDist2_r2, 0, -cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[2, 1, 0], bodies[1, 1, 1], new Vector3(+cubeDist2_r2, 0, -cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[2, 1, 2], bodies[1, 1, 1], new Vector3(+cubeDist2_r2, 0, +cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[0, 1, 2], bodies[1, 1, 1], new Vector3(-cubeDist2_r2, 0, +cubeDist2_r2), Vector3.Zero));

            World.AddConstraint(new Point2PointConstraint(bodies[1, 0, 0], bodies[1, 1, 1], new Vector3(0, -cubeDist2_r2, -cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 2, 0], bodies[1, 1, 1], new Vector3(0, +cubeDist2_r2, -cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 2, 2], bodies[1, 1, 1], new Vector3(0, +cubeDist2_r2, +cubeDist2_r2), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[1, 0, 2], bodies[1, 1, 1], new Vector3(0, -cubeDist2_r2, +cubeDist2_r2), Vector3.Zero));

            World.AddConstraint(new Point2PointConstraint(bodies[0, 0, 1], bodies[1, 1, 1], new Vector3(-cubeDist2_r2, -cubeDist2_r2, 0), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[2, 0, 1], bodies[1, 1, 1], new Vector3(+cubeDist2_r2, -cubeDist2_r2, 0), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[2, 2, 1], bodies[1, 1, 1], new Vector3(+cubeDist2_r2, +cubeDist2_r2, 0), Vector3.Zero));
            World.AddConstraint(new Point2PointConstraint(bodies[0, 2, 1], bodies[1, 1, 1], new Vector3(-cubeDist2_r2, +cubeDist2_r2, 0), Vector3.Zero));
        }
    }
}
