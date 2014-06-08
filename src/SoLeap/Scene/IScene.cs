using SoLeap.Worlds;

namespace SoLeap.Scene
{
    public interface IScene
    {
        string Name { get; }

        IPhysicsWorld World { get; }
    }
}
