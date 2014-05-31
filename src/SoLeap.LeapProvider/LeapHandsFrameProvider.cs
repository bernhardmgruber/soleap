using System;
using System.Diagnostics;
using Leap;
using SoLeap.Device;

namespace SoLeap.LeapProvider
{
    public class LeapProvider
        : Listener, IHandsFrameProvider
    {
        private readonly Controller controller;

        private readonly IFrameConverter frameConverter;

        public event EventHandler<HandsFrame> FrameReady;

        public LeapProvider(IFrameConverter frameConverter)
        {
            this.frameConverter = frameConverter;

            controller = new Controller();
            controller.AddListener(this);
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
            var handler = FrameReady;
            if (handler != null) {
                handler(this, frameConverter.Convert(c.Frame()));
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            controller.RemoveListener(this);
            controller.Dispose();
        }
    }
}
