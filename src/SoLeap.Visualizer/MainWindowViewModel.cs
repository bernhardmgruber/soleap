using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using SoLeap.Devices;
using SoLeap.Hand;
using SoLeap.World;

namespace SoLeap.Visualizer
{
    public sealed class MainWindowViewModel
        : PropertyChangedBase, IDisposable
    {
        private IHandsFrameProvider handsProvider;

        #region Scenes

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

        public InteractingHands Hands
        {
            get { return hands; }
            set { if (hands != value) { hands = value; NotifyOfPropertyChange(); } }
        }
        private InteractingHands hands;

        #endregion


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

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel(IHandsFrameProvider handsProvider, IEnumerable<IWorld> scenes)
        {
            this.handsProvider = handsProvider;
            Scenes = new BindableCollection<IWorld>(scenes);

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == this.GetPropertyName(() => CurrentScene))
            {
                if (CurrentScene != null && !CurrentScene.IsLoaded)
                {
                    CurrentScene.Load();
                    Hands = new InteractingHands(handsProvider, CurrentScene);
                }
            }
        }

        private void ReloadScene()
        {
            CurrentScene = Scenes[0];
        }

        public void Dispose()
        {
            //SwitchScene(null);
            foreach (var scene in Scenes.Where(s => s.IsLoaded))
            {
                //scene.Unload();
                scene.Dispose();
            }
            Scenes.Clear();
        }
    }
}