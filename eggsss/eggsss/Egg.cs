using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace eggsss
{
    public class Egg
    {
        private Vector2 stepDelta;
        private Texture2D[] textures;
        public Vector2 Position { get; set; }
        private TimeSpan lastStepTime;
        public TimeSpan Pace;
        public int Value { get; set; }
        public bool Active { get; set; }
        public CatcherState TrayNumber { get; set; }
        private SoundEffect[] soundEffects;

        public int StepNumber
        {
            get;
            private set;
        }

        public bool Crushed;

        public void Initialize(Rectangle window, Texture2D[] textures, SoundEffect[] soundEffects, CatcherState trayNumber, TimeSpan createTime, TimeSpan pace)
        {
            this.textures = textures;
            lastStepTime = createTime;
            this.soundEffects = soundEffects;
            Pace = pace;
            TrayNumber = trayNumber;

            switch (trayNumber)
            {
                case CatcherState.BottomLeft:
                case CatcherState.TopLeft:
                    stepDelta = new Vector2(25, 18);
                    break;
                case CatcherState.BottomRight:
                case CatcherState.TopRight:
                    stepDelta = new Vector2(-25, 18);
                    break;
            }

            switch (trayNumber)
            {
                case CatcherState.TopLeft:
                    Position = new Vector2(150, 270);
                    break;
                case CatcherState.TopRight:
                    Position = new Vector2(850, 270);
                    break;
                case CatcherState.BottomRight:
                    Position = new Vector2(900, 390);
                    break;
                case CatcherState.BottomLeft:
                    Position = new Vector2(100, 390);
                    break;
            }

            StepNumber = 0;
            Crushed = false;
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastStepTime > Pace)
            {
                lastStepTime = gameTime.TotalGameTime;
                soundEffects[(int) TrayNumber - 1].Play();
                StepNumber++;
                Position += stepDelta;

                if (StepNumber > 4)
                {
                    Crushed = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Crushed) return;
            var texture = textures[StepNumber];
            spriteBatch.Draw(texture, Position, Color.White);
        }
    }
}
