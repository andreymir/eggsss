using System;
using System.Collections.Generic;
using KinectActionCapture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace eggsss
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Catcher cather;
        Texture2D mainBackground;
        private bool isPause;
        private Random random;
        private Texture2D[][] eggTextures;
        private List<Egg> eggs;
        private TimeSpan eggSpawnTime;
        private TimeSpan previousEggTime;
        SoundEffect explosionSound;
        private KinectManager kinect;
        private StartResult kinectStartState;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            cather = new Catcher();
            score = 0;
            random = new Random();
            eggs = new List<Egg>();
            kinect = new KinectManager();
            kinectStartState = kinect.StartKinect();

            // Initial egg spawn time
            eggSpawnTime = new TimeSpan(2 * TimeSpan.TicksPerSecond); // 2 seconds

            base.Initialize();
        }

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

            mainBackground = Content.Load<Texture2D>("mainbackground");

            // Load the score font
            font = Content.Load<SpriteFont>("gameFont");
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
            kinect.StopKinect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                isPause = true;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                isPause = false;

            if (!isPause)
            {
            // Update the player
            UpdateCatcher(gameTime);

            // Update eggs
            UpdateEggs(gameTime);

                UpdateCatcher(gameTime);
            }
            base.Update(gameTime);
        }

        private void UpdateEggs(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousEggTime > eggSpawnTime)
            {
                AddEgg();
                previousEggTime = eggSpawnTime;
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
            var state = CatcherState.Unknown;
            if (kinectStartState == StartResult.KinectStarted)
            {
                var kinectState = kinect.GetCurrentGameState();

                if (kinectState == GameState.SecondAreaSelected)
                {
                    state = CatcherState.TopLeft;
                }
                else if (kinectState == GameState.ThirdAreaSelected)
                {
                    state = CatcherState.TopRight;
                }
                else if (kinectState == GameState.FirstAreaSelected)
                {
                    state = CatcherState.BottomLeft;
                }
                else if (kinectState == GameState.FourthAreaSelected)
                {
                    state = CatcherState.BottomRight;
                }
            }
            else
            {
                var keyboardState = Keyboard.GetState();

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
            }

            if (state != CatcherState.Unknown)
            {
                cather.Update(state);
            }
        }

        private void UpdateEgg(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            //if (gameTime.TotalGameTime - previousSpawnTime > new TimeSpan(0,0,1))
            //{
            //    //previousSpawnTime = gameTime.TotalGameTime;

            //    // Add an Enemy
            //    AddEgg();
            //}

            // Update the Enemies
            for (int i = eggs.Count - 1; i >= 0; i--)
            {
                eggs[i].Update(gameTime);

                if (eggs[i].Active == false)
                {
                    // Add an explosion
                    AddExplosion(eggs[i].Position);

                    // Play the explosion sound
                    explosionSound.Play();

                    //Add to the player's score
                    score += eggs[i].Value;

                    eggs.RemoveAt(i);
                }
            }
        }

        private void AddExplosion(Vector2 position)
        {
            throw new NotImplementedException();
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

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the Player
            cather.Draw(spriteBatch);
            
            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + cather.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void AddEgg()
        {
            var egg = new Egg();
            var textureSet = eggTextures[random.Next(3)];
            egg.Initialize(GraphicsDevice.Viewport.TitleSafeArea, textureSet, random.Next(3));
            eggs.Add(egg);
        }
    }
}
