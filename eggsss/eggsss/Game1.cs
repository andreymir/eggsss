using System;
using System.Collections.Generic;
using KinectActionCapture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace eggsss
{
    public class Game1 : Game
    {
        private const int MIN_EGG_SPAWN_TIME = 1000;
        private const int MAX_EGG_PACE = 400;

        private SoundEffect[] moveSounds;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Catcher cather;
        private Texture2D mainBackground;
        private bool isPause;
        private Random random;

        private Texture2D[][] eggTextures;
        private List<Egg> eggs;
        private TimeSpan eggSpawnTime;
        private TimeSpan previousEggTime;
        private TimeSpan eggPace;

        private SoundEffect explosionSound;
        private SoundEffect catchSound;
        private SoundEffect crashSound;
        private SoundEffect gameOverSound;
        private KinectManager kinect;
        private StartResult kinectStartState;

        int score;
        int prevScore;
        SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Start();
            base.Initialize();
        }

        private void Start()
        {
            cather = new Catcher();
            moveSounds = new SoundEffect[4];
            score = 0;
            random = new Random();
            eggs = new List<Egg>();
            kinect = new KinectManager();
            kinectStartState = kinect.StartKinect();

            // Initial egg spawn time
            eggSpawnTime = TimeSpan.FromSeconds(4);
            previousEggTime = TimeSpan.FromSeconds(-3);
            // Initial egg pace
            eggPace = TimeSpan.FromSeconds(1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            kinectStartState = kinect.StartKinect();
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
                }
            };

            catchSound = Content.Load<SoundEffect>("sound/catchSound");
            crashSound = Content.Load<SoundEffect>("sound/crashSound");
            moveSounds[0] = Content.Load<SoundEffect>("sound/moveSound0");
            moveSounds[1] = Content.Load<SoundEffect>("sound/moveSound1");
            moveSounds[2] = Content.Load<SoundEffect>("sound/moveSound2");
            moveSounds[3] = Content.Load<SoundEffect>("sound/moveSound3");
            gameOverSound = Content.Load<SoundEffect>("sound/gameOverSound");
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                isPause = true;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                isPause = false;

            if (isPause)
            {
                return;
            }

            UpdateCatcher(gameTime);
            UpdateEggs(gameTime);

            base.Update(gameTime);
        }

        private void UpdateEggs(GameTime gameTime)
        {
            // Add new egg
            if (gameTime.TotalGameTime - previousEggTime > eggSpawnTime)
            {
                AddEgg(gameTime);
                previousEggTime = gameTime.TotalGameTime;
            }

            // Check eggs
            for (int i = eggs.Count - 1; i >= 0; i--)
            {
                var egg = eggs[i];

                if (egg.StepNumber == 4 && egg.TrayNumber == cather.State) // ?????
                {
                    eggs.RemoveAt(i);
                    catchSound.Play();
                    score++;
                    UpdateSpeed();
                }
                else
                {
                    egg.Update(gameTime);

                    if (egg.Crushed)
                    {
                        eggs.RemoveAt(i);

                        AddCrushedEgg(eggs[i].TrayNumber);
                        cather.Health--;
                        crashSound.Play();
                    }
                }
            }

            CheckGameOver();
        }

        private void CheckGameOver()
        {
            if (cather.Health == 0)
            {
                Debug.Print("Game over!");

                gameOverSound.Play();
                Restart();
            }
        }

        private void UpdateSpeed()
        {
            var t = TimeSpan.FromMilliseconds(MIN_EGG_SPAWN_TIME);
            if (score % 5f == 0 && eggSpawnTime > t)
            {
                eggSpawnTime = eggSpawnTime.Subtract(TimeSpan.FromMilliseconds(1000));

                if (eggSpawnTime < t)
                {
                    eggSpawnTime = t;
                }
            }

            t = TimeSpan.FromMilliseconds(MAX_EGG_PACE);
            if (score % 5f == 0 && eggPace > t)
            {
                eggPace = eggPace.Subtract(TimeSpan.FromMilliseconds(100));
                if (eggPace < t)
                {
                    eggPace = t;
                }

                foreach (var egg in eggs)
                {
                    if (egg.StepNumber < 4)
                    {
                        egg.Pace = eggPace;
                    }
                }
            }

            //eggSpawnTime = TimeSpan.FromMilliseconds(900);
            //eggPace = TimeSpan.FromMilliseconds(300);

            Debug.Print("Level up! Spawn time: {0}; Egg pace: {1}", eggSpawnTime, eggPace);
        }

        private void Restart()
        {
            score = 0;
            cather.Health = 3;
        }

        private void AddCrushedEgg(CatcherState position)
        {
            //throw new NotImplementedException();
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

            DrawText();

            DrawEggs();

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawEggs()
        {
            foreach (var egg in eggs)
            {
                egg.Draw(spriteBatch);
            }
        }

        private void DrawText()
        {
            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + cather.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);
        }

        private void AddEgg(GameTime gameTime)
        {
            var egg = new Egg();
            var textureSet = eggTextures[random.Next(3)];
            var eggPace = TimeSpan.FromMilliseconds(1000);
            var trayNumber = (CatcherState)random.Next(1, 4);
            egg.Initialize(GraphicsDevice.Viewport.TitleSafeArea,
                textureSet, moveSounds, trayNumber, gameTime.TotalGameTime, eggPace);
            eggs.Add(egg);
        }
    }
}
