using Device;
using HandReconstruction.Domain;
using HandReconstruction.Reconstructer.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HandReconstruction.Reconstructer
{
    public class SimpleReconstructer : AbstractReconstructor
    {
        public SimpleReconstructer(HandCalibration calibration)
            : base(calibration) { }

        private Vector3D RotateAroundAxis(Vector3D vector, Vector3D axis, double angle)
        {
            // from http://inside.mines.edu/~gmurray/ArbitraryAxisRotation/
            double u = axis.X, v = axis.Y, w = axis.Z;
            double x = vector.X, y = vector.Y, z = vector.Z;

            double c = Math.Cos(angle);
            double s = Math.Sin(angle);

            double t = (u * x + v * y + w * z) * (1 - c);

            return new Vector3D(
                u * t + x * c + (-w * y + v * z) * s,
                v * t + y * c + (w * x - u * z) * s,
                w * t + z * c + (-v * x - u * y) * s
                );
        }


        private IList<Point3D> inverseKinematic2(Point3D fingerStart, Point3D target, IList<double> s, Vector3D up)
        {
            IList<Point3D> result = new List<Point3D>();

            Vector3D dVec = target - fingerStart;
            double d = Math.Abs(dVec.Length);

            if (s[0] + s[1] >= d) {
                // the target is too far away, stretch finger
                dVec.Normalize();
                result.Add(fingerStart + dVec * s[0]);
                result.Add(fingerStart + dVec * (s[0] + s[1]));
            } else {
                // angle between d and first segment
                double beta = Math.Acos((s[0] * s[0] + d * d - s[1] * s[1]) / (2 * s[0] * d));

                // turn dVec by beta around right
                Vector3D right = Vector3D.CrossProduct(dVec, up);
                right.Normalize();

                Vector3D v = RotateAroundAxis(dVec, right, beta);

                // scale to length of first segment
                v.Normalize();
                v = v * s[0];

                result.Add(fingerStart + v);

                // turn v by alpha-180 around right
                double alpha = Math.Acos((s[0] * s[0] + s[1] * s[1] - d * d) / (2 * s[0] * s[1]));

                v = RotateAroundAxis(v, right, alpha - 180);

                // scale to length of second segment
                v.Normalize();
                v = v * s[1];

                result.Add(result[0] + v);
            }

            return result;
        }

        /// <summary>
        /// Calculates the point the finger reaches from a given start position.
        /// </summary>
        /// <param name="start">Start position</param>
        /// <param name="s">Segment lengths</param>
        /// <param name="a">Angle for each segment</param>
        /// <returns></returns>
        private Point3D forwardKinematic(Point3D start, Point3D target, IList<double> s, IList<double> a, Vector3D up)
        {
            if (s.Count != a.Count)
                throw new ArgumentException("there must be equally as much angles as segments");

            Point3D p = start;
            Vector3D v = target - start;
            Vector3D right = Vector3D.CrossProduct(v, up);

            for (int i = 0; i < s.Count; i++) {
                v = RotateAroundAxis(v, right, a[i]);
                v.Normalize();
                p = p + v * s[i];
            }

            return p;
        }

        private IList<Point3D> inverseKinematicIterative(Point3D start, Point3D target, IList<double> s, Vector3D up)
        {
            IList<Point3D> result = new List<Point3D>();

            Vector3D dVec = start - target;
            double d = Math.Abs(dVec.Length);

            if (s.Sum() >= d) {
                // the target is too far away, stretch finger
                dVec.Normalize();
                Point3D p = start;
                result.Add(start);
                foreach (double segment in s) {
                    p = p + dVec * segment;
                    result.Add(p);
                }
            } else {
                const double step = 1.0;

                //
                // optimize on alpha
                //

                // start with a straight finger with zero angles
                double alpha = +step;

                double bestDelta = Math.Abs(s.Sum() - d); // current best delta to d
                while (alpha >= -(360.0 / s.Count)) {
                    alpha -= step;

                    IList<double> a = new List<double>();
                    for (int i = 0; i < s.Count; i++)
                        a.Add(alpha);

                    Point3D t = forwardKinematic(start, target, s, a, up);
                    double currentD = Math.Abs((t - start).Length);

                    double currentDelta = Math.Abs(currentD - d);

                    if (currentDelta < bestDelta)
                        // this iteration was successful
                        bestDelta = currentDelta;
                    else {
                        // this iteration was unsuccessful, roll back the change to alpha and take it
                        alpha += step;
                        break;
                    }
                }

                //
                // Calculate beta using cosine law
                //

                double opposingEdge = s.Last();
                for (int i = s.Count - 2; i > 0; i--) {
                    double edge1 = opposingEdge;
                    double edge2 = s[i];
                    double angle = 180.0 + alpha; // alpha is negative

                    opposingEdge = Math.Sqrt(edge1 * edge1 + edge2 * edge2 - 2 * edge1 * edge2 * Math.Cos(angle));
                }

                double beta = Math.Acos((s[0] * s[0] + d * d - opposingEdge * opposingEdge) / (2.0 * s[0] * d));

                //
                // Forward kinematic for final points
                //

                result.Add(start);

                Vector3D v = dVec;
                Vector3D right = Vector3D.CrossProduct(v, up);
                Point3D p = start;
                for (int i = 0; i < s.Count; i++) {
                    v = RotateAroundAxis(v, right, i == 0 ? beta : alpha);
                    v.Normalize();
                    p = p + v * s[i];
                    result.Add(p);
                }
            }

            return result;
        }

        private IList<Point3D> inverseKinematic(Point3D fingerStart, Point3D target, IList<double> segmentLengths, Vector3D up)
        {
            switch (segmentLengths.Count) {
            case 0:
                throw new ArgumentException("no segments");
            case 1:
                Vector3D d = (fingerStart - target);
                d.Normalize();
                return new List<Point3D>() { fingerStart, fingerStart + d * segmentLengths[0] };
            case 2:
                return inverseKinematic2(fingerStart, target, segmentLengths, up);
            default:
                return inverseKinematicIterative(fingerStart, target, segmentLengths, up);
            }
        }

        public override ReconstructedHand Process(HandInputFrame input)
        {
            ReconstructedHand result = new ReconstructedHand();

            result.HandDirection = input.HandDirection;
            result.PalmPosition = input.PalmPosition;
            result.PalmNormal = input.PalmNormal;

            for (int i = 0; i < 5; i++) {
                Vector3D palmToTip = input.TipPositions[i] - input.PalmPosition;

                // make it normal to palmNormal
                Vector3D right = Vector3D.CrossProduct(input.PalmNormal, palmToTip);
                Vector3D toFingerStart = Vector3D.CrossProduct(right, input.PalmNormal);
                toFingerStart.Normalize();

                Point3D fingerStart = input.PalmPosition + toFingerStart * calibration.FingerSegmentLengths[i][0];

                IList<double> segments = new List<double>(calibration.FingerSegmentLengths[i]);
                segments.RemoveAt(0);

                result.FingerJointPositions[i] = inverseKinematic(fingerStart, input.TipPositions[i], segments, -input.PalmNormal).ToArray();
            }

            return result;
        }
    }
}
