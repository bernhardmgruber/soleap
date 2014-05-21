using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using System.Diagnostics;

namespace BowlPhysics
{
    public class PhysicsWorld : System.IDisposable
    {
        CollisionConfiguration collisionConfig;
        Dispatcher dispatcher;
        BroadphaseInterface broadphase;

        DiscreteDynamicsWorld world;

        AlignedCollisionShapeArray collisionShapes;

        // create 125 (5x5x5) dynamic objects
        const int ArraySizeX = 2, ArraySizeY = 2, ArraySizeZ = 2;

        // scaling of the objects (0.1 = 20 centimeter boxes )
        const float StartPosX = -2;
        const float StartPosY = -2;
        const float StartPosZ = -2;

        private long lastUpdate;

        public PhysicsWorld()
        {
            lastUpdate = Stopwatch.GetTimestamp();
            SetupPhysics();
        }

        private void SetupPhysics()
        {
            collisionShapes = new AlignedCollisionShapeArray();

            // collision configuration contains default setup for memory, collision setup
            collisionConfig = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfig);
            broadphase = new DbvtBroadphase();

            // create world and set gravity
            world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfig);
            world.Gravity = new Vector3(0, -50, 0);

            // create static ground
            BoxShape groundShape = new BoxShape(10, 0.5f, 10);
            collisionShapes.Add(groundShape);

            var ground = CreateRigidBody(0, Matrix.Identity, groundShape);
            ground.UserObject = "Ground";
            world.AddRigidBody(ground);

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
                collisionShapes.Add(bowlShape);

                var bowl = CreateRigidBody(2.0f, Matrix.Translation(-5 + i * 10, 2, 0), bowlShape);
                bowl.UserObject = "Bowl " + i;
                world.AddRigidBody(bowl);
            }

            // create the ball
            SphereShape ballShape = new SphereShape(0.5f);
            collisionShapes.Add(ballShape);

            var ballBody = CreateRigidBody(1.0f, Matrix.Translation(-5, 5, 0), ballShape);
            ballBody.UserObject = "Ball";
            world.AddRigidBody(ballBody);

        }

        public void DebugDraw(IDebugDraw debugDraw)
        {
            world.DebugDrawer = debugDraw;

            world.DebugDrawWorld();

        }

        public void Update()
        {
            long time = Stopwatch.GetTimestamp();
            Update((time - lastUpdate) / (float)Stopwatch.Frequency);
        }

        public void Update(float deltaSeconds)
        {
            Debug.WriteLine("world step " + deltaSeconds);
            world.StepSimulation(deltaSeconds);
            lastUpdate = Stopwatch.GetTimestamp();
        }

        private RigidBody CreateRigidBody(float mass, Matrix startTransform, CollisionShape shape)
        {
            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
            var myMotionState = new DefaultMotionState(startTransform);
            var rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            var body = new RigidBody(rbInfo);
            rbInfo.Dispose();

            return body;
        }

        public void Dispose()
        {
            //remove/dispose constraints
            for (int i = world.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = world.GetConstraint(i);
                world.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (int i = world.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = world.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                    body.MotionState.Dispose();
                world.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            world.Dispose();

            //delete collision shapes
            foreach (CollisionShape shape in collisionShapes)
                shape.Dispose();
            collisionShapes.Clear();
            collisionShapes.Dispose();

            broadphase.Dispose();
            dispatcher.Dispose();
            collisionConfig.Dispose();

            collisionShapes.Dispose();

        }
    }
}
