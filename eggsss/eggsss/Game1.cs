using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace eggsss
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Catcher cather;
        private Random random;
        private Texture2D[][] eggTextures;
        private List<Egg> eggs;
        private TimeSpan eggSpawnTime;
        private TimeSpan previousEggTime;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize the player class
            cather = new Catcher();
            random = new Random();
            eggs = new List<Egg>();

            // Initial egg spawn time
            eggSpawnTime = new TimeSpan(5 * TimeSpan.TicksPerSecond); // 2 seconds

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var playerPosition =
                new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height);

            cather.Initialize(
                new[] {
                    Content.Load<Texture2D>("Catcher/catcher-left"),
                    Content.Load<Texture2D>("Catcher/catcher-right"),
                    Content.Load<Texture2D>("Catcher/catcher-ltop"),
                    Content.Load<Texture2D>("Catcher/catcher-lbottom"),
                    Content.Load<Texture2D>("Catcher/catcher-rtop"),
                    Content.Load<Texture2D>("Catcher/catcher-rbottom"),
            },
            playerPosition,
            CatcherState.TopLeft);

            eggTextures = new[]
            {
                new []
                {
                    Content.Load<Texture2D>("Egg/0-0"),
                    Content.Load<Texture2D>("Egg/0-1"),
                    Content.Load<Texture2D>("Egg/0-2"),
                    Content.Load<Texture2D>("Egg/0-3"),
                    Content.Load<Texture2D>("Egg/0-4"),
                },
                new []
                {
                    Content.Load<Texture2D>("Egg/1-0"),
                    Content.Load<Texture2D>("Egg/1-1"),
                    Content.Load<Texture2D>("Egg/1-2"),
                    Content.Load<Texture2D>("Egg/1-3"),
                    Content.Load<Texture2D>("Egg/1-4"),
                },
                new []
                {
                    Content.Load<Texture2D>("Egg/2-0"),
                    Content.Load<Texture2D>("Egg/2-1"),
                    Content.Load<Texture2D>("Egg/2-2"),
                    Content.Load<Texture2D>("Egg/2-3"),
                    Content.Load<Texture2D>("Egg/2-4"),
                },
                new []
                {
                    Content.Load<Texture2D>("Egg/3-0"),
                    Content.Load<Texture2D>("Egg/3-1"),
                    Content.Load<Texture2D>("Egg/3-2"),
                    Content.Load<Texture2D>("Egg/3-3"),
                    Content.Load<Texture2D>("Egg/3-4"),
                },
            };
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().GetPressedKeys().FirstOrDefault() == Keys.Escape)
                this.Exit();

            // Update the player
            UpdateCatcher(gameTime);

            // Update eggs
            UpdateEggs(gameTime);

            base.Update(gameTime);
        }

        private void UpdateEggs(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousEggTime > eggSpawnTime)
            {
                AddEgg(gameTime);
                previousEggTime = gameTime.TotalGameTime;
            }

            for (int i = eggs.Count - 1; i >= 0; i--)
            {
                var egg = eggs[i];
                egg.Update(gameTime);

                if (egg.Crushed)
                {
                    eggs.RemoveAt(i);

                    AddCrushedEgg();
                }
            }
        }

        private void AddCrushedEgg()
        {
            
        }

        private void UpdateCatcher(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var state = CatcherState.Unknown;

            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                state = CatcherState.TopLeft;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad5))
            {
                state = CatcherState.TopRight;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad1))
            {
                state = CatcherState.BottomLeft;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad2))
            {
                state = CatcherState.BottomRight;
            }

            if (state != CatcherState.Unknown)
            {
                cather.Update(state);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            // Draw the Player
            cather.Draw(spriteBatch);

            foreach (var egg in eggs)
            {
                egg.Draw(spriteBatch);
            }

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddEgg(GameTime gameTime)
        {
            var egg = new Egg();
            var textureSet = eggTextures[random.Next(3)];
            var eggPace = TimeSpan.FromMilliseconds(1000);
            egg.Initialize(GraphicsDevice.Viewport.TitleSafeArea, 
                textureSet, (CatcherState)random.Next(1, 4), gameTime.TotalGameTime, eggPace);
            eggs.Add(egg);
        }
    }
}
