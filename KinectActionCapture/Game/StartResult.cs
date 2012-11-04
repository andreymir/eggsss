using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectActionCapture.Game
{
    public enum StartResult
    {
        // Errors
        NoKinectDetected,       // Kinect wasn't detected
        ManyKinectsDetected,    // More than one kinects were detected
        KinectError,

        // Normal state
        KinectStarted
    }
}
