namespace SoLeap.World
{
    public interface IWorld
        : IPhysicsWorld, IRenderableWorld
    {
        string Name { get; }

        bool IsLoaded { get; }

        void SetupScene();
    }
}
