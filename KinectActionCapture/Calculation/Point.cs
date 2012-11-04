using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectActionCapture.Calculation
{
    struct Point
    {
        public double X;
        public double Y;
        public double Z;

        public Point(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
