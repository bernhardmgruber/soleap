using System.Collections.Generic;

namespace SoLeap.World
{
    public interface IRenderableWorld
    {
        IList<WorldObject> Renderables { get; }
    }
}
