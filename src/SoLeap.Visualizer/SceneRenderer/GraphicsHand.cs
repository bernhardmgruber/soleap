using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using BulletSharp;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.WPF;
using SoLeap.Hand;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SoLeap.Visualizer
{
    public sealed class GraphicsHand
        : System.IDisposable
    {
        private readonly Device device;

        private readonly PhysicsHand physicsHand;

        private Buffer vertexBuffer;

        private IList<Tuple<int, int>> shapeRanges;
        private IList<SharpDX.Matrix> transformations;

        public GraphicsHand(PhysicsHand physicsHand, Device device)
        {
            Contract.Requires(device != null);
            Contract.Requires(physicsHand != null);

            this.device = device;
            this.physicsHand = physicsHand;

            CreateVertexBuffer();
            Update(hand);
        }

        private void CreateVertexBuffer()
        {
            var vertices = new List<VertexPositionNormal>();
            shapeRanges = new List<Tuple<int, int>>();

            // place all shapes adjacently into the vertex buffer
            foreach (var shape in physicsHand.AllShapes) {
                var start = vertices.Count;

                var shapeVertices = CollisionShapeConverter.GetVertices(shape);

                vertices.AddRange(shapeVertices);
                shapeRanges.Add(new Tuple<int, int>(start, vertices.Count));
            }

            vertexBuffer = device.CreateBuffer(vertices.ToArray());
        }

        public void Update(Domain.Hand hand)
        {
            Contract.Requires(hand != null);

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
            for (int i = 0; i < shapeRanges.Count; i++) {
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
            vertexBuffer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}