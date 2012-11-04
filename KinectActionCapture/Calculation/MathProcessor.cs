using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectActionCapture.Calculation
{
    internal static class MathProcessor
    {
        public static double CalculateDistance(SkeletonPoint point1, SkeletonPoint point2)
        {
            return CalculateDistance(new Point(point1.X, point1.Y, point1.Z),
                                     new Point(point2.X, point2.Y, point2.Z));
        }

        public static double CalculateDistance(Point point1, Point point2)
        {
            return Math.Sqrt(
                Math.Pow(point1.X - point2.X, 2.0) +
                Math.Pow(point1.Y - point2.Y, 2.0) +
                Math.Pow(point1.Z - point2.Z, 2.0)
                );
        }

        public static Point CalculateMiddlePoint(SkeletonPoint point1, SkeletonPoint point2)
        {
            return new Point(
                GetMiddleValue(point1.X, point2.X),
                GetMiddleValue(point1.Y, point2.Y),
                GetMiddleValue(point1.Z, point2.Z)
            );
        }

        private static double GetMiddleValue(double value1, double value2)
        {
            double delta = Math.Abs(value1 - value2);

            return Math.Min(value1, value2) + delta/2.0;
        }
    }
}
