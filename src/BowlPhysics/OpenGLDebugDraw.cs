using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BulletSharp;
using SharpGL;
using System.Diagnostics;

namespace BowlPhysics
{
    public class OpenGLDebugDraw : IDebugDraw
    {
        OpenGL gl;

        public DebugDrawModes DebugMode { get { return DebugDrawModes.MaxDebugDrawMode; } set { } }

        public OpenGLDebugDraw(OpenGL gl)
        {
            this.gl = gl;
        }

        public void Draw3dText(ref Vector3 location, string textString)
        {
            gl.RasterPos(location.X, location.Y, location.Z);
            Debug.WriteLine("Draw3dText");
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
            Debug.WriteLine("DrawAabb");
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
            Debug.WriteLine("DrawContactPoint");
        }

        public void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            gl.PushMatrix();
            gl.MultMatrix(transform.ToArray());
            IntPtr q = gl.NewQuadric();
            gl.Cylinder(q, radius, radius, 2 * halfHeight, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();
            Debug.WriteLine("DrawCylinder");
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color color)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(color.R, color.G, color.B);
            gl.Vertex(from.X, from.Y, from.Z);
            gl.Vertex(to.X, to.Y, to.Z);
            gl.End();
            Debug.WriteLine("DrawLine");
        }

        public void DrawLine(ref Vector3 from, ref Vector3 to, Color fromColor, Color toColor)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(fromColor.R, fromColor.G, fromColor.B);
            gl.Vertex(from.X, from.Y, from.Z);
            gl.Color(to.X, to.Y, to.Z);
            gl.Vertex(to.X, to.Y, to.Z);
            gl.End();
            Debug.WriteLine("DrawLine");
        }

        public void DrawPlane(ref Vector3 planeNormal, float planeConst, ref Matrix transform, Color color)
        {
            Debug.WriteLine("DrawPlane");
        }

        public void DrawSphere(ref Vector3 p, float radius, Color color)
        {
            gl.PushMatrix();
            gl.Translate(p.X, p.Y, p.Z);
            gl.Color(color.R, color.G, color.B);
            IntPtr q = gl.NewQuadric();
            gl.Sphere(q, radius, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();
        }

        public void DrawSphere(float radius, ref Matrix transform, Color color)
        {
            gl.PushMatrix();
            gl.MultMatrix(transform.ToArray());
            gl.Color(color.R, color.G, color.B);
            IntPtr q = gl.NewQuadric();
            gl.Sphere(q, radius, 10, 10);
            gl.DeleteQuadric(q);
            gl.PopMatrix();
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
            Debug.WriteLine("DrawTransform");
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
