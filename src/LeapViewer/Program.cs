using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeapHandReconstruction
{
    class Program
    {
        private static LeapViewer viewer;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var leap = new LeapMotionDevice()) {
                viewer = new LeapViewer();
                leap.OnFrame += leap_OnFrame;
                Application.Run(viewer);
            }
        }

        static void leap_OnFrame(Frame frame)
        {
            viewer.Invoke(new Action(() => {
                Console.Clear();
                Console.WriteLine("Frame");
                foreach (var hand in frame.Hands) {
                    Console.WriteLine("    Hand");
                    foreach (var finger in hand.Fingers)
                        Console.WriteLine("        Finger (" + finger.Type() + ") " + finger.TipPosition);
                    foreach (var tool in hand.Tools)
                        Console.WriteLine("        Tool " + tool.TipPosition.ToString());
                }

                viewer.DrawFrame(frame);
            }));
        }
    }
}
