using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace SoLeap.Visualizer
{
    public class ConstantBuffer<T>
        : IDisposable
        where T : struct
    {
        private readonly Device device;

        private readonly DataStream stream;

        public Buffer Buffer { get; private set; }

        public ConstantBuffer(Device device)
        {
            Contract.Requires(device != null);

            this.device = device;

            var size = Marshal.SizeOf(typeof(T));

            var bufferDescription = new BufferDescription {
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ConstantBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = size,
                StructureByteStride = 0
            };

            Buffer = new Buffer(device, bufferDescription);

            stream = new DataStream(size, false, true);
        }

        public void Update(T value)
        {
            Marshal.StructureToPtr(value, stream.DataPointer, false);

            var dataBox = new DataBox(stream.DataPointer);

            device.ImmediateContext.UpdateSubresource(dataBox, Buffer);
        }

        public void Dispose()
        {
            stream.Dispose();
            Buffer.Dispose();
        }
    }
}
