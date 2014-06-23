using SoLeap.Devices;
using SoLeap.Hand;
using SoLeap.World;
using System;
using System.Collections.Generic;

namespace SoLeap.Visualizer
{
    public class HandsManager
    {
        private readonly IHandsFrameProvider handsProvider;

        private readonly IDictionary<long, PhysicsHand> hands = new Dictionary<long, PhysicsHand>();

        public IDictionary<long, PhysicsHand> Hands
        {
            get { return hands; }
        }

        public HandsManager(IHandsFrameProvider handsProvider)
        {
            this.handsProvider = handsProvider;

            hands = new Dictionary<long, PhysicsHand>();
        }

        // Called by the renderer
        public void Update(IWorld world)
        {
            var lastFrame = handsProvider.LastFrame;

            var newHands = new Dictionary<long, PhysicsHand>();
            foreach (var frameHand in lastFrame.Hands) {
                PhysicsHand hand;
                if (hands.TryGetValue(frameHand.Id, out hand)) {
                    // this hand existed in the last frame, update it
                    hand.Update(frameHand);
                    //hands.Remove(frameHand.Id);
                } else {
                    // this hand is new, create it
                    hand = new PhysicsHand(world, frameHand);
                }
                newHands[frameHand.Id] = hand;
            }

            hands.Clear();
            foreach (var newHand in newHands)
                hands.Add(newHand);
        }

        public void Clear()
        {
            hands.Clear();
        }
    }
}
