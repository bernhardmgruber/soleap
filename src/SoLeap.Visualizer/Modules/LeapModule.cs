using Ninject.Modules;
using SoLeap.Devices;
using SoLeap.LeapProvider;

namespace SoLeap.Visualizer.Modules
{
    public class LeapModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IHandsFrameProvider>().To<LeapProvider.LeapProvider>().InSingletonScope();
            Bind<IFrameConverter>().To<FrameConverter>().InSingletonScope();
        }
    }
}
