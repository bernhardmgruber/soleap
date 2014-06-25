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
            Bind<IWorld>().To<CubesWorld>();
            Bind<IWorld>().To<BowlWorld>();
            Bind<IWorld>().To<DumbbellWorld>();
            Bind<IWorld>().To<HoleWorld>();
            Bind<IWorld>().To<LeverWorld>();
            Bind<IWorld>().To<JengaWorld>();
            Bind<IWorld>().To<RubicsWorld>();
        }
    }
}
