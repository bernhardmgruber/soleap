using BulletSharp;
using SoLeap;
using SoLeap.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace BowlPhysics
{
    public class PhysicsHand
    {
        private IDictionary<FingerType, IDictionary<BoneType, CollisionShape>> fingerShapes;
        public IDictionary<FingerType, IDictionary<BoneType, CollisionShape>> FingerShapes
        {
            get
            {
                assertCalibrated();
                return fingerShapes;
            }
        }

        public IDictionary<FingerType, IDictionary<BoneType, RigidBody>> fingerBodies;
        public IDictionary<FingerType, IDictionary<BoneType, RigidBody>> FingerBodies
        {
            get
            {
                assertCalibrated();
                return fingerBodies;
            }
        }

        private CollisionShape palmShape;
        public CollisionShape PalmShape
        {
            get
            {
                assertCalibrated();
                return palmShape;
            }
        }

        private RigidBody palmBody;
        public RigidBody PalmBody
        {
            get
            {
                assertCalibrated();
                return palmBody;
            }
        }

        public IEnumerable<Tuple<CollisionShape, Matrix>> AllShapesWithTransformations
        {
            get
            {
                assertCalibrated();

                // damn, that code duplication ...
                foreach (var fingerType in EnumUtils.GetValues<FingerType>())
                {
                    foreach (var boneType in EnumUtils.GetValues<BoneType>())
                    {
                        if (boneType == BoneType.Metacarpal)
                            continue; // skip the bones inside the hand

                        yield return new Tuple<CollisionShape, Matrix>(fingerShapes[fingerType][boneType], fingerBodies[fingerType][boneType].MotionState.WorldTransform);
                    }
                }

                yield return new Tuple<CollisionShape, Matrix>(palmShape, palmBody.MotionState.WorldTransform); 
            }
        }

        private readonly PhysicsWorld world;

        public bool Calibrated { get; private set; }

        public PhysicsHand(PhysicsWorld world)
        {
            this.world = world;

            fingerShapes = new Dictionary<FingerType, IDictionary<BoneType, CollisionShape>>();
            foreach (var fingerType in EnumUtils.GetValues<FingerType>())
                fingerShapes[fingerType] = new Dictionary<BoneType, CollisionShape>();

            fingerBodies = new Dictionary<FingerType, IDictionary<BoneType, RigidBody>>();
            foreach (var fingerType in EnumUtils.GetValues<FingerType>())
                fingerBodies[fingerType] = new Dictionary<BoneType, RigidBody>();
        }

        /// <summary>
        /// Creates all rigid bodies for the physics world according to the measures of the provided hand.
        /// </summary>
        public void Calibrate(Hand hand)
        {
            CollisionShape shape;
            RigidBody body;

            // create fingers
            foreach (var fingerType in EnumUtils.GetValues<FingerType>())
            {
                foreach (var boneType in EnumUtils.GetValues<BoneType>())
                {
                    if (boneType == BoneType.Metacarpal)
                        continue; // skip the bones inside the hand

                    // get the bone
                    Bone bone = hand.GetFinger(fingerType).GetBone(boneType);

                    string userObjectString = "Finger " + fingerType + " " + boneType;

                    // build collision shape
                    float length = (float)(bone.PrevJoint - bone.NextJoint).Length;
                    float width = (float)bone.Width;
                    float height = (float)bone.Width;

                    shape = new BoxShape(width / 2.0f, height / 2.0f, length / 2.0f);
                    shape.UserObject = userObjectString;
                    world.CollisionShapes.Add(shape);

                    // create rigid body
                    body = world.CreateRigidBody(1.0f, ConvertMatrix(bone.Transformation), shape, userObjectString, true);

                    // store shape and body
                    fingerShapes[fingerType][boneType] = shape;
                    fingerBodies[fingerType][boneType] = body;
                }
            }

            // create palm ground shape by collecting all corner points of the palm
            IList<Vector3> groundShapePoints = new List<Vector3>();

            // create offset vector to make the palm broader (extend it to the left and right)
            var leftMetacarpalBone = hand.GetFinger(FingerType.Pinky).GetBone(BoneType.Metacarpal);
            var rightMetacarpalBone = hand.GetFinger(FingerType.Index).GetBone(BoneType.Metacarpal);

            var left = ConvertDirection(Vector3D.CrossProduct((leftMetacarpalBone.NextJoint - leftMetacarpalBone.PrevJoint), hand.PalmNormal));
            var right = ConvertDirection(Vector3D.CrossProduct(hand.PalmNormal, (rightMetacarpalBone.NextJoint - rightMetacarpalBone.PrevJoint)));

            left.Normalize();
            right.Normalize();

            left *= (float)leftMetacarpalBone.Width / 2.0f;
            right *= (float)rightMetacarpalBone.Width / 2.0f;

            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Pinky).GetBone(BoneType.Metacarpal).PrevJoint) + left);
            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Pinky).GetBone(BoneType.Metacarpal).NextJoint) + left);
            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Ring).GetBone(BoneType.Metacarpal).NextJoint));
            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Middle).GetBone(BoneType.Metacarpal).NextJoint));
            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Index).GetBone(BoneType.Metacarpal).NextJoint) + right);
            groundShapePoints.Add(ConvertPosition(hand.GetFinger(FingerType.Thumb).GetBone(BoneType.Metacarpal).PrevJoint));

            // now extrude palm 3D shape using the palm's width
            Vector3 normal = ConvertDirection(hand.PalmNormal);
            normal.Normalize();

            float palmHeight2 = (float)hand.PalmHeight / 2.0f;

            var lowerPointsTrans = groundShapePoints.Select(p => p + normal * palmHeight2).ToList();
            var upperPointsTrans = groundShapePoints.Select(p => p - normal * palmHeight2).ToList();

            // now remove the palm's transformation from them
            var inverseTransformation = Matrix.Invert(ConvertMatrix(hand.PalmTransformation));

            var lowerPoints = lowerPointsTrans.Select(p => { Vector4 v = Vector3.Transform(p, inverseTransformation); return new Vector3(v.X, v.Y, v.Z); }).ToList();
            var upperPoints = upperPointsTrans.Select(p => { Vector4 v = Vector3.Transform(p, inverseTransformation); return new Vector3(v.X, v.Y, v.Z); }).ToList();

            // now triangulate ;)
            var mesh = new TriangleMesh();

            Action<IList<Vector3>, TriangleMesh> triangulatePolygon = (ps, m) =>
            {
                // triangle fan
                var first = ps.First();
                var last = ps[1];
                for (int i = 2; i < ps.Count; i++)
                {
                    var cur = ps[i];
                    m.AddTriangle(first, last, cur, false);
                    last = cur;
                }
            };

            // generate top and bottom
            triangulatePolygon(lowerPoints, mesh);
            triangulatePolygon(upperPoints, mesh);

            // generate side
            Debug.Assert(lowerPoints.Count == upperPoints.Count);
            for (int i = 0; i < lowerPoints.Count; i++)
            {
                var curLower = lowerPoints[i];
                var curUpper = upperPoints[i];
                var nextLower = lowerPoints[(i + 1) % lowerPoints.Count];
                var nextUpper = upperPoints[(i + 1) % upperPoints.Count];

                mesh.AddTriangle(curLower, curUpper, nextLower);
                mesh.AddTriangle(curUpper, nextUpper, nextLower);
            }

            // create shape from triangle mesh
            palmShape = new ConvexTriangleMeshShape(mesh);
            palmShape.UserObject = "palm";
            world.CollisionShapes.Add(palmShape);

            // create rigid body
            palmBody = world.CreateRigidBody(1.0f, ConvertMatrix(hand.PalmTransformation), palmShape, "palm", true);

            Calibrated = true;
        }

        public void Update(Hand hand)
        {
            assertCalibrated();

            // update fingers
            // TODO, code duplication with Calibrate() method
            foreach (var fingerType in EnumUtils.GetValues<FingerType>())
            {
                foreach (var boneType in EnumUtils.GetValues<BoneType>())
                {
                    if (boneType == BoneType.Metacarpal)
                        continue; // skip the bones inside the hand

                    // grab body
                    RigidBody body = fingerBodies[fingerType][boneType];

                    // find corresponding bone
                    Bone bone = hand.GetFinger(fingerType).GetBone(boneType);

                    // apply bone basis to the rigid body's world transformation
                    body.MotionState.WorldTransform = ConvertMatrix(bone.Transformation);
                }
            }

            // update palm
            palmBody.MotionState.WorldTransform = ConvertMatrix(hand.PalmTransformation);
        }

        private Matrix ConvertMatrix(Matrix3D m)
        {
            return new Matrix()
            {
                M11 = (float)m.M11,
                M12 = (float)m.M12,
                M13 = (float)m.M13,
                M14 = (float)m.M14,
                M21 = (float)m.M21,
                M22 = (float)m.M22,
                M23 = (float)m.M23,
                M24 = (float)m.M24,
                M31 = (float)m.M31,
                M32 = (float)m.M32,
                M33 = (float)m.M33,
                M34 = (float)m.M34,
                M41 = (float)m.OffsetX,
                M42 = (float)m.OffsetY,
                M43 = (float)m.OffsetZ,
                M44 = (float)m.M44,
            };
        }

        private Vector3 ConvertPosition(Point3D p)
        {
            return new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        private Vector3 ConvertDirection(Vector3D p)
        {
            return new Vector3((float)p.X, (float)p.Y, (float)p.Z);
        }

        private void assertCalibrated()
        {
            if (!Calibrated)
                throw new InvalidOperationException("Hand is not initialized");
        }
    }
}
