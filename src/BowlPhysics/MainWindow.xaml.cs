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

namespace BowlPhysics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PhysicsWorld world;

        bool tracking = false;
        Point lastMousePos;

        float xrot = 0.0f;
        float yrot = 0.0f;
        float zoom = -20.0f;

        RigidBody userBox;

        public MainWindow(PhysicsWorld world)
        {
            InitializeComponent();
            this.world = world;

            // create user box
            // TODO move this code somewhere into the physics code
            var shape = new BoxShape(0.5f);
            world.CollisionShapes.Add(shape);
            userBox = world.CreateRigidBody(1.0f, BulletSharp.Matrix.Translation(0.0f, 4.0f, 0.0f), shape, "user box");
            userBox.Gravity = Vector3.Zero;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.ShadeModel(ShadeModel.Smooth);

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

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            var gl = args.OpenGL;

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
            world.Dispose();
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
