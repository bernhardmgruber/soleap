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

        public PhysicsWorld(IDebugDraw debugDrawer)
        {
            lastUpdate = Stopwatch.GetTimestamp();
            SetupPhysics();
            world.DebugDrawer = debugDrawer;
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
            world.Gravity = new Vector3(0, -10, 0);

            // create static ground
            BoxShape groundShape = new BoxShape(10, 1, 10);
            collisionShapes.Add(groundShape);

            var ground = CreateRigidBody(0, Matrix.Identity, groundShape);
            ground.UserObject = "Ground";
            world.AddRigidBody(ground);

            // create a few dynamic rigidbodies
            const float mass = 1.0f;

            BoxShape colShape = new BoxShape(1);
            collisionShapes.Add(colShape);

            const float start_x = StartPosX - ArraySizeX / 2;
            const float start_y = StartPosY;
            const float start_z = StartPosZ - ArraySizeZ / 2;

            int k, i, j;
            for (k = 0; k < ArraySizeY; k++)
            {
                for (i = 0; i < ArraySizeX; i++)
                {
                    for (j = 0; j < ArraySizeZ; j++)
                    {
                        Matrix startTransform = Matrix.Translation(
                            2 * i + start_x,
                            2 * k + start_y,
                            2 * j + start_z
                        );

                        var body = CreateRigidBody(mass, startTransform, colShape);

                        // make it drop from a height
                        body.Translate(new Vector3(0, 20, 0));
                        world.AddRigidBody(body);
                    }
                }
            }

            SphereShape ballShape = new SphereShape(1);
            collisionShapes.Add(ballShape);

            var ballBody = CreateRigidBody(1.0f, Matrix.Translation(0, 30, 0), ballShape);
            world.AddRigidBody(ballBody);

        }

        public void Draw()
        {
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
