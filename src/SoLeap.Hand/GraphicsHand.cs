using BulletSharp;
using SoLeap.Domain;

using System.Linq;
using SharpDX;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using System;

namespace SoLeap.Hand
{
    public sealed class GraphicsHand : System.IDisposable
    {
        private readonly PhysicsHand physicsHand;

        private SharpDX.Direct3D11.Device device;

        private IList<Tuple<int, int>> shapeRanges;
        private SharpDX.Direct3D11.Buffer vertexBuffer;

        private struct Vertex
        {
            public Vertex(SharpDX.Vector3 normal, SharpDX.Vector3 position)
            {
                this.normal = normal;
                this.position = position;
            }

            SharpDX.Vector3 normal;
            SharpDX.Vector3 position;
        }

        public GraphicsHand(SharpDX.Direct3D11.Device device, PhysicsHand physicsHand)
        {
            this.device = device;
            this.physicsHand = physicsHand;

            SetupRessources();
        }

        private void EmitBoxVertices(IList<Vertex> vertices, float xExtentHalf, float yExtentHalf, float zExtentHalf)
        {
            var x2 = xExtentHalf;
            var y2 = yExtentHalf;
            var z2 = zExtentHalf;

            SharpDX.Vector3 normal;

            // side 1
            normal = new SharpDX.Vector3(0, 0, -1);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, -z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, -z2)));

            // side 2
            normal = new SharpDX.Vector3(-1, 0, 0);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, +z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, -z2)));

            // side 3
            normal = new SharpDX.Vector3(0, 0, +1);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, +z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, +z2)));

            // side 4
            normal = new SharpDX.Vector3(+1, 0, 0);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, +z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, +z2)));

            // side 5
            normal = new SharpDX.Vector3(0, +1, 0);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, -z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, +y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, +y2, -z2)));

            // side 6
            normal = new SharpDX.Vector3(0, -1, 0);
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, -z2)));

            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, -z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(+x2, -y2, +z2)));
            vertices.Add(new Vertex(normal, new SharpDX.Vector3(-x2, -y2, +z2)));
        }

        private void SetupRessources()
        {
            IList<Vertex> vertices = new List<Vertex>();
            shapeRanges = new List<Tuple<int, int>>();

            // place all shapes adjacently into the vertex buffer
            foreach (var shape in physicsHand.AllShapes)
            {
                var start = vertices.Count;

                if (shape is BoxShape)
                {
                    var s = shape as BoxShape;

                    var ex = s.HalfExtentsWithoutMargin;
                    
                    EmitBoxVertices(vertices, ex.X, ex.Y, ex.Z);
                }
                else if (shape is ConvexTriangleMeshShape)
                {
                    var s = shape as ConvexTriangleMeshShape;

                    BulletSharp.DataStream stream;
                    int numVertes;
                    PhyScalarType type;
                    int vertexStride;
                    BulletSharp.DataStream indexStream;
                    int indexStride;
                    int numFaces;
                    PhyScalarType indicesType;
                    s.MeshInterface.GetLockedReadOnlyVertexIndexData(out stream, out numVertes, out type, out vertexStride, out indexStream, out indexStride, out numFaces, out indicesType);

                    for (int i = 0; i < numFaces; i++)
                    {
                        var positions = new SharpDX.Vector3[3];

                        for (int j = 0; j < 3; j++)
                        {
                            long offset = stream.Position;
                            float v1 = stream.Read<float>();
                            float v2 = stream.Read<float>();
                            float v3 = stream.Read<float>();
                            stream.Position = offset + vertexStride;
                            positions[j] = new SharpDX.Vector3(v1, v2, v3);
                        }

                        var a = positions[0] - positions[1];
                        var b = positions[2] - positions[1];

                        var normal = SharpDX.Vector3.Cross(a, b);

                        for (int j = 0; j < 3; j++)
                            vertices.Add(new Vertex(normal, positions[j]));
                    }

                    s.MeshInterface.UnlockReadOnlyVertexData(0);
                }

                shapeRanges.Add(new Tuple<int, int>(start, vertices.Count));
            }

            // now create the buffer
            using (var stream = new SharpDX.DataStream(vertices.Count * Marshal.SizeOf(typeof(Vertex)), true, true))
            {
                stream.WriteRange(vertices.ToArray()); // FIXME, prevent copy
                vertexBuffer = new SharpDX.Direct3D11.Buffer(device, stream, new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    SizeInBytes = (int)stream.Length,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    StructureByteStride = 0,
                    Usage = ResourceUsage.Default
                });
            }
        }

        private void CleanupRessources()
        {
            vertexBuffer.Dispose();
        }

        public void Update(Domain.Hand hand)
        {
            physicsHand.Update(hand);

            // transformations have changed

            // TODO
        }

        public void Render() // TODO
        {
            // TODO


            device.ImmediateContext.Draw(shapeRanges.Last().Item2, 0);
        }

        public void Dispose()
        {
            CleanupRessources();
            GC.SuppressFinalize(this);
        }
    }
}