using BulletSharp;

namespace SoLeap.Worlds
{
    public interface IPhysicsWorld : System.IDisposable
    {
        Vector3 Gravity { get; set; }

        IDebugDraw DebugDrawer { set; }

        void Add(CollisionShape shape);

        void Add(TypedConstraint constraint);

        RigidBody CreateAndAddRigidBody(float mass, Matrix startTransform, CollisionShape shape, object userObject = null, bool isKinematic = false);

        void Update();

        void DebugDraw();
    }
}