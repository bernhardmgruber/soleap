using BulletSharp;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SoLeap.Worlds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace SoLeap.Hand
{
    public sealed class GraphicsHand : System.IDisposable
    {
        private readonly PhysicsHand physicsHand;

        private SharpDX.Direct3D11.Device device;

        private IList<Tuple<int, int>> shapeRanges;
        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private SharpDX.Direct3D11.Buffer matrixBuffer;
        private SharpDX.Direct3D11.InputLayout inputLayout;
        private IList<SharpDX.Matrix> transformations;

        private struct Vertex
        {
            public Vertex(SharpDX.Vector3 normal, SharpDX.Vector3 position)
            {
                this.normal = normal;
                this.position = position;
            }

            private SharpDX.Vector3 normal;
            private SharpDX.Vector3 position;
        }

        public GraphicsHand(SharpDX.Direct3D11.Device device, byte[] inputSignature, IPhysicsWorld world, Domain.Hand hand)
        {
            this.device = device;
            this.physicsHand = new PhysicsHand(world, hand);

            inputLayout = new InputLayout(device, inputSignature, new[] {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                    new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0)
                });

            SetupRessources();
            Update(hand);
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

            // create constant buffer for the current transformation matrix
            matrixBuffer = new SharpDX.Direct3D11.Buffer(device, new BufferDescription
            {
                BindFlags = BindFlags.ConstantBuffer,
                SizeInBytes = Marshal.SizeOf(typeof(SharpDX.Matrix)),
                CpuAccessFlags = CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = 0,
                Usage = ResourceUsage.Dynamic
            });
        }

        private void CleanupRessources()
        {
            vertexBuffer.Dispose();
            matrixBuffer.Dispose();
            inputLayout.Dispose();
        }

        public void Update(Domain.Hand hand)
        {
            physicsHand.Update(hand);

            // transformations have changed
            transformations = physicsHand.AllTransformations.Select(m => new SharpDX.Matrix(m.ToArray())).ToList();
        }

        public void Render()
        {
            var context = device.ImmediateContext;
            context.InputAssembler.InputLayout = inputLayout;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, 24, 0));
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            Debug.Assert(shapeRanges.Count == transformations.Count);
            for (int i = 0; i < shapeRanges.Count; i++)
            {
                SharpDX.Matrix trans = transformations[i];
                int vertexOffset = shapeRanges[i].Item1;
                int vertexCount = shapeRanges[i].Item2 - vertexOffset;

                SharpDX.DataStream stream;
                context.MapSubresource(matrixBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out stream);
                stream.Write(trans);
                context.UnmapSubresource(matrixBuffer, 0);

                context.Draw(vertexCount, vertexOffset);
            }
        }

        public void Dispose()
        {
            CleanupRessources();
            GC.SuppressFinalize(this);
        }
    }
}