using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using SharpDX;
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

            graphicHands = new Dictionary<long, GraphicsHand>();
        }

        public void DrawHands(IDictionary<long, PhysicsHand> hands)
        {
            SyncronizeHands(hands);

            foreach (var gh in graphicHands) {
                var color = GetColorForId(gh.Key);
                gh.Value.Render(color);
            }
        }

        private void SyncronizeHands(IDictionary<long, PhysicsHand> hands)
        {
            // remove old hands
            var idsToRemove = graphicHands.Keys.Except(hands.Keys).ToList();
            foreach (long idToRemove in idsToRemove) {
                graphicHands[idToRemove].Dispose();
                graphicHands.Remove(idToRemove);
            }

            // create new hands
            var newIds = hands.Keys.Except(graphicHands.Keys);
            foreach (long newId in newIds) {
                graphicHands.Add(newId, new GraphicsHand(hands[newId], device));
            }
        }


        private Color3 GetColorForId(long id)
        {
            return Color.Blue.ToColor3();
            //return new Color3((int)id);
        }
    }
}