
using Ninject.Modules;
using SoLeap.Devices;

namespace SoLeap.Visualizer
{
    internal class Module
        : NinjectModule
    {
        public override void Load()
        {
            Bind<IHandsFrameProvider>().To<LeapProvider.LeapProvider>();
        }
    }
}
