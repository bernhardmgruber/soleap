using Ninject.Modules;

namespace SoLeap.Visualizer.Modules
{
    public class VisualizerModule
        : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindowViewModel>().ToSelf().InSingletonScope();
            Bind<HandsManager>().ToSelf().InSingletonScope();
        }
    }
}
