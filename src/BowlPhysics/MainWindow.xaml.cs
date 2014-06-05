using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SoLeap.Device;
using SoLeap.Hand;
using SoLeap.Worlds;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BowlPhysics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPhysicsWorld world;
        private IHandsFrameProvider handsProvider;

        private bool tracking = false;
        private Point lastMousePos;

        private float xrot = 0.0f;
        private float yrot = 0.0f;
        private float zoom = -500.0f;

        private IDictionary<long, GraphicsHand> hands = new Dictionary<long, GraphicsHand>();

        private HandsFrame lastFrame = new HandsFrame();

        public MainWindow(IPhysicsWorld world, IHandsFrameProvider handsProvider)
        {
            InitializeComponent();
            this.world = world;
            this.handsProvider = handsProvider;

            handsProvider.FrameReady += handsProvider_FrameReady;
        }

        private void handsProvider_FrameReady(object sender, HandsFrame e)
        {
            lock (lastFrame)
                lastFrame = e;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ShadeModel(ShadeModel.Smooth);
            //openglInitialized = true;
        }

        private void glControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            Update();
            Render(args.OpenGL);
        }

        private void DrawAxes(OpenGL gl)
        {
            float l = 200.0f;
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

        private void Update()
        {
            world.Update();

            lock (lastFrame)
            {
                IDictionary<long, GraphicsHand> newHands = new Dictionary<long, GraphicsHand>();
                foreach (var hand in lastFrame.Hands)
                {
                    GraphicsHand gh;
                    if (hands.TryGetValue(hand.Id, out gh))
                    {
                        // this hand existed in the last frame, update it
                        gh.Update(hand);
                        hands.Remove(hand.Id);
                    }
                    else
                    {
                        // this hand is new, create it
                        gh = new GraphicsHand(new PhysicsHand(world, hand));
                    }
                    newHands[hand.Id] = gh;
                }

                // remove missing hands
                foreach (var missingHands in hands.Values)
                    missingHands.Dispose();

                hands = newHands;
            }
        }

        private void Render(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_COLOR_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Translate(0.0f, 0.0f, zoom);
            gl.Rotate(xrot, 1.0f, 0.0f, 0.0f);
            gl.Rotate(yrot, 0.0f, 1.0f, 0.0f);

            DrawAxes(gl);

            //gl.Enable(OpenGL.GL_LIGHTING);
            //gl.Enable(OpenGL.GL_LIGHT0);
            //gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines); // wireframe
            gl.Color(1.0f, 0.7f, 0.0f);

            foreach(var hand in hands.Values)
                hand.Render(gl);

            //gl.Disable(OpenGL.GL_LIGHTING);

            world.DebugDrawer = new OpenGLDebugDraw(gl);
            //gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            world.DebugDraw();
            //gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            handsProvider.FrameReady -= handsProvider_FrameReady;
            world.Dispose();
            //hasShutDown = true;
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            var width = glControl.ActualWidth;
            var height = glControl.ActualHeight;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double aspect = (double)width / (double)Math.Max(height, 1.0);

            gl.Perspective(45.0, aspect, 0.1, 3000.0);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //gl.Viewport(0, 0, width, height); // done by OpenGLControl
        }

        private void OpenGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (tracking)
            {
                Point curMousePos = e.GetPosition(this);

                float deltaX = (float)(curMousePos.X - lastMousePos.X);
                float deltaY = (float)(curMousePos.Y - lastMousePos.Y);

                lastMousePos = curMousePos;

                xrot += deltaY;
                yrot += deltaX;
            }
        }

        private void OpenGLControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tracking = true;
            lastMousePos = e.GetPosition(this);
        }

        private void OpenGLControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            tracking = false;
        }

        private void OpenGLControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            zoom += e.Delta / 120 * 10;
        }

        //private void Window_KeyDown(object sender, KeyEventArgs e)
        //{
        //    const float impuls = 4.0f;

        //    switch (e.Key)
        //    {
        //        case Key.A:
        //            userBox.ApplyCentralImpulse(new Vector3(-impuls, 0, 0));
        //            break;
        //        case Key.D:
        //            userBox.ApplyCentralImpulse(new Vector3(+impuls, 0, 0));
        //            break;
        //        case Key.W:
        //            userBox.ApplyCentralImpulse(new Vector3(0, 0, -impuls));
        //            break;
        //        case Key.S:
        //            userBox.ApplyCentralImpulse(new Vector3(0, 0, +impuls));
        //            break;
        //        case Key.LeftCtrl:
        //            userBox.ApplyCentralImpulse(new Vector3(0, -impuls, 0));
        //            break;
        //        case Key.Space:
        //            userBox.ApplyCentralImpulse(new Vector3(0, +impuls, 0));
        //            break;
        //    }

        //}
    }
}