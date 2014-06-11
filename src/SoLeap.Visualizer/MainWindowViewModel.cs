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
        #region Hands

        private readonly IHandsFrameProvider handsProvider;

        private IDictionary<long, GraphicsHand> hands = new Dictionary<long, GraphicsHand>();

        #endregion


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
            if (propertyChangedEventArgs.PropertyName == this.GetPropertyName(() => CurrentScene)) {
                if (CurrentScene != null && !CurrentScene.IsLoaded)
                    CurrentScene.SetupScene();
            }
        }

        private void ReloadScene()
        {
            CurrentScene = Scenes[0];
        }

        /*
        /// <summary>
        /// Shuts down the currently shown physics scenario and loads another one.
        /// </summary>
        private void SwitchScene(IWorld scene)
        {
            // unload scene
            if (CurrentScene != null)
                CurrentScene.Dispose();

            if (scene != null) {
                // create new scene
            }
        }*/

        /// <summary>
        /// Called by the renderer to update the models data using leap input and bullet
        /// </summary>
        internal void Update()
        {
            if (CurrentScene != null) {
                CurrentScene.Update();

                var lastFrame = handsProvider.LastFrame;

                IDictionary<long, GraphicsHand> newHands = new Dictionary<long, GraphicsHand>();
                foreach (var hand in lastFrame.Hands) {
                    GraphicsHand gh;
                    if (hands.TryGetValue(hand.Id, out gh)) {
                        // this hand existed in the last frame, update it
                        gh.Update(hand);
                        hands.Remove(hand.Id);
                    } else {
                        // this hand is new, create it
                        //gh = new GraphicsHand(null, new byte[0], CurrentScene, hand); // TODO
                    }
                    newHands[hand.Id] = gh;
                }

                // remove missing hands
                foreach (var missingHands in hands.Values)
                    missingHands.Dispose();

                hands = newHands;
            }
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