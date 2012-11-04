using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eggsss
{
    public class Health
    {
        public int Value { get; private set; }
        
        // Animation representing the player
        public Texture2D Texture;

        // Position of the Player relative to the upper left side of the screen
        public Vector2 Position;
        private readonly Vector2 stepDelta = new Vector2(80, 0);

        // State of the player
        public bool Active;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;

            Position = position + new Vector2(300, -650);

            Active = true;

            Value = 3;
        }

        public void Update()
        {
            Value--;
        }

        public void Clear()
        {
            Value = 3;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = Value - 3; i < 0; i++)
            {
                spriteBatch.Draw(Texture, Position + stepDelta * i, Color.White);
            }
        }
    }
}
