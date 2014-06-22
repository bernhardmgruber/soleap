using System.Runtime.InteropServices;

namespace SoLeap.Visualizer
{
    [StructLayout(LayoutKind.Explicit, Size = 2 * 64 + 2 * 16 + (DirectionalLight.MaxLights * 32))]
    public struct FrameConstants
    {
        [FieldOffset(0)]
        private SharpDX.Matrix view;

        [FieldOffset(64)]
        private SharpDX.Matrix projection;

        [FieldOffset(128)]
        public SharpDX.Vector3 EyePosition;

        [FieldOffset(128 + 16)]
        public SharpDX.Color3 AmbientLightColor;

        [FieldOffset(128 + 32), MarshalAs(UnmanagedType.ByValArray, SizeConst = DirectionalLight.MaxLights)]
        public DirectionalLight[] Lights;

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

    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct DirectionalLight
    {
        public const int MaxLights = 4;

        [FieldOffset(0)]
        public SharpDX.Vector3 Direction;

        [FieldOffset(16)]
        public SharpDX.Color3 Color;

        [FieldOffset(28)]
        public bool Enabled;
    }


    [StructLayout(LayoutKind.Explicit, Size = 128 + 48)]
    public struct ObjectConstants
    {
        [FieldOffset(0)]
        private BulletSharp.Matrix world;

        [FieldOffset(64)]
        private BulletSharp.Matrix worldInverseTranspose;

        [FieldOffset(128)]
        public SharpDX.Color3 Ambient;

        [FieldOffset(128 + 16)]
        public SharpDX.Color3 Diffuse;

        [FieldOffset(128 + 32)]
        public SharpDX.Color3 Specular;

        [FieldOffset(128 + 44)]
        public float SpecularPower;

        public BulletSharp.Matrix World
        {
            get { return BulletSharp.Matrix.Transpose(world); }
            set { world = BulletSharp.Matrix.Transpose(value); }
        }

        public BulletSharp.Matrix WorldInverseTranspose
        {
            get { return BulletSharp.Matrix.Transpose(worldInverseTranspose); }
            set { worldInverseTranspose = BulletSharp.Matrix.Transpose(value); }
        }


    }
}