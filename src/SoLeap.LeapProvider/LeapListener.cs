using Leap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
