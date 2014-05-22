using Leap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Device.Leap
{
    public class LeapDevice : Listener, IDevice
    {
        public event FrameReadyHandler FrameReady;

        private Controller controller;

        public LeapDevice()
        {
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
            if (FrameReady != null)
                FrameReady(ConvertFrame(c.Frame()));
        }

        private HandInputFrame ConvertFrame(Frame frame)
        {
            HandInputFrame input = new HandInputFrame();

            // TODO select hand
            Hand hand = frame.Hands.FirstOrDefault();

            if (hand != null)
            {
                input.HandDirection = new Vector3D(hand.Direction.x, hand.Direction.y, hand.Direction.z);
                input.PalmPosition = new Point3D(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
                input.PalmNormal = new Vector3D(hand.PalmNormal.x, hand.PalmNormal.y, hand.PalmNormal.z);

                // TODO logic for assigning tip positions
                for (int i = 0; i < hand.Fingers.Count && i < 5; i++)
                {
                    var pos = hand.Fingers[i].TipPosition;
                    input.TipPositions[i] = new Point3D(pos.x, pos.y, pos.z);
                }
            }

            return input;
        }

        new void Dispose()
        {
            base.Dispose();
            controller.RemoveListener(this);
            controller.Dispose();
        }
    }
}
