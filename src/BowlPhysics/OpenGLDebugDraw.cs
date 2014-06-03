using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BulletSharp;
using SharpGL;
using System.Diagnostics;
using SharpGL.Enumerations;

namespace BowlPhysics
{
    public class OpenGLDebugDraw : IDebugDraw
    {
        OpenGL gl;

        public DebugDrawModes DebugMode { get; set;}

        public OpenGLDebugDraw(OpenGL gl)
        {
            this.gl = gl;

            DebugMode = DebugDrawModes.MaxDebugDrawMode;

            gl.Enable(OpenGL.GL_LIGHT0);
            //gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
        }

        public void Draw3dText(ref Vector3 location, string textString)
        {
            // project 3D position to 2D window location
            double[] modelViewMatrix = new double[16];
            gl.GetDouble(GetTarget.ModelviewMatix, modelViewMatrix);
            double[] projMatrix = new double[16];
            gl.GetDouble(GetTarget.ProjectionMatrix, projMatrix);

            int[] viewport = new int[4];
            gl.GetInteger(GetTarget.Viewport, viewport);

            double[] x = new double[1];
            double[] y = new double[1];
            double[] z = new double[1];
            gl.Project(location.X, location.Y, location.Z, modelViewMatrix, projMatrix, viewport, x, y, z);

            // raster text
            gl.DrawText(Convert.ToInt32(x), Convert.ToInt32(y), 1.0f, 1.0f, 1.0f, "Arial", 10, textString);
        }

        public void DrawAabb(ref Vector3 from, ref Vector3 to, Color color)
        {
            gl.Color(color.R, color.G, color.B);
            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex(from.X, from.Y, from.Z);
            gl.Vertex(from.X, from.Y, to.Z);

            gl.Vertex(from.X, to.Y, from.Z);
            gl.Vertex(from.X, to.Y, to.Z);

            gl.Vertex(to.X, from.Y, from.Z);
            gl.Vertex(to.X, from.Y, to.Z);

            gl.Vertex(to.X, to.Y, from.Z);
            gl.Vertex(to.X, to.Y, to.Z);

            gl.Vertex(from.X, from.Y, from.Z);
            gl.Vertex(from.X, to.Y, from.Z);

            gl.Vertex(from.X, from.Y, to.Z);
            gl.Vertex(from.X, to.Y, to.Z);

            gl.Vertex(to.X, from.Y, from.Z);
            gl.Vertex(to.X, to.Y, from.Z);

            gl.Vertex(to.X, from.Y, to.Z);
            gl.Vertex(to.X, to.Y, to.Z);

            gl.Vertex(to.X, from.Y, from.Z);
            gl.Vertex(to.X, from.Y, from.Z);

            gl.Vertex(to.X, to.Y, from.Z);
            gl.Vertex(to.X, to.Y, from.Z);

            gl.Vertex(to.X, from.Y, to.Z);
            gl.Vertex(to.X, from.Y, to.Z);

            gl.Vertex(to.X, to.Y, to.Z);
            gl.Vertex(to.X, to.Y, to.Z);

            gl.End();
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color color, bool drawSect, float stepDegrees)
        {
            Debug.WriteLine("DrawArc");
        }

        public void DrawArc(ref Vector3 center, ref Vector3 normal, ref Vector3 axis, float radiusA, float radiusB, float minAngle, float maxAngle, Color color, bool drawSect)
        {
            Debug.WriteLine("DrawArc");
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix trans, Color color)
        {
            gl.PushMatrix();
            gl.MultMatrix(trans.ToArray());
            DrawBox(ref bbMin, ref bbMax, color);
            gl.PopMatrix();
        }

