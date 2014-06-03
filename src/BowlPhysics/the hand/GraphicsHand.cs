using BulletSharp;
using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BowlPhysics
{
    public class GraphicsHand
    {
        private readonly PhysicsHand physicsHand;

        public GraphicsHand(PhysicsHand physicsHand)
        {
            this.physicsHand = physicsHand;
        }

        public void Render(OpenGL gl)
        {
            foreach (Tuple<CollisionShape, Matrix> shape in physicsHand.AllShapesWithTransformations)
            {
                gl.PushMatrix();

                var m = shape.Item2;

                // apply world transformation
                gl.MultMatrix(m.ToArray());

                if (shape.Item1 is BoxShape)
                {
                    var s = shape.Item1 as BoxShape;

                    float x2 = s.HalfExtentsWithMargin.X;
                    float y2 = s.HalfExtentsWithMargin.Y;
                    float z2 = s.HalfExtentsWithMargin.Z;

                    // render that box
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
                else if (shape.Item1 is ConvexTriangleMeshShape)
                {
                    var s = shape.Item1 as ConvexTriangleMeshShape;

                    DataStream stream;
                    int numVertes;
                    PhyScalarType type;
                    int vertexStride;
                    DataStream indexStream;
                    int indexStride;
                    int numFaces;
                    PhyScalarType indicesType;
                    s.MeshInterface.GetLockedReadOnlyVertexIndexData(out stream, out numVertes, out type, out vertexStride, out indexStream, out indexStride, out numFaces, out indicesType);

                    gl.Begin(BeginMode.Triangles);
                    for (int i = 0; i < numVertes; i++) {
                        long offset = stream.Position;
                        float v1 = stream.Read<float>();
                        float v2 = stream.Read<float>();
                        float v3 = stream.Read<float>();
                        gl.Vertex(v1, v2, v3);
                        stream.Position = offset + vertexStride;
                    }
                    gl.End();

                    s.MeshInterface.UnlockReadOnlyVertexData(0);
                }

                gl.PopMatrix();
            }
        }
    }
}
