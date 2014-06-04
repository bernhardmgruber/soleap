using Leap;
using SoLeap.Device;

namespace SoLeap.LeapProvider
{
    public interface IFrameConverter
    {
        HandsFrame Convert(Frame leapFrame);
    }
}