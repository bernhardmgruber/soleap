using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletSharp;
using SharpGL.SceneGraph;
using SharpGL.Enumerations;
using SoLeap.Device;
using SharpGL.WPF;
using System.Diagnostics;

namespace BowlPhysics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PhysicsWorld world;
        IHandsFrameProvider handsProvider;

        bool tracking = false;
        Point lastMousePos;

        float xrot = 0.0f;
        float yrot = 0.0f;
        float zoom = -20.0f;

        RigidBody userBox;

        public MainWindow(PhysicsWorld world, IHandsFrameProvider handsProvider)
        {
            InitializeComponent();
            this.world = world;
            this.handsProvider = handsProvider;

            handsProvider.FrameReady += handsProvider_FrameReady;

            // create user box
            // TODO move this code somewhere into the physics code
            var shape = new BoxShape(0.5f);
            world.CollisionShapes.Add(shape);
            userBox = world.CreateRigidBody(1.0f, BulletSharp.Matrix.Translation(new Vector3(0.0f, 4.0f, 0.0f)), shape, "user box", true);
        }

        bool openglInitialized = false;
        bool hasShutDown = false;

        Vector3? lastLeapPosition;

        void handsProvider_FrameReady(object sender, HandsFrame e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (hasShutDown)
                    return;

                if (e.Hands.Count() > 0)
                {
                    var palm = e.Hands.First().PalmPosition;

                    var newPos = new Vector3((float)palm.X / 10.0f, (float)palm.Y / 10.0f, (float)palm.Z / 10.0f);
                    newPos.Y -= 20;

                    //Debug.WriteLine("got HandsFrame " + newPos);

                    if (lastLeapPosition.HasValue)
                    {
                        var translate = newPos - lastLeapPosition.Value;

                        // transform according to view
                        var xRotMatrix = BulletSharp.Matrix.RotationX(-xrot * (float)Math.PI / 180.0f);
                        var yRotMatrix = BulletSharp.Matrix.RotationY(-yrot * (float)Math.PI / 180.0f);

                        var rotMatrix = BulletSharp.Matrix.Multiply(xRotMatrix, yRotMatrix);

                        var transformedTranslate4 = BulletSharp.Vector3.Transform(translate, rotMatrix);
                        var transformedTranslate3 = new Vector3(transformedTranslate4.X, transformedTranslate4.Y, transformedTranslate4.Z);

                        //Debug.WriteLine("translate " + transformedTranslate3);
                        
                        //userBox.Translate(transformedTranslate3);
                        var translationMatrix = BulletSharp.Matrix.Translation(transformedTranslate3);
                        userBox.MotionState.WorldTransform = BulletSharp.Matrix.Multiply(userBox.MotionState.WorldTransform, translationMatrix);
                    }

                    lastLeapPosition = newPos;
                }

                if (openglInitialized)
                    Render(glControl.OpenGL);
            }));
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ShadeModel(ShadeModel.Smooth);
            openglInitialized = true;
        }

        private void DrawAxes(OpenGL gl)
        {
            float l = 10.0f;
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

        private void Render(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_COLOR_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Translate(0.0f, 0.0f, zoom);
            gl.Rotate(xrot, 1.0f, 0.0f, 0.0f);
            gl.Rotate(yrot, 0.0f, 1.0f, 0.0f);

            DrawAxes(gl);

            world.Update();

            world.DebugDrawer = new OpenGLDebugDraw(gl);
            world.DebugDraw();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            handsProvider.FrameReady -= handsProvider_FrameReady;
            world.Dispose();
            hasShutDown = true;
        }

        private void OpenGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            var width = glControl.ActualWidth;
            var height = glControl.ActualHeight;

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double aspect = (double)width / (double)Math.Max(height, 1.0);

            gl.Perspective(45.0, aspect, 0.1, 100.0);

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
            zoom += e.Delta / 120;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            const float impuls = 4.0f;

            switch (e.Key)
            {
                case Key.A:
                    userBox.ApplyCentralImpulse(new Vector3(-impuls, 0, 0));
                    break;
                case Key.D:
                    userBox.ApplyCentralImpulse(new Vector3(+impuls, 0, 0));
                    break;
                case Key.W:
                    userBox.ApplyCentralImpulse(new Vector3(0, 0, -impuls));
                    break;
                case Key.S:
                    userBox.ApplyCentralImpulse(new Vector3(0, 0, +impuls));
                    break;
                case Key.LeftCtrl:
                    userBox.ApplyCentralImpulse(new Vector3(0, -impuls, 0));
                    break;
                case Key.Space:
                    userBox.ApplyCentralImpulse(new Vector3(0, +impuls, 0));
                    break;
            }

        }
    }
}
