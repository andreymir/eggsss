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
        private Rectangle Window;
        private Texture2D[] EggTextures;
        public Vector2 Position { get; set; }


        public int StepNumber { get; set; }
        public int TrayNumber { get; set; }

        public bool Crushed;

        public void Initialize(Rectangle window, Texture2D[] textures, int trayNumber)
        {
            Window = window;
            EggTextures = textures;
            TrayNumber = trayNumber;
            StepNumber = 0;
            Crushed = false;
        }

        internal void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
