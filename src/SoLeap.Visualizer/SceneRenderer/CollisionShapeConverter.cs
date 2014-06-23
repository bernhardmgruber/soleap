using System;
using System.Collections.Generic;
using BulletSharp;
using Vector3 = SharpDX.Vector3;
using BtVector3 = BulletSharp.Vector3;

namespace SoLeap.Visualizer
{
    public static class CollisionShapeConverter
    {
        public static IList<VertexPositionNormal> GetVertices(CollisionShape shape)
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
            if (shape is ConvexTriangleMeshShape)
                return GetVertices((ConvexTriangleMeshShape)shape);
            throw new NotImplementedException("GetVertices");
        }

        private static IList<VertexPositionNormal> GetVertices(SphereShape sphere)
        {
            throw new NotImplementedException();
        }

        private static IList<VertexPositionNormal> GetVertices(BoxShape box)
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

        private static IList<VertexPositionNormal> GetVertices(CompoundShape compound)
        {

            foreach (CompoundShapeChild child in compound.ChildList) {
                var shape = child.ChildShape;
                var world = child.Transform;
            }

            throw new NotImplementedException();
        }

        private static IList<VertexPositionNormal> GetVertices(StaticPlaneShape plane)
        {
            // http://bulletphysics.com/Bullet/BulletFull/btStaticPlaneShape_8cpp_source.html#l00058

            var normal = plane.PlaneNormal;
            var constant = plane.PlaneConstant;

            BtVector3 aabbMin, aabbMax;
            plane.GetAabb(Matrix.Identity, out aabbMin, out aabbMax);

            var halfExtents = (aabbMax - aabbMin) * 0.5f;
            float radius = halfExtents.Length() / 1000000000000000.0f;
            var center = (aabbMax + aabbMin) * 0.5f;

            // this is where the triangles are generated, given AABB and plane equation (normal/constant)

            BtVector3 tangentDir0, tangentDir1;
            //tangentDir0/tangentDir1 can be precalculated
            PlaneSpace(plane.PlaneNormal, out tangentDir0, out tangentDir1);

            var projectedCenter = center - (BtVector3.Dot(normal, center) - constant) * normal;

            var vertices = new List<VertexPositionNormal>(6);
            // triangle 1
            vertices.Add(new VertexPositionNormal(projectedCenter + tangentDir0 * radius + tangentDir1 * radius, normal));
            vertices.Add(new VertexPositionNormal(projectedCenter - tangentDir0 * radius - tangentDir1 * radius, normal));
            vertices.Add(new VertexPositionNormal(projectedCenter + tangentDir0 * radius - tangentDir1 * radius, normal));
            // triangle 2
            vertices.Add(new VertexPositionNormal(projectedCenter - tangentDir0 * radius - tangentDir1 * radius, normal));
            vertices.Add(new VertexPositionNormal(projectedCenter + tangentDir0 * radius + tangentDir1 * radius, normal));
            vertices.Add(new VertexPositionNormal(projectedCenter - tangentDir0 * radius + tangentDir1 * radius, normal));

            return vertices;
        }

        private static void PlaneSpace(BtVector3 n, out BtVector3 p, out BtVector3 q)
        {
            // http://bulletphysics.com/Bullet/BulletFull/btVector3_8h_source.html#l01261
            const float SIMDSQRT12 = 0.7071067811865475244008443621048490f;
            p = new BtVector3();
            q = new BtVector3();

            if (Math.Abs(n[2]) > SIMDSQRT12) {
                // choose p in y-z plane
                float a = n[1] * n[1] + n[2] * n[2];
                float k = 1.0f / (a * a);
                p[0] = 0;
                p[1] = -n[2] * k;
                p[2] = n[1] * k;
                // set q = n x p
                q[0] = a * k;
                q[1] = -n[0] * p[2];
                q[2] = n[0] * p[1];
            } else {
                // choose p in x-y plane
                float a = n[0] * n[0] + n[1] * n[1];
                float k = 1.0f / (a * a);
                p[0] = -n[1] * k;
                p[1] = n[0] * k;
                p[2] = 0;
                // set q = n x p
                q[0] = -n[2] * p[1];
                q[1] = n[2] * p[0];
                q[2] = a * k;
            }
        }

        private static IList<VertexPositionNormal> GetVertices(CylinderShapeX cylinder)
        {
            throw new NotImplementedException();
        }

        private static IList<VertexPositionNormal> GetVertices(ConvexTriangleMeshShape meshShape)
        {
            var vertices = new List<VertexPositionNormal>();

            DataStream stream, indexStream;
            int numVertes, vertexStride, indexStride, numFaces;

            PhyScalarType type, indicesType;
            meshShape.MeshInterface.GetLockedReadOnlyVertexIndexData(out stream, out numVertes, out type, out vertexStride, out indexStream, out indexStride, out numFaces, out indicesType);

            for (int i = 0; i < numFaces; i++) {
                var positions = new SharpDX.Vector3[3];

                for (int j = 0; j < 3; j++) {
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
                    vertices.Add(new VertexPositionNormal(positions[j], normal));
            }

            meshShape.MeshInterface.UnlockReadOnlyVertexData(0);

            return vertices;
        }
    }
}