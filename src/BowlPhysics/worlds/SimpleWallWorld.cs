using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlPhysics
{
    class SimpleWallWorld : AbstractPhysicsWorld
    {
        const float sceneHeight = 100f;

        const float gravity = 500f;

        Vector3 wallDimensions2 = new Vector3(20f, 200f, 200f);

        public SimpleWallWorld()
            : base(new Vector3(0f, -gravity, 0f)) { }

        protected override void SetupScene()
        {
            // create static ground
            BoxShape groundShape = new BoxShape(1000f, 10f, 1000f);
            Add(groundShape);

            CreateAndAddRigidBody(0f, Matrix.Translation(0, sceneHeight, 0), groundShape, "Ground");
   
            // create walls
            BoxShape wallShape = new BoxShape(wallDimensions2);
            Add(wallShape);

            CreateAndAddRigidBody(0f, Matrix.Translation(-200f, sceneHeight + wallDimensions2.Y, 0), wallShape, "static wall");
            CreateAndAddRigidBody(10f, Matrix.Translation(+200f, sceneHeight + wallDimensions2.Y, 0), wallShape, "dynamic wall");
        }
    }
}
