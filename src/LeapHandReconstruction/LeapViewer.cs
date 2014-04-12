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

        private bool tracking = false;
        private Point lastMousePos;
        private float xrot, yrot, zoom = -500.0f;

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
            gl.Perspective(60.0, (double)glControl.Width / (double)glControl.Height, 0.1, 2000.0);
            gl.MatrixMode(MatrixMode.Modelview);
        }

        private void DrawBox(OpenGL gl, float xlen, float ylen, float zlen)
        {
            float xlen2 = xlen / 2.0f;
            float ylen2 = ylen / 2.0f;
            float zlen2 = zlen / 2.0f;

            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex(-xlen2, ylen2, zlen2);
            gl.Vertex(xlen2, ylen2, zlen2);

            gl.Vertex(-xlen2, -ylen2, zlen2);
            gl.Vertex(xlen2, -ylen2, zlen2);

            gl.Vertex(-xlen2, ylen2, -zlen2);
            gl.Vertex(xlen2, ylen2, -zlen2);

            gl.Vertex(-xlen2, -ylen2, -zlen2);
            gl.Vertex(xlen2, -ylen2, -zlen2);

            gl.Vertex(xlen2, -ylen2, zlen2);
            gl.Vertex(xlen2, ylen2, zlen2);

            gl.Vertex(-xlen2, -ylen2, zlen2);
            gl.Vertex(-xlen2, ylen2, zlen2);

            gl.Vertex(xlen2, -ylen2, -zlen2);
            gl.Vertex(xlen2, ylen2, -zlen2);

            gl.Vertex(-xlen2, -ylen2, -zlen2);
            gl.Vertex(-xlen2, ylen2, -zlen2);

            gl.Vertex(xlen2, ylen2, -zlen2);
            gl.Vertex(xlen2, ylen2, zlen2);

            gl.Vertex(-xlen2, ylen2, -zlen2);
            gl.Vertex(-xlen2, ylen2, zlen2);

            gl.Vertex(xlen2, -ylen2, -zlen2);
            gl.Vertex(xlen2, -ylen2, zlen2);

            gl.Vertex(-xlen2, -ylen2, -zlen2);
            gl.Vertex(-xlen2, -ylen2, zlen2);

            gl.End();
        }

        private void DrawCube(OpenGL gl, float l = 1.0f)
        {
            DrawBox(gl, l, l, l);
        }

        private void DrawAxes()
        {
            float l = 500;
            gl.Begin(BeginMode.Lines);
            gl.Color(1.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(l, 0.0f, 0.0f);
            gl.Color(0.0f, l, 0.0f);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, l, 0.0f);
            gl.Color(0.0f, 0.0f, l);
            gl.Vertex(0.0f, 0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f, l);
            gl.End();
        }

        private float radToDeg(float rad)
        {
            return rad * (float)180.0 / (float)Math.PI;
        }

        private void glControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            var frame = (Frame)glControl.Tag;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, zoom);
            gl.Rotate(xrot, 1.0f, 0.0f, 0.0f);
            gl.Rotate(yrot, 0.0f, 1.0f, 0.0f);

            // floor
            gl.Color(0.4, 0.4, 0.4);
            gl.Begin(BeginMode.Quads);
            gl.Vertex(-500, 0, +500);
            gl.Vertex(+500, 0, +500);
            gl.Vertex(+500, 0, -500);
            gl.Vertex(-500, 0, -500);
            gl.End();

            // coord system
            DrawAxes();

            // leap data
            gl.PointSize(5.0f);
            gl.Begin(BeginMode.Points);
            foreach (var hand in frame.Hands)
            {
                gl.Color(0.5, 0.0, 0.0);
                gl.Vertex(hand.PalmPosition.ToFloatArray());
                gl.Color(1.0, 0.0, 0.0);
                gl.Vertex(hand.StabilizedPalmPosition.ToFloatArray());

                foreach (var finger in hand.Fingers)
                {
                    gl.Color(0.0, 0.5, 0.0);
                    gl.Vertex(finger.TipPosition.ToFloatArray());
                    gl.Color(0.0, 1.0, 0.0);
                    gl.Vertex(finger.StabilizedTipPosition.ToFloatArray());
                }

                foreach (var tool in hand.Tools)
                {
                    gl.Color(0.0, 0.0, 0.5);
                    gl.Vertex(tool.TipPosition.ToFloatArray());
                    gl.Color(0.0, 0.0, 1.0);
                    gl.Vertex(tool.StabilizedTipPosition.ToFloatArray());
                }
            }
            gl.End();

            gl.Begin(BeginMode.Lines);
            foreach (var hand in frame.Hands)
            {
                gl.Color(0.75, 0.0, 0.0);
                gl.Vertex(hand.PalmPosition.ToFloatArray());
                gl.Vertex((hand.PalmPosition + hand.PalmNormal * 100.0f).ToFloatArray());

                gl.Vertex(hand.PalmPosition.ToFloatArray());
                gl.Vertex((hand.PalmPosition + hand.Direction * 100.0f).ToFloatArray());

                gl.Color(0.0, 0.75, 0.0);
                foreach (var finger in hand.Fingers)
                {
                    gl.Vertex(finger.TipPosition.ToFloatArray());
                    gl.Vertex((finger.TipPosition - finger.Direction * finger.Length).ToFloatArray());
                }

                gl.Color(0.0, 0.0, 0.75);
                foreach (var tool in hand.Tools)
                {
                    gl.Vertex(tool.TipPosition.ToFloatArray());
                    gl.Vertex((tool.TipPosition - tool.Direction * tool.Length).ToFloatArray());
                }
            }
            gl.End();

            // the hand bvh
            foreach (var hand in frame.Hands)
            {
                gl.PushMatrix();

                var pos = hand.PalmPosition;
                //gl.Translate(pos.x, pos.y, pos.z);
                //gl.LoadIdentity();
                //gl.Translate(0, 0, -500);

                var eye = -hand.Direction;
                //var eye = new Leap.Vector(1, 0, 0);
                Console.WriteLine(eye.ToString());
                var up = Vector.YAxis;
                gl.LookAt(eye.x, eye.y, eye.z, 0.0f, 0.0f, 0.0f, up.y, up.y, up.z);
                DrawCube(gl, 100);

                gl.PopMatrix();
            }


        }



        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (tracking)
            {
                Point curMousePos = e.Location;

                float deltaX = (float)(curMousePos.X - lastMousePos.X);
                float deltaY = (float)(curMousePos.Y - lastMousePos.Y);

                lastMousePos = curMousePos;

                xrot += deltaY;
                yrot += deltaX;
            }
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            tracking = true;
            lastMousePos = e.Location;
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            tracking = false;
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            zoom += e.Delta;
        }

    }
}
