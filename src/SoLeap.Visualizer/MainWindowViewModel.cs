using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using SoLeap.Devices;
using SoLeap.Hand;
using SoLeap.Scene;
using SoLeap.Worlds;

namespace SoLeap.Visualizer
{
    public sealed class MainWindowViewModel
        : PropertyChangedBase, IDisposable
    {
        private readonly IHandsFrameProvider handsProvider;

        public BindableCollection<IScene> Scenes
        {
            get { return scenes; }
            set { scenes = value; NotifyOfPropertyChange(() => Scenes); }
        }
        private BindableCollection<IScene> scenes;

        public IScene CurrentScene
        {
            get { return currentScene; }
            set { currentScene = value; NotifyOfPropertyChange(() => CurrentScene); }
        }
        private IScene currentScene;

        private IDictionary<long, GraphicsHand> hands = new Dictionary<long, GraphicsHand>();
        private IPhysicsWorld currentWorld;

        #region member for properties and commands

        private string selectedSceneName;
        private IEnumerable<Lazy<string>> sceneNames;

        private ICommand reloadSceneCommand;
        private ICommand recalibrateCommand;

        #endregion member for properties and commands

        #region properties

        public String SelectedScene
        {
            get { return selectedSceneName; }
            set
            {
                selectedSceneName = value;
                SwitchScene(value);
            }
        }

        public IEnumerable<Lazy<string>> SceneNames
        {
            get { return sceneNames; }
            set { sceneNames = value; }
        }

        #endregion properties

        #region commands

        public ICommand ReloadSceneCommand
        {
            get
            {
                if (reloadSceneCommand == null)
                    reloadSceneCommand = new RelayCommand(o => {
                        SwitchScene(selectedSceneName);
                    });

                return reloadSceneCommand;
            }
        }

        public ICommand RecalibrateCommand
        {
            get
            {
                if (recalibrateCommand == null)
                    recalibrateCommand = new RelayCommand(o => {
                        hands.Clear();
                    });

                return recalibrateCommand;
            }
        }

        #endregion commands

        public MainWindowViewModel()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel(IHandsFrameProvider handsProvider, IEnumerable<IScene> scenes)
        {
            this.handsProvider = handsProvider;
            Scenes = new BindableCollection<IScene>(scenes);
            //SceneNames = scenes;

        }

        /// <summary>
        /// Shuts down the currently shown physics scenario and loads another one.
        /// </summary>
        private void SwitchScene(string scene)
        {
            // unload scene
            if (currentWorld != null)
                currentWorld.Dispose();

            if (scene != null) {
                // create new scene
            }
        }

        /// <summary>
        /// Called by the renderer to update the models data using leap input and bullet
        /// </summary>
        internal void Update()
        {
            if (currentWorld != null) {
                currentWorld.Update();

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
                        gh = new GraphicsHand(null, new byte[0], currentWorld, hand); // TODO
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
            SwitchScene(null);
        }
    }
}