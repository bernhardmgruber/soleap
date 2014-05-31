using BulletSharp;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BowlPhysics
{
    public class Program
    {
        [STAThreadAttribute]
        public static void Main()
        {
            var kernel = new StandardKernel();

            kernel.Bind<PhysicsWorld>().To<RubicsPhysicsWorld>();

            var app = new Application();
            app.Run(kernel.Get<MainWindow>());
        }
    }
}
