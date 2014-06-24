using Ninject.Modules;
using SoLeap.World;
using SoLeap.Worlds;

namespace SoLeap.Visualizer.Modules
{
    public class WorldModule
        : NinjectModule
    {
        public override void Load()
        {
            Bind<IWorld>().To<CubesWorld>()/*.InSingletonScope()*/;
            Bind<IWorld>().To<BowlWorld>()/*.InSingletonScope()*/;
            Bind<IWorld>().To<DumbbellWorld>();
            Bind<IWorld>().To<SimpleWallWorld>();
            //Bind<IWorld>().To<RubicsWorld>();
        }
    }
}
