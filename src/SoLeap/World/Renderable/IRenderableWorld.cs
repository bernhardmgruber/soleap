using System.Collections.Generic;

namespace SoLeap.World
{
    public interface IRenderableWorld
    {
        // bgruber: i would remove this and let the world decide how it wants to render itself
        IList<RigidBodyRenderable> Renderables { get; }

        // e.g.
        // void PrepareForRendering();
        // void Render();
        // void CleanupAfterRendering();
    }
}
