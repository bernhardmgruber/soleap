using Caliburn.Micro;
using Ninject;
using SharpDX.WPF;
using SoLeap.Visualizer.ViewModels;
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
            StartRuntime();
        }

        protected override void Configure()
        {
            kernel.Load("*.dll");

            //kernel.Bind<D3D11>().To<TestRenderer>();
      

            var assemblies = kernel.GetModules()
                .Select(m => m.GetType().Assembly)
                .Distinct()
                .ToList();
            AssemblySource.Instance.AddRange(assemblies);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
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

        protected override void OnExit(object sender, EventArgs e)
        {
            kernel.Dispose();
        }
    }
}