using System;
using BulletSharp;
using Vector3 = SharpDX.Vector3;

namespace SoLeap.Visualizer
{
    public class CollisionShapeConverter
    {
        public SharpDX.Vector3[] GetVertices(CollisionShape shape)
        {
            if (shape is SphereShape) {
                return GetVertices((SphereShape)shape);
            } else if (shape is BoxShape) {
                return GetVertices((BoxShape)shape);
            } else if (shape is CompoundShape) {
                return GetVertices((CompoundShape)shape);
            } else if (shape is StaticPlaneShape) {
                return GetVertices((StaticPlaneShape)shape);
            } else if (shape is CylinderShapeX) {
                return GetVertices((CylinderShapeX)shape);
            } else {
                throw new NotImplementedException("GetVertices");
            }
        }

        private Vector3[] GetVertices(SphereShape sphere)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GetVertices(BoxShape box)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GetVertices(CompoundShape compound)
        {
            throw new NotImplementedException();
        }

        private Vector3[] GetVertices(StaticPlaneShape plane)
        {
            throw new NotImplementedException();
        }
        private Vector3[] GetVertices(CylinderShapeX cylinder)
        {
            throw new NotImplementedException();
        }


    }
}