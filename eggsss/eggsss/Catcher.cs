using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eggsss
{
    public class Catcher
    {
        public CatcherState State { get; set; }

        // Animation representing the player
        public Texture2D[] PlayerTexture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Active;

        public void Initialize(Texture2D[] texture, Vector2 position, CatcherState state)
        {
            PlayerTexture = texture;

            Position = position;

            // Set the starting position of the player around the middle of thescreen and to the back
            State = state;

            // Set the player to be active
            Active = true;
        }


        public void Update(CatcherState state)
        {
            State = state;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D bodyTexture = null;
            Texture2D basketTexture = null;
            var bodyPosition = new Vector2();
            var basketPosition = new Vector2();

            const int BODY_HEIGHT = 140;
            switch (State)
            {
                case CatcherState.TopLeft:
                    bodyTexture = PlayerTexture[0];
                    basketTexture = PlayerTexture[2];
                    bodyPosition = Position + new Vector2(-bodyTexture.Width, -bodyTexture.Height - BODY_HEIGHT);
                    basketPosition = Position + new Vector2(-bodyTexture.Width - basketTexture.Width + 50, - bodyTexture.Height - 150);
                    break;
                case CatcherState.BottomLeft:
                    bodyTexture = PlayerTexture[0];
                    basketTexture = PlayerTexture[3];
                    bodyPosition = Position + new Vector2(-bodyTexture.Width, -bodyTexture.Height - BODY_HEIGHT);
                    basketPosition = Position + new Vector2(-bodyTexture.Width - basketTexture.Width + 40, - bodyTexture.Height + basketTexture.Height - 100);
                    break;
                case CatcherState.TopRight:
                    bodyTexture = PlayerTexture[1];
                    basketTexture = PlayerTexture[4];
                    bodyPosition = Position + new Vector2(0, -bodyTexture.Height - BODY_HEIGHT);
                    basketPosition = Position + new Vector2(basketTexture.Width + basketTexture.Width - 180 , - bodyTexture.Height - 150);
                    break;
                case CatcherState.BottomRight:
                    bodyTexture = PlayerTexture[1];
                    basketTexture = PlayerTexture[5];
                    bodyPosition = Position + new Vector2(0, -bodyTexture.Height - BODY_HEIGHT);
                    basketPosition = Position + new Vector2(basketTexture.Width + basketTexture.Width - BODY_HEIGHT, - bodyTexture.Height + basketTexture.Height - 100);
                    break;
            }

            if (bodyTexture == null || basketTexture == null)
            {
                throw new InvalidOperationException();
            }

            spriteBatch.Draw(bodyTexture, bodyPosition, Color.White);
            spriteBatch.Draw(basketTexture, basketPosition, Color.White);
        }
    }
}
