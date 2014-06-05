using Leap;
using SoLeap.Devices;
using System;

namespace SoLeap.LeapProvider
{
    public class LeapProvider
        : IDisposable, IHandsFrameProvider
    {
        public event EventHandler<HandsFrame> FrameReady;

        public HandsFrame LastFrame
        {
            get
            {
                return frameConverter.Convert(controller.Frame(0));
            }
        }

        private readonly Controller controller;
        private readonly IFrameConverter frameConverter;
        private LeapListener listener;

        public LeapProvider(IFrameConverter frameConverter)
        {
            this.frameConverter = frameConverter;

            listener = new LeapListener(this);

            controller = new Controller();
            controller.AddListener(listener);
        }

        /// <summary>
        /// Called by leap listener
        /// </summary>
        /// <param name="f"></param>
        public void ProcessFrame(Frame f)
        {
            var handler = FrameReady;
            if (handler != null)
            {
                handler(this, frameConverter.Convert(f));
            }
        }

        public void Dispose()
        {
            controller.RemoveListener(listener);
            listener.Dispose();
            controller.Dispose();
        }
    }
}