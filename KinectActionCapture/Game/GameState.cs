using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KinectActionCapture.Game
{
    public enum GameState
    {
        // Normal Game states
        FirstAreaSelected,      // Left Bottom Area was selected
        SecondAreaSelected,     // Left Top Area was selected
        ThirdAreaSelected,      // Right Top Area was selected
        FourthAreaSelected,     // Right Bottom Area was selected
        NoAreaSelected          // Area wasn't selected (no skeleton in frame etc.)
    }
}
