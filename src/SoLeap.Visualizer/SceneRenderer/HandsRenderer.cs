using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Sockets;
using SharpDX.Direct3D11;
using SoLeap.Hand;

namespace SoLeap.Visualizer
{
    public class HandsRenderer
    {
        private readonly Device device;

        private readonly IDictionary<long, GraphicsHand> graphicHands;

        public HandsRenderer(Device device)
        {
            Contract.Requires(device != null);

            this.device = device;
        }

        public void DrawHands(IDictionary<long, PhysicsHand> hands)
        {
            // remove old hands
            var idsToRemove = graphicHands.Keys.Except(hands.Keys);
            foreach (long idToRemove in idsToRemove) {
                graphicHands[idToRemove].Dispose();
                graphicHands.Remove(idToRemove);
            }

            // create new hands
            var newIds = hands.Keys.Except(graphicHands.Keys);
            foreach (long newId in newIds) {
                graphicHands.Add(newId, new GraphicsHand(device, hands[newId]));
            }

            // draw all hands
        }
    }
}