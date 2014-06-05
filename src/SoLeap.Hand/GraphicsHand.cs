using BulletSharp;
using SoLeap.Domain;
using System;

namespace SoLeap.Hand
{
    public sealed class GraphicsHand : System.IDisposable
    {
        private readonly PhysicsHand physicsHand;

        public GraphicsHand(PhysicsHand physicsHand)
        {
            this.physicsHand = physicsHand;

            SetupRessources();
        }

        private void SetupRessources()
        {
            // TODO
        }

        private void CleanupRessources()
        {
            // TODO
        }

        public void Update(Domain.Hand hand)
        {
            physicsHand.Update(hand);

            // transformations have changed

            // TODO
        }

        public void Render(object device) // TODO
        {
            // TODO
        }

        public void Dispose()
        {
            CleanupRessources();
            GC.SuppressFinalize(this);
        }
    }
}