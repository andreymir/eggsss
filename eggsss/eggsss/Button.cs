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
        private Texture2D voiceTexture;
        private Texture2D bg_l;
        private Texture2D bg_c;
        private Texture2D bg_r;

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
            Width = (int)Math.Ceiling(size.X) + bg_l.Width + bg_r.Width + voiceTexture.Width;
            Height = bg_c.Height;
        }

        public string Text { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public SpriteFont Font { get; set; }
        public Vector2 Position { get; set; }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            bg_l = this.Game.Content.Load<Texture2D>("Button/button-l");
            bg_c = this.Game.Content.Load<Texture2D>("Button/button-c");
            bg_r = this.Game.Content.Load<Texture2D>("Button/button-r");
            voiceTexture = this.Game.Content.Load<Texture2D>("Button/voice");
            Font = this.Game.Content.Load<SpriteFont>("Button/buttonFont");
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(bg_l, Position, Color.White);
            spriteBatch.Draw(bg_c, new Rectangle((int)Position.X + bg_r.Width, (int)Position.Y, 
                Width - bg_l.Width - bg_r.Width, bg_c.Height), Color.White);
            spriteBatch.Draw(bg_r, new Vector2(Position.X + this.Width - bg_r.Width, Position.Y), Color.White);
            spriteBatch.Draw(voiceTexture, new Vector2(Position.X + Width - bg_r.Width - voiceTexture.Width, Position.Y), Color.White);
            spriteBatch.DrawString(Font, Text, Position + new Vector2(bg_l.Width, -4), Color.White);

            spriteBatch.End();
        }
    }
}
