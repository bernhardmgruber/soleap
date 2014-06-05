using System;

namespace SoLeap.Devices
{
    public interface IHandsFrameProvider
    {
        event EventHandler<HandsFrame> FrameReady;
    }
}