using BowlPhysics.Worlds;
using Ninject;
using SoLeap.Device;
using SoLeap.LeapProvider;
using System;
using System.Windows;

namespace BowlPhysics
{
    public class Program
    {
        [STAThreadAttribute]
        public static void Main()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IPhysicsWorld>().To<CubesWorld>();

            kernel.Bind<IHandsFrameProvider>().To<LeapProvider>();
            kernel.Bind<IFrameConverter>().To<FrameConverter>();

            var app = new Application();
            app.Run(kernel.Get<MainWindow>());
        }
    }
}