using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace LeapHandReconstruction
{
    public delegate void LeapConnectHandler();
    public delegate void LeapDisconnectHandler();
    public delegate void LeapFrameHandler(Frame frame);

    internal class LeapMotionDevice : IDisposable
    {
        /// <summary>
        /// Is raised when a Leap Motion controller is connected to the computer
        /// </summary>
        public event LeapConnectHandler OnConnect;

        /// <summary>
        /// Is raised when a Leap Motion controller is disconnected from the computer
        /// </summary>
        public event LeapDisconnectHandler OnDisconnect;

        /// <summary>
        /// Is raised when a new frame has been captured by the Leap Motion controller
        /// </summary>
        public event LeapFrameHandler OnFrame;

        /// <summary>
        /// Returns the last indexth frame
        /// </summary>
        public Frame Frame(int index)
        {
            return controller.Frame(index);
        }

        private Controller controller;
        private LeapListener listener;

        internal LeapMotionDevice()
        {
            controller = new Controller();
            listener = new LeapListener(this);
            controller.AddListener(listener);

            OnConnect += () => Console.WriteLine("Leap connected");
            OnDisconnect += () => Console.WriteLine("Leap disconnected");
        }

        public void Dispose()
        {
            controller.RemoveListener(listener);
            controller.Dispose();
        }

        /// <summary>
        /// Implements a Leap.Listener and forwards leap events to the C# events of the outer LeapMotionDevice class
        /// </summary>
        private class LeapListener : Listener
        {
            private LeapMotionDevice device;

            public LeapListener(LeapMotionDevice device)
            {
                this.device = device;
            }

            public override void OnConnect(Controller c)
            {
                if (device.OnConnect != null)
                    device.OnConnect();
            }

            public override void OnDisconnect(Controller c)
            {
                if (device.OnDisconnect != null)
                    device.OnDisconnect();
            }

            public override void OnFrame(Controller c)
            {
                if (device.OnFrame != null)
                    device.OnFrame(c.Frame());
            }
        }
    }
}
