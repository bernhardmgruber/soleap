using System.Runtime.InteropServices;

namespace SoLeap.Visualizer
{
    [StructLayout(LayoutKind.Explicit, Size = 2 * 64)]
    public struct FrameConstants
    {
        [FieldOffset(0)]
        private SharpDX.Matrix view;

        [FieldOffset(64)]
        private SharpDX.Matrix projection;

        public SharpDX.Matrix View
        {
            get { return SharpDX.Matrix.Transpose(view); }
            set { view = SharpDX.Matrix.Transpose(value); }
        }

        public SharpDX.Matrix Projection
        {
            get { return SharpDX.Matrix.Transpose(projection); }
            set { projection = SharpDX.Matrix.Transpose(value); }
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 64 + 16)]
    public struct ObjectConstants
    {
        [FieldOffset(0)]
        private BulletSharp.Matrix world;

        [FieldOffset(64)]
        public int Color;

        public BulletSharp.Matrix World
        {
            get { return BulletSharp.Matrix.Transpose(world); }
            set { world = BulletSharp.Matrix.Transpose(value); }
        }


    }
}