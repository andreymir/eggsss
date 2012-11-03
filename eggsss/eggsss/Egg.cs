using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace eggsss
{
    public class Egg
    {
        private Vector2 stepDelta;
        private Texture2D[] textures;
        private Vector2 position { get; set; }
        private TimeSpan lastStepTime;
        private TimeSpan pace;

        public int StepNumber
        {
            get;
            private set;
        }

        public bool Crushed;

        public void Initialize(Rectangle window, Texture2D[] textures, CatcherState trayNumber, TimeSpan createTime, TimeSpan pace)
        {
            this.textures = textures;
            lastStepTime = createTime;
            this.pace = pace;

            if (trayNumber == CatcherState.TopLeft || trayNumber == CatcherState.BottomLeft)
            {
                stepDelta = new Vector2(10, 10);
            }
            else if (trayNumber == CatcherState.TopRight || trayNumber == CatcherState.BottomRight)
            {
                stepDelta = new Vector2(-10, 10);
            }

            switch (trayNumber)
            {
                case CatcherState.TopLeft:
                    position = new Vector2(100, 100);
                    break;
                case CatcherState.TopRight:
                    position = new Vector2(400, 100);
                    break;
                case CatcherState.BottomRight:
                    position = new Vector2(400, 400);
                    break;
                case CatcherState.BottomLeft:
                    position = new Vector2(100, 400);
                    break;
            }

            StepNumber = 0;
            Crushed = false;
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - lastStepTime > pace)
            {
                lastStepTime = gameTime.TotalGameTime;
                StepNumber++;
                position += stepDelta;

                if (StepNumber > 4)
                {
                    Crushed = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Crushed)
            {
                var texture = textures[StepNumber];
                spriteBatch.Draw(texture, position, Color.White);
            }
        }
    }
}
