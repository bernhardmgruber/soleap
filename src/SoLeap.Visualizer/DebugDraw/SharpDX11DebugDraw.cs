using System.Diagnostics.Contracts;
using BulletSharp;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DataStream = global::SharpDX.DataStream;
using Device = SharpDX.Direct3D11.Device;

namespace SoLeap.Visualizer.DebugDraw
{
    public class SharpDX11DebugDraw : BufferedDebugDraw
    {
        private readonly SharpDX.Direct3D11.Device device;
        private readonly InputAssemblerStage inputAssembler;
        private readonly InputLayout inputLayout;
        private BufferDescription vertexBufferDesc;
        private PositionColored[] lineArray;
        private Buffer vertexBuffer;
        private VertexBufferBinding vertexBufferBinding;

        public SharpDX11DebugDraw(DynamicsWorld world, Device device, byte[] shaderByteCode)
        {
            Contract.Requires(world != null);
            Contract.Requires(device != null);
            Contract.Requires(shaderByteCode != null);

            world.DebugDrawer = this;

            this.device = device;
            inputAssembler = device.ImmediateContext.InputAssembler;
            lineArray = new PositionColored[0];

            InputElement[] elements = new InputElement[] {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("COLOR", 0, Format.R8G8B8A8_UNorm, 12, 0, InputClassification.PerVertexData, 0)
            };
            inputLayout = new InputLayout(device, shaderByteCode, elements);

            vertexBufferDesc = new BufferDescription() {
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.Write
            };

            vertexBufferBinding = new VertexBufferBinding(null, PositionColored.Stride, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (vertexBuffer != null) {
                    vertexBuffer.Dispose();
                    vertexBuffer = null;
                }
            }

            base.Dispose(disposing);
        }

        public void DrawDebugWorld(DynamicsWorld world)
        {
            Contract.Requires(world != null);

            world.DebugDrawWorld();

            if (lines.Count == 0)
                return;

            inputAssembler.InputLayout = inputLayout;

            if (lineArray.Length != lines.Count) {
                lineArray = new PositionColored[lines.Count];
                lines.CopyTo(lineArray);

                if (vertexBuffer != null) {
                    vertexBuffer.Dispose();
                }
                vertexBufferDesc.SizeInBytes = PositionColored.Stride * lines.Count;
                using (var data = new DataStream(vertexBufferDesc.SizeInBytes, false, true)) {
                    data.WriteRange(lineArray);
                    data.Position = 0;
                    vertexBuffer = new Buffer(device, data, vertexBufferDesc);
                }
                vertexBufferBinding.Buffer = vertexBuffer;
            } else {
                lines.CopyTo(lineArray);
                DataStream data;
                device.ImmediateContext.MapSubresource(vertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out data);
                data.WriteRange(lineArray);
                device.ImmediateContext.UnmapSubresource(vertexBuffer, 0);
                data.Dispose();
            }

            inputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            inputAssembler.PrimitiveTopology = global::SharpDX.Direct3D.PrimitiveTopology.LineList;

            device.ImmediateContext.Draw(lines.Count, 0);

            lines.Clear();
        }
    }
};