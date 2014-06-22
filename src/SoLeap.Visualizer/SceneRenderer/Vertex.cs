﻿using SharpDX;

namespace SoLeap.Visualizer
{
    public struct VertexPositionNormal
    {
        public static readonly int SizeInBytes = 2 * Vector3.SizeInBytes;

        public readonly Vector3 Position;
        public readonly Vector3 Normal;

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            Position = position;
            Normal = normal;
        }
    }
}