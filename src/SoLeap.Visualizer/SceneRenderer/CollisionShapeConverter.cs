using System;
using System.Collections.Generic;
using BulletSharp;
using Vector3 = SharpDX.Vector3;

namespace SoLeap.Visualizer
{
    public class CollisionShapeConverter
    {
        public IList<VertexPositionNormal> GetVertices(CollisionShape shape)
        {
            if (shape is SphereShape)
                return GetVertices((SphereShape)shape);
            if (shape is BoxShape)
                return GetVertices((BoxShape)shape);
            if (shape is CompoundShape)
                return GetVertices((CompoundShape)shape);
            if (shape is StaticPlaneShape)
                return GetVertices((StaticPlaneShape)shape);
            if (shape is CylinderShapeX)
                return GetVertices((CylinderShapeX)shape);
            throw new NotImplementedException("GetVertices");
        }

        private IList<VertexPositionNormal> GetVertices(SphereShape sphere)
        {
            throw new NotImplementedException();
        }

        private IList<VertexPositionNormal> GetVertices(BoxShape box)
        {
            var vertices = new List<VertexPositionNormal>(6 * 6);

            var extents = box.HalfExtentsWithoutMargin;
            var x2 = extents.X;
            var y2 = extents.Y;
            var z2 = extents.Z;

            Vector3 normal;

            // side 1
            normal = new Vector3(0, 0, -1);
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, -z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, -z2), normal));

            // side 2                                                     
            normal = new Vector3(-1, 0, 0);
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, +z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, -z2), normal));

            // side 3                                                       
            normal = new Vector3(0, 0, +1);
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, +z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, +z2), normal));

            // side 4                                                     
            normal = new Vector3(+1, 0, 0);
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, +z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, +z2), normal));

            // side 5                                                    
            normal = new Vector3(0, +1, 0);
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, -z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(-x2, +y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, +y2, -z2), normal));

            // side 6                                                       
            normal = new Vector3(0, -1, 0);
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, -z2), normal));

            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, -z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(+x2, -y2, +z2), normal));
            vertices.Add(new VertexPositionNormal(new Vector3(-x2, -y2, +z2), normal));

            return vertices;
        }

        private IList<VertexPositionNormal> GetVertices(CompoundShape compound)
        {

            foreach (CompoundShapeChild child in compound.ChildList) {
                var shape = child.ChildShape;
                var world = child.Transform;
            }

            throw new NotImplementedException();
        }

        private IList<VertexPositionNormal> GetVertices(StaticPlaneShape plane)
        {
            throw new NotImplementedException();
        }
        private IList<VertexPositionNormal> GetVertices(CylinderShapeX cylinder)
        {
            throw new NotImplementedException();
        }
    }
}