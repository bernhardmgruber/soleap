using System;
using SoLeap.Common.Domain;

namespace SoLeap.Common.Interfaces
{
    public interface IHandFrameProvider
    {
        event Action<HandsFrame> FrameReady;
    }
}
