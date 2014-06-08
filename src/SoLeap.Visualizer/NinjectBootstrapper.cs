using System.Windows;
using Caliburn.Micro;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoLeap.Visualizer
{
    public class NinjectBootstrapper
        : BootstrapperBase
    {
        private readonly IKernel kernel;

        public NinjectBootstrapper()
        {
            kernel = new StandardKernel();

            kernel.Bind<IWindowManager>().To<WindowManager>();

            kernel.Bind<MainWindowViewModel>().ToSelf();

            //kernel.Load(new Module());

            Initialize();
        }

        protected override void Configure()
        {
            kernel.Load("*.dll");

            var assemblies = kernel.GetModules()
                .Select(m => m.GetType().Assembly)
                .Distinct()
                .ToList();
            AssemblySource.Instance.AddRange(assemblies);

            base.Configure();
        }

        protected override object GetInstance(Type service, string key)
        {
            return kernel.Get(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }

        protected override void BuildUp(object instance)
        {
            kernel.Inject(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            kernel.Dispose();
        }
    }
}