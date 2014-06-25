using Leap;
using SoLeap.Devices;
using SoLeap.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Media.Media3D;
using Bone = SoLeap.Domain.Bone;
using Finger = SoLeap.Domain.Finger;
using Hand = SoLeap.Domain.Hand;

namespace SoLeap.LeapProvider
{
    public class FrameConverter
        : IFrameConverter
    {
        public HandsFrame Convert(Frame leapFrame)
        {
            var hands = ConvertHands(leapFrame.Hands);

            var handsFrame = new HandsFrame(leapFrame.Id, new DateTime(leapFrame.Timestamp), hands);

            return handsFrame;
        }

        private IEnumerable<Hand> ConvertHands(HandList leapHands)
        {
            foreach (var hand in leapHands)
            {
                yield return ConvertHand(hand);
            }
        }

        private Hand ConvertHand(Leap.Hand leapHand)
        {
            Point3D palmPosition = ConvertPosition(leapHand.PalmPosition);
            Vector3D palmNormal = ConvertDirection(leapHand.PalmNormal);
            double palmWidth = leapHand.PalmWidth;
            double palmHeight = leapHand.PalmWidth / 7.32; // magic, don't touch   lulz what the faq?
            Matrix3D basis = ConvertMatrix(leapHand.Basis);
            basis.OffsetX = palmPosition.X;
            basis.OffsetY = palmPosition.Y;
            basis.OffsetZ = palmPosition.Z;
            Vector3D handDirection = ConvertDirection(leapHand.Direction);
            bool isRight = leapHand.IsRight;
            IList<Finger> fingers = ConvertFingers(leapHand.Fingers);

            return new Hand(leapHand.Id, palmPosition, palmNormal, palmWidth, palmHeight, basis, handDirection, isRight, fingers);
        }

        private IList<Finger> ConvertFingers(FingerList leapFingers)
        {
            Contract.Ensures(Contract.Result<IList<Finger>>().All(f => f != null));

            var fingers = new List<Finger>(5) { null, null, null, null, null };

            foreach (var leapFingerType in EnumUtils.GetValues<Leap.Finger.FingerType>())
            {
                FingerType fingerType = ConvertFingerType(leapFingerType);

                // copy leapFingers because FingerList.FingerType() modifies the list
                // see https://developer.leapmotion.com/documentation/skeletal/csharp/api/Leap.FingerList.html
                var list = new FingerList();
                list.Append(leapFingers);
                var finger = list.FingerType(leapFingerType).Single();

                fingers[(int)fingerType] = ConvertFinger(finger);
            }

            return fingers;
        }

        private Finger ConvertFinger(Leap.Finger leapFinger)
        {
            FingerType fingerType = ConvertFingerType(leapFinger.Type());
            IList<Bone> bones = ExtractBones(leapFinger);
            IList<Point3D> joints = ExtractJoints(leapFinger);
            return new Finger(fingerType, bones, joints);
        }

        private IList<Bone> ExtractBones(Leap.Finger leapFinger)
        {
            Contract.Ensures(Contract.Result<IList<Bone>>().All(b => b != null));

            var bones = new List<Bone>(4) { null, null, null, null };

            foreach (var leapBoneType in EnumUtils.GetValues<Leap.Bone.BoneType>())
            {
                BoneType boneType = ConvertBoneType(leapBoneType);
                var bone = leapFinger.Bone(leapBoneType);
                bones[(int)boneType] = ConvertBone(bone);
            }

            return bones;
        }

        private Bone ConvertBone(Leap.Bone leapBone)
        {
            BoneType boneType = ConvertBoneType(leapBone.Type);
            Point3D nextJoint = ConvertPosition(leapBone.NextJoint);
            Point3D prevJoint = ConvertPosition(leapBone.PrevJoint);
            Matrix3D basis = ConvertMatrix(leapBone.Basis);
            basis.OffsetX = leapBone.Center.x;
            basis.OffsetY = leapBone.Center.y;
            basis.OffsetZ = leapBone.Center.z;
            double width = leapBone.Width;

            return new Bone(boneType, nextJoint, prevJoint, width, basis);
        }

        private IList<Point3D> ExtractJoints(Leap.Finger leapFinger)
        {
            Contract.Ensures(Contract.Result<IList<Point3D>>().All(b => b != default(Point3D)));

            var joints = new List<Point3D>(5) { default(Point3D), default(Point3D), default(Point3D), default(Point3D), default(Point3D) };

            var metacarpal = leapFinger.Bone(Leap.Bone.BoneType.TYPE_METACARPAL);
            joints[(int)JointType.CarpalMetacarpal] = ConvertPosition(metacarpal.PrevJoint);
            foreach (var leapJointType in EnumUtils.GetValues<Leap.Finger.FingerJoint>())
            {
                var jointType = ConvertJointType(leapJointType);
                var joint = leapFinger.JointPosition(leapJointType);
                joints[(int)jointType] = ConvertPosition(joint);
            }

            return joints;
        }

        private Point3D ConvertPosition(Vector leapVector)
        {
            return new Point3D(leapVector.x, leapVector.y, leapVector.z);
        }

        private Vector3D ConvertDirection(Vector leapVector)
        {
            return new Vector3D(leapVector.x, leapVector.y, leapVector.z);
        }

        private Matrix3D ConvertMatrix(Matrix leapMatrix)
        {
            float[] fs = leapMatrix.ToArray4x4();
            if (fs.Length != 16)
                throw new InvalidOperationException("Matrix.ToArray4x4() does not return a 16 element array");
            return new Matrix3D(fs[0], fs[1], fs[2], fs[3], fs[4], fs[5], fs[6], fs[7], fs[8], fs[9], fs[10], fs[11], fs[12], fs[13], fs[14], fs[15]);
        }

        private FingerType ConvertFingerType(Leap.Finger.FingerType leapFingerType)
        {
            return (FingerType)leapFingerType;
        }

        private BoneType ConvertBoneType(Leap.Bone.BoneType leapBoneType)
        {
            return (BoneType)leapBoneType;
        }

        private JointType ConvertJointType(Leap.Finger.FingerJoint leapJointType)
        {
            return (JointType)((int)leapJointType + 1);
        }
    }
}