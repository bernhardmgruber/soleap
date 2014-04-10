using Leap;
using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LeapHandReconstruction
{
    public partial class LeapViewer : Form
    {
        private OpenGL gl;

        public LeapViewer()
        {
            InitializeComponent();
        }

        internal void DrawFrame(Frame frame)
        {
            glControl.Tag = frame; // FIXME: not very elegant to pass the frame to the render call
            glControl.DoRender();
        }

        private void glControl_OpenGLInitialized(object sender, EventArgs e)
        {
            gl = glControl.OpenGL;
        }

        private void glControl_Resized(object sender, EventArgs e)
        {
            gl.MatrixMode(MatrixMode.Projection);
            gl.LoadIdentity();
            gl.Perspective(60.0, (double)glControl.Width / (double)glControl.Height, 0.1, 1000.0);
            gl.MatrixMode(MatrixMode.Modelview);
        }

        private void glControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            var frame = (Frame)glControl.Tag;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0, 0, -500.0f);

            gl.PointSize(5.0f);

            gl.Begin(BeginMode.Points);
            foreach (var hand in frame.Hands)
            {
                gl.Vertex(hand.PalmPosition.ToFloatArray());
                foreach (var finger in hand.Fingers)
                    gl.Vertex(finger.TipPosition.ToFloatArray());
            }
            gl.End();

            gl.Begin(BeginMode.Lines);
            foreach (var hand in frame.Hands)
            {
                foreach (var finger in hand.Fingers)
                {
                    gl.Vertex(hand.PalmPosition.ToFloatArray());
                    gl.Vertex(finger.TipPosition.ToFloatArray());
                }
            }
            gl.End();
        }

    }
}
