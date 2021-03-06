﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eggsss
{
    public class CrushedEgg
    {
        public CatcherState State { get; set; }

        public Texture2D[] Texture;

        public Vector2 Position;

        int elapsedTime;
        private const int FRAME_TIME = 30;
        private const int FRAME_COUNT = 3;
        int currentFrame;

        public bool Active;

        public void Initialize(Texture2D[] texture, Vector2 position, CatcherState state, TimeSpan gameTime)
        {
            elapsedTime = 0;
            currentFrame = 0;
            Texture = texture;
            Position = position;
            State = state;

            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (Active == false)
                return;


            // Update the elapsed time
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsedTime > FRAME_TIME)
            {
                currentFrame++;

                if (currentFrame == FRAME_COUNT)
                {
                    currentFrame = 0;
                    Active = false;
                }
                elapsedTime = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                Texture2D texture = null;
                var position = new Vector2();

                switch (State)
                {
                    case CatcherState.TopLeft:
                    case CatcherState.BottomLeft:
                        texture = Texture[0];
                        position = Position + new Vector2(- texture.Width - 200, - texture.Height - 100);
                        break;
                    case CatcherState.TopRight:
                    case CatcherState.BottomRight:
                        texture = Texture[1];
                        position = Position + new Vector2(texture.Width + 100, - texture.Height - 100);
                        break;
                }

                if (texture != null)
                {
                    spriteBatch.Draw(texture, position, Color.White);
                }
            }
        }
    }
}
