using Caliburn.Micro;
using SharpDX.WPF;
using SoLeap.Devices;
using SoLeap.Hand;
using SoLeap.Worlds;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SoLeap.Visualizer.ViewModels
{
    public sealed class MainWindowViewModel : PropertyChangedBase, IDisposable
    {
        private IHandsFrameProvider handsProvider;
        private IDictionary<long, GraphicsHand> hands = new Dictionary<long, GraphicsHand>();
        private IPhysicsWorld currentWorld;

        #region member for properties and commands

        private string selectedSceneName;
        private IEnumerable<Lazy<string>> sceneNames;
        private D3D11 renderer;

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
                NotifyOfPropertyChange(() => SelectedScene);
            }
        }

        public IEnumerable<Lazy<string>> SceneNames
        {
            get { return sceneNames; }
            set
            {
                sceneNames = value;
                NotifyOfPropertyChange(() => SceneNames);
            }
        }

        public D3D11 Renderer
        {
            get { return renderer; }
            set
            {
                Renderer = value;
                NotifyOfPropertyChange(() => Renderer);
            }
        }
        #endregion properties

        #region commands

        public ICommand ReloadSceneCommand
        {
            get
            {
                if (reloadSceneCommand == null)
                    reloadSceneCommand = new RelayCommand(o =>
                    {
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
                    recalibrateCommand = new RelayCommand(o =>
                    {
                        hands.Clear();
                    });

                return recalibrateCommand;
            }
        }

        #endregion commands

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel(IEnumerable<Lazy<string>> scenes, IHandsFrameProvider handsProvider, D3D11 renderer)
        {
            this.sceneNames = scenes;
            this.handsProvider = handsProvider;
            this.renderer = renderer;

            currentWorld = new BowlWorld(); // TODO remove
        }

        /// <summary>
        /// Shuts down the currently shown physics scenario and loads another one.
        /// </summary>
        private void SwitchScene(string scene)
        {
            // unload scene
            if (currentWorld != null)
                currentWorld.Dispose();

            if (scene != null)
            {
                // create new scene
            }
        }

        /// <summary>
        /// Called by the renderer to update the models data using leap input and bullet
        /// </summary>
        internal void Update()
        {
            if (currentWorld != null)
            {
                currentWorld.Update();

                var lastFrame = handsProvider.LastFrame;

                IDictionary<long, GraphicsHand> newHands = new Dictionary<long, GraphicsHand>();
                foreach (var hand in lastFrame.Hands)
                {
                    GraphicsHand gh;
                    if (hands.TryGetValue(hand.Id, out gh))
                    {
                        // this hand existed in the last frame, update it
                        gh.Update(hand);
                        hands.Remove(hand.Id);
                    }
                    else
                    {
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