        public void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, Color color)
        {
            var x2 = (bbMax.X - bbMin.X) / 2.0f;
            var y2 = (bbMax.Y - bbMin.Y) / 2.0f;
            var z2 = (bbMax.Z - bbMin.Z) / 2.0f;

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.Color(color.R, color.G, color.B);
            gl.Begin(OpenGL.GL_QUADS);
            /* f1: front */
            gl.Normal(0, -1, 0);
            gl.Vertex(-x2, -y2, -z2);
            gl.Vertex(-x2, -y2, +z2);
            gl.Vertex(+x2, -y2, +z2);
            gl.Vertex(+x2, -y2, -z2);
            /* f2: bottom */
            gl.Normal(0, 0, -1);
            gl.Vertex(-x2, -y2, -z2);
            gl.Vertex(+x2, -y2, -z2);
            gl.Vertex(+x2, +y2, -z2);
            gl.Vertex(-x2, +y2, -z2);
            /* f3:back */
            gl.Normal(0, 1, 0);
            gl.Vertex(+x2, +y2, -z2);
            gl.Vertex(+x2, +y2, +z2);
            gl.Vertex(-x2, +y2, +z2);
            gl.Vertex(-x2, +y2, -z2);
            /* f4: top */
            gl.Normal(0, 0, 1);
            gl.Vertex(+x2, +y2, +z2);
            gl.Vertex(+x2, -y2, +z2);
            gl.Vertex(-x2, -y2, +z2);
            gl.Vertex(-x2, +y2, +z2);
            /* f5: left */
            gl.Normal(-1, 0, 0);
            gl.Vertex(-x2, -y2, -z2);
            gl.Vertex(-x2, +y2, -z2);
            gl.Vertex(-x2, +y2, +z2);
            gl.Vertex(-x2, -y2, +z2);
            /* f6: right */
            gl.Normal(1, 0, 0);
            gl.Vertex(+x2, -y2, -z2);
            gl.Vertex(+x2, -y2, +z2);
            gl.Vertex(+x2, +y2, +z2);
            gl.Vertex(+x2, +y2, -z2);
            gl.End();

            gl.Disable(OpenGL.GL_LIGHTING);
        }

        public void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            Debug.WriteLine("DrawCapsule");
        }

        public void DrawCone(float radius, float height, int upAxis, ref Matrix transform, Color color)
        {
            Debug.WriteLine("DrawCone");
        }

        public void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, Color color)
        {
            Vector3 from = pointOnB;
            Vector3 to = pointOnB + normalOnB * distance;

            gl.Color(color.R, color.G, color.B);
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(from.X, from.Y, from.Z);
            gl.Vertex(to.X, to.Y, to.Z);
            gl.End();
        }

        public void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            gl.PushMatrix();
            gl.MultMatrix(transform.ToArray());
            IntPtr q = gl.NewQuadric();
            gl.QuadricNormals(q, OpenGL.GLU_SMOOTH);
            gl.Cylinder(q, radius, radius, 2 * halfHeight, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color color)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(color.R, color.G, color.B);
            gl.Vertex(from.X, from.Y, from.Z);
            gl.Vertex(to.X, to.Y, to.Z);
            gl.End();
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color fromColor, Color toColor)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(fromColor.R, fromColor.G, fromColor.B);
            gl.Vertex(from.X, from.Y, from.Z);
            gl.Color(to.X, to.Y, to.Z);
            gl.Vertex(to.X, to.Y, to.Z);
            gl.End();
        }

        public void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix transform, Color color)
        {
            Debug.WriteLine("DrawPlane");
        }

        public void DrawSphere(ref Vector3 p, float radius, Color color)
        {
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.PushMatrix();
            gl.Translate(p.X, p.Y, p.Z);
            gl.Color(color.R, color.G, color.B);
            IntPtr q = gl.NewQuadric();
            gl.QuadricNormals(q, OpenGL.GLU_SMOOTH);
            gl.Sphere(q, radius, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_LIGHTING);
        }

        public void DrawSphere(float radius, ref Matrix transform, Color color)
        {
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            gl.PushMatrix();
            gl.MultMatrix(transform.ToArray());
            gl.Color(color.R, color.G, color.B);
            IntPtr q = gl.NewQuadric();
            gl.QuadricNormals(q, OpenGL.GLU_SMOOTH);
            gl.Sphere(q, radius, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_LIGHTING);

            gl.GetError(); // BUG, sphere does not appear without this =(
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color color, float stepDegrees, bool drawSphere)
        {
            Debug.WriteLine("DrawSpherePatch");
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color color, float stepDegrees)
        {
            Debug.WriteLine("DrawSpherePatch");
        }

        public void DrawSpherePatch(ref Vector3 center, ref Vector3 up, ref Vector3 axis, float radius, float minTh, float maxTh, float minPs, float maxPs, Color color)
        {
            Debug.WriteLine("DrawSpherePatch");
        }

        public void DrawTransform(ref Matrix transform, float orthoLen)
        {
            gl.PushMatrix();
            gl.MultMatrix(transform.ToArray());

            float l = orthoLen * 10;

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

            gl.PopMatrix();
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, Color color, float __unnamed004)
        {
            Debug.WriteLine("DrawTriangle");
        }

        public void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 __unnamed003, ref Vector3 __unnamed004, ref Vector3 __unnamed005, Color color, float alpha)
        {

            Debug.WriteLine("DrawTriangle");
        }

        public void ReportErrorWarning(string warningString)
        {
            Console.WriteLine("Bullet: " + warningString);
        }
    }
}
