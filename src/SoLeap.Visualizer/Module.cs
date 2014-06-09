using Ninject.Modules;
using SoLeap.Devices;
using SoLeap.World;
using SoLeap.Worlds;

namespace SoLeap.Visualizer
{
    internal class Module
        : NinjectModule
    {
        public override void Load()
        {
            Bind<IHandsFrameProvider>().To<LeapProvider.LeapProvider>();

            Bind<IWorld>().To<CubesWorld>();
        }
    }
}
