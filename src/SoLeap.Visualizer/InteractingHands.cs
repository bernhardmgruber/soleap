using SoLeap.Devices;
using SoLeap.Hand;
using SoLeap.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoLeap.Visualizer
{
    public sealed class InteractingHands : IDisposable
    {
        private readonly IHandsFrameProvider handsProvider;
        private readonly IPhysicsWorld physicsWorld;

        private IDictionary<long, GraphicsHand> hands = new Dictionary<long, GraphicsHand>();

        public InteractingHands(IHandsFrameProvider handsProvider, IPhysicsWorld physicsWorld)
        {
            this.handsProvider = handsProvider;
            this.physicsWorld = physicsWorld;
        }

        /// <summary>
        /// Called by the renderer to update the models data using leap input and bullet
        /// </summary>
        public void Update()
        {
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
                    gh = new GraphicsHand(null, new byte[0], physicsWorld, hand); // FIXME
                }
                newHands[hand.Id] = gh;
            }

            // remove missing hands
            foreach (var missingHands in hands.Values)
                missingHands.Dispose();

            hands = newHands;
        }

        public void Clear()
        {
            foreach (var h in hands.Values)
                h.Dispose();
            hands.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
