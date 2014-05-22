using System;

namespace Device
{
    public delegate void FrameReadyHandler(HandInputFrame frame);

    public interface IDevice : IDisposable
    {
        event FrameReadyHandler FrameReady;
    }
}