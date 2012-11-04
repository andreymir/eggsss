using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KinectActionCapture.Game;
using KinectActionCapture.VoiceManipulation;

namespace KinectActionCapture
{
    public interface IKinectManager
    {
        StartResult StartKinect();

        GameState GetCurrentGameState();

        VoiceCommand GetCurrentVoiceCommand();

        void StopKinect();
    }
}
