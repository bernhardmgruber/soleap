using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WPF;
using SoLeap.Hand;
using Buffer = SharpDX.Direct3D11.Buffer;
using Matrix = BulletSharp.Matrix;

namespace SoLeap.Visualizer
{
    public sealed class GraphicsHand
        : System.IDisposable
    {
        private readonly Device device;

        private readonly PhysicsHand physicsHand;

        private Buffer vertexBuffer;

        private IList<Tuple<int, int>> shapeRanges;

        private ConstantBuffer<ObjectConstants> objectConstantsBuffer;

        public GraphicsHand(PhysicsHand physicsHand, Device device)
        {
            Contract.Requires(device != null);
            Contract.Requires(physicsHand != null);

            this.device = device;
            this.physicsHand = physicsHand;

            CreateVertexBuffer();

            objectConstantsBuffer = new ConstantBuffer<ObjectConstants>(device);
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
        }

        public void Render(Color3 color)
        {
            // Assume RenderStats and Shaders are set by the SceneRenderer

            var context = device.ImmediateContext;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, VertexPositionNormal.SizeInBytes, 0));

            context.VertexShader.SetConstantBuffer(1, objectConstantsBuffer.Buffer);
            context.PixelShader.SetConstantBuffer(1, objectConstantsBuffer.Buffer);

            var transformations = physicsHand.AllTransformations;
            Debug.Assert(shapeRanges.Count == transformations.Count);
            for (int i = 0; i < shapeRanges.Count; i++) {
                var world = transformations[i];
                int vertexOffset = shapeRanges[i].Item1;
                int vertexCount = shapeRanges[i].Item2 - vertexOffset;

                objectConstantsBuffer.Update(new ObjectConstants {
                    World = world,
                    WorldInverseTranspose = Matrix.Transpose(Matrix.Invert(world)),
                    Ambient = new Color3(color.ToVector3() / 5.0f),
                    Diffuse = color,
                    Specular = color,
                    SpecularPower = 30.0f
                });

                context.Draw(vertexCount, vertexOffset);
            }
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            objectConstantsBuffer.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}