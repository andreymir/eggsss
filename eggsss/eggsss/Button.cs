using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace eggsss
{
    public class Button : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        public Button(Game game, string text, Vector2 position)
            : base(game)
        {
            Text = text;
            Position = position;
        }

        public override void Initialize()
        {
            base.Initialize();

            var size = Font.MeasureString(Text);
            Width = (int)Math.Ceiling(size.X) + 20;
            Height = Texture.Height;
        }

        public string Text { get; set; }
        public Texture2D Texture { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            this.Texture = this.Game.Content.Load<Texture2D>("button");
            Font = this.Game.Content.Load<SpriteFont>("gameFont");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, this.Width, this.Height), Color.White);
            spriteBatch.DrawString(Font, Text, Position + new Vector2(10, 5), Color.White);

            spriteBatch.End();
        }
    }
}
