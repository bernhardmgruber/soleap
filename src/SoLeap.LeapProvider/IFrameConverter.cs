using Leap;
using SoLeap.Devices;

namespace SoLeap.LeapProvider
{
    public interface IFrameConverter
    {
        HandsFrame Convert(Frame leapFrame);
    }
}