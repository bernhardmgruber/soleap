using BulletSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlPhysics
{
    /// <summary>
    /// Abstract base class for all bullet based physic scenes
    /// </summary>
    public abstract class PhysicsWorld : System.IDisposable
    {
        public Vector3 Gravity
        {
            get { return World.Gravity; }
            set { World.Gravity = value; }
        }

        public IDebugDraw DebugDrawer
        {
            get { return World.DebugDrawer; }
            set { World.DebugDrawer = value; }
        }

        // configuration
        protected CollisionConfiguration CollisionConfig { get; private set; }
        protected Dispatcher Dispatcher { get; private set; }
        protected BroadphaseInterface Broadphase { get; private set; }

        // the physics world
        public DiscreteDynamicsWorld World { get; private set; } // FIXME, world should not be public

        // all shapes that are used in collision
        public AlignedCollisionShapeArray CollisionShapes { get; private set; }

        // the last time a physics update of the secene was done
        private long lastUpdate;

        public PhysicsWorld(Vector3 gravity)
        {
            lastUpdate = Stopwatch.GetTimestamp();
            SetupPhysics(gravity);
        }

        private void SetupPhysics(Vector3 gravity)
        {
            CollisionShapes = new AlignedCollisionShapeArray();

            // collision configuration contains default setup for memory, collision setup
            CollisionConfig = new DefaultCollisionConfiguration();
            Dispatcher = new CollisionDispatcher(CollisionConfig);
            Broadphase = new DbvtBroadphase();

            // create world and set gravity
            World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, CollisionConfig);
            World.Gravity = gravity;

            SetupScene();
        }

        /// <summary>
        /// Called during construction after the physics world has been initialized.
        /// Derived classes should setup their rigit bodies and other stuff here.
        /// </summary>
        protected abstract void SetupScene();

        public void Update()
        {
            long time = Stopwatch.GetTimestamp();
            Update((time - lastUpdate) / (float)Stopwatch.Frequency);
        }

        public void Update(float deltaSeconds)
        {
            Debug.WriteLine("world step " + deltaSeconds);
            World.StepSimulation(deltaSeconds);
            lastUpdate = Stopwatch.GetTimestamp();
        }

        public void DebugDraw()
        {
            World.DebugDrawWorld();
        }

        public void Dispose()
        {
            //remove/dispose constraints
            for (int i = World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = World.GetConstraint(i);
                World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            //remove the rigidbodies from the dynamics world and delete them
            for (int i = World.NumCollisionObjects - 1; i >= 0; i--)
            {
                CollisionObject obj = World.CollisionObjectArray[i];
                RigidBody body = obj as RigidBody;
                if (body != null && body.MotionState != null)
                    body.MotionState.Dispose();
                World.RemoveCollisionObject(obj);
                obj.Dispose();
            }

            World.Dispose();

            //delete collision shapes
            foreach (CollisionShape shape in CollisionShapes)
                shape.Dispose();
            CollisionShapes.Clear();
            CollisionShapes.Dispose();

            Broadphase.Dispose();
            Dispatcher.Dispose();
            CollisionConfig.Dispose();
        }

        /// <summary>
        /// Creates a new rigit body and adds it to the world
        /// </summary>
        /// <param name="mass">The mass of the rigit body, must be larger than zero for dynamic objects</param>
        /// <param name="startTransform">The initial transformation of the object</param>
        /// <param name="shape">The shape that is used for collision detection with the body</param>
        /// <param name="userObject">An optional object assigned to the UserObject property of the rigid body</param>
        /// <returns>The newly created body. Usually this return value is not needed.</returns>
        public RigidBody CreateRigidBody(float mass, Matrix startTransform, CollisionShape shape, object userObject = null)
        {
            // rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            // using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
            var myMotionState = new DefaultMotionState(startTransform);
            var rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            var body = new RigidBody(rbInfo);
            rbInfo.Dispose();

            // set userobject, may be used to identify this body
            body.UserObject = userObject;

            // add it to the world
            World.AddRigidBody(body);

            return body;
        }
    }
}
