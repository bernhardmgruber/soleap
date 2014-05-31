using System;
using System.Collections.Generic;

namespace SoLeap.Visualizer.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IEnumerable<Lazy<string>> scenes;

        public MainWindowViewModel(IEnumerable<Lazy<string>> scenes)
        {
            this.scenes = scenes;
        }
    }
}
