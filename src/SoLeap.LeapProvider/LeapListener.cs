using Leap;
using System.Diagnostics;

namespace SoLeap.LeapProvider
{
    internal class LeapListener : Listener
    {
        private LeapProvider provider;

        public LeapListener(LeapProvider provider)
        {
            this.provider = provider;
        }

        public override void OnConnect(Controller c)
        {
            Debug.WriteLine("Leap connected");
        }

        public override void OnDisconnect(Controller c)
        {
            Debug.WriteLine("Leap disconnected");
        }

        public override void OnFrame(Controller c)
        {
            provider.ProcessFrame(c.Frame());
        }
    }
}