using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using SoLeap.World;

namespace SoLeap.Visualizer
{
    public sealed class MainWindowViewModel
        : PropertyChangedBase, IDisposable
    {
        private readonly HandsManager handsManager;

        public BindableCollection<IWorld> Scenes
        {
            get { return scenes; }
            set { if (scenes != value) { scenes = value; NotifyOfPropertyChange(); } }
        }
        private BindableCollection<IWorld> scenes;

        public IWorld CurrentScene
        {
            get { return currentScene; }
            set { if (currentScene != value) { currentScene = value; NotifyOfPropertyChange(); } }
        }
        private IWorld currentScene;

        public HandsManager Hands
        {
            get { return hands; }
            set { if (hands != value) { hands = value; NotifyOfPropertyChange(); } }
        }
        private HandsManager hands;

        #region Commands

        public ICommand ReloadSceneCommand
        {
            get { return reloadSceneCommand ?? (reloadSceneCommand = new RelayCommand(o => ReloadScene())); }
        }
        private ICommand reloadSceneCommand;

        public ICommand RecalibrateCommand
        {
            get { return recalibrateCommand ?? (recalibrateCommand = new RelayCommand(o => hands.Clear())); }
        }
        private ICommand recalibrateCommand;

        #endregion

        public MainWindowViewModel(HandsManager handsManager, IEnumerable<IWorld> scenes)
        {
            this.handsManager = handsManager;
            Scenes = new BindableCollection<IWorld>(scenes);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == this.GetPropertyName(() => CurrentScene)) {
                handsManager.Clear();

                if (CurrentScene != null && !CurrentScene.IsLoaded) {
                    CurrentScene.Updating += CurrentSceneOnUpdating;
                    CurrentScene.Load();
                }
            }
        }

        private void CurrentSceneOnUpdating(object sender, EventArgs eventArgs)
        {
            handsManager.Update(CurrentScene);
        }

        private void ReloadScene()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //SwitchScene(null);
            foreach (var scene in Scenes.Where(s => s.IsLoaded)) {
                //scene.Unload();
                scene.Dispose();
            }
            Scenes.Clear();
        }
    }
}