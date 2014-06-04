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
    public abstract class AbstractPhysicsWorld : IPhysicsWorld
    {
        public Vector3 Gravity
        {
            get { return world.Gravity; }
            set { world.Gravity = value; }
        }

        public IDebugDraw DebugDrawer
        {
            get { return world.DebugDrawer; }
            set { world.DebugDrawer = value; }
        }

        // configuration
        private CollisionConfiguration collisionConfig;
        private Dispatcher dispatcher;
        private BroadphaseInterface broadphase;

        // the physics world
        private DiscreteDynamicsWorld world;

        // all shapes that are used in collision
        private AlignedCollisionShapeArray collisionShapes;

        // the last time a physics update of the secene was done
        private long lastUpdate;

        protected AbstractPhysicsWorld(Vector3 gravity)
        {
            lastUpdate = Stopwatch.GetTimestamp();
            SetupPhysics(gravity);
        }

        private void SetupPhysics(Vector3 gravity)
        {
            collisionShapes = new AlignedCollisionShapeArray();

            // collision configuration contains default setup for memory, collision setup
            collisionConfig = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfig);
            broadphase = new DbvtBroadphase();

            // create world and set gravity
            world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfig);
            world.Gravity = gravity;

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
            //Debug.WriteLine("world step " + deltaSeconds);
            world.StepSimulation(deltaSeconds);
            lastUpdate = Stopwatch.GetTimestamp();
        }

        public void DebugDraw()
        {
            world.DebugDrawWorld();
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
        }

        /// <summary>
        /// Creates a new rigit body and adds it to the world
        /// </summary>
        /// <param name="mass">The mass of the rigit body, must be larger than zero for dynamic objects</param>
        /// <param name="startTransform">The initial transformation of the object</param>
        /// <param name="shape">The shape that is used for collision detection with the body</param>
        /// <param name="userObject">An optional object assigned to the UserObject property of the rigid body</param>
        /// <returns>The newly created body. Usually this return value is not needed.</returns>
        public RigidBody CreateAndAddRigidBody(float mass, Matrix startTransform, CollisionShape shape, object userObject = null, bool isKinematic = false)
        {
            // rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic)
                shape.CalculateLocalInertia(mass, out localInertia);

            // using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
            var rbInfo = new RigidBodyConstructionInfo(mass, new DefaultMotionState(startTransform), shape, localInertia);
            var body = new RigidBody(rbInfo);
            rbInfo.Dispose();

            // kinematic settings
            if (isKinematic)
            {
                body.CollisionFlags = body.CollisionFlags | CollisionFlags.KinematicObject;
                body.ActivationState = ActivationState.DisableDeactivation;
            }

            // set userobject, may be used to identify this body
            body.UserObject = userObject;

            // add it to the world
            world.AddRigidBody(body);

            return body;
        }

        public void Add(CollisionShape shape)
        {
            collisionShapes.Add(shape);
        }

        public void Add(TypedConstraint constraint)
        {
            world.AddConstraint(constraint);
        }
    }
}
