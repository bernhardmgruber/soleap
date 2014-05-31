using System;

namespace SoLeap.Device
{
    public interface IHandsFrameProvider
    {
        event EventHandler<HandsFrame> FrameReady;
    }
}