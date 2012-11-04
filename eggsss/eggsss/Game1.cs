using System;
using System.Collections.Generic;
using KinectActionCapture;
using KinectActionCapture.Game;
using KinectActionCapture.VoiceManipulation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace eggsss
{
    public class Game1 : Game
    {
        private const int MIN_EGG_SPAWN_TIME = 1500;
        private const int MAX_EGG_PACE = 500;
        private const int MAX_EGG_COUNT = 8;

        private SoundEffect[] moveSounds;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Catcher cather;
        private Health health;
        private Texture2D mainBackground;
        private Random random;

        private Texture2D[][] eggTextures;
        private Texture2D[] crushedEggTextures;
        private List<Egg> eggs;
        private List<CrushedEgg> crushedEggs;
        private TimeSpan eggSpawnTime;
        private TimeSpan previousEggTime;
        private TimeSpan eggPace;
        private CatcherState prevTrayNumber = CatcherState.Unknown;

        private SoundEffect catchSound;
        private SoundEffect crashSound;
        private SoundEffect gameOverSound;
        private KinectManager kinect;
        private StartResult kinectStartState;

        private int score;
        private readonly HighScoreData highScore = new HighScoreData();
        SpriteFont font;
        private Vector2 playerPosition;

        // Buttons
        private Button newGameButton;
        private readonly Button pauseButton;
        private readonly Button continueButton;
        private Button exitButton;

        // Pause
        private EggGameState eggGameState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
                           {
                               IsFullScreen = false,
                               PreferredBackBufferHeight = 768,
                               PreferredBackBufferWidth = 1024
                           };
            Content.RootDirectory = "Content";

            newGameButton = CreateButtonSprite(Resources.NewGame, new Vector2(50, 710));
            pauseButton = CreateButtonSprite(Resources.Pause, new Vector2(346, 710));
            continueButton = CreateButtonSprite(Resources.Continue, new Vector2(346, 710));
            continueButton.Visible = false;
            exitButton = CreateButtonSprite(Resources.Exit, new Vector2(800, 710));
        }

        protected override void Initialize()
        {
            cather = new Catcher();
            health = new Health();
            moveSounds = new SoundEffect[4];
            crushedEggTextures = new Texture2D[2];
            random = new Random();
            eggs = new List<Egg>();
            crushedEggs = new List<CrushedEgg>(5);
            kinect = new KinectManager();
            kinectStartState = kinect.StartKinect();
            highScore.Initialize();
            this.Restart();

            base.Initialize();
        }

        private Button CreateButtonSprite(string text, Vector2 position)
        {
            var button = new Button(this, text, position);
            Components.Add(button);
            return button;
        }

        protected override void LoadContent()
        {
            kinectStartState = kinect.StartKinect();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            playerPosition =
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

            crushedEggTextures[0] = Content.Load<Texture2D>("crash/crash-left");
            crushedEggTextures[1] = Content.Load<Texture2D>("crash/crash-right");

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

            health.Initialize(Content.Load<Texture2D>("penalty/penalty"), playerPosition);

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
            highScore.SaveHighScore();
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
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Pause();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                eggGameState = EggGameState.Game;
            }

            if (kinectStartState == StartResult.ManyKinectsDetected || kinectStartState == StartResult.KinectError)
                Pause();

            if (kinectStartState == StartResult.KinectStarted)
            {
                var kinectCurrentVoice = kinect.GetCurrentVoiceCommand();
                switch (kinectCurrentVoice)
                {
                    case VoiceCommand.NewGame:
                        Restart();
                        break;
                    case VoiceCommand.Exit:
                        Exit();
                        break;
                    case VoiceCommand.Pause:
                        Pause();
                        break;
                    case VoiceCommand.Continue:
                        eggGameState = EggGameState.Game;
                        break;
                }
            }

            if (eggGameState == EggGameState.Pause)
            {
                pauseButton.Visible = false;
                continueButton.Visible = true;
                return;
            }
            else
            {
                pauseButton.Visible = true;
                continueButton.Visible = false;
            }

            UpdateCatcher(gameTime);
            UpdateEggs(gameTime);
            UpdateCrushedEggs(gameTime);

            base.Update(gameTime);
        }

        private void Pause()
        {
            eggGameState = EggGameState.Pause;
        }

        private void UpdateEggs(GameTime gameTime)
        {
            if (eggs.Count <= MAX_EGG_COUNT & gameTime.TotalGameTime - previousEggTime > eggSpawnTime)
            {
                AddEgg(gameTime);
                previousEggTime = gameTime.TotalGameTime;
            }

            for (var i = eggs.Count - 1; i >= 0; i--)
            {
                var egg = eggs[i];

                if (egg.StepNumber == 4 && egg.TrayNumber == cather.State) // catch egg
                {
                    eggs.RemoveAt(i);
                    catchSound.Play();
                    SetScore();
                    UpdateSpeed();
                }
                else
                {
                    egg.Update(gameTime);

                    if (egg.Crushed)
                    {
                        AddCrushedEgg(gameTime, eggs[i].TrayNumber);
                        eggs.Clear();
                        health.Update();
                        crashSound.Play();
                    }
                }
            }

            CheckGameOver();
        }

        private void SetScore()
        {
            score++;
            if (score > highScore.Score)
                highScore.Score = score;
        }

        private void UpdateCrushedEggs(GameTime gameTime)
        {
            for (int i = crushedEggs.Count - 1; i >= 0; i--)
            {
                crushedEggs[i].Update(gameTime);
                if (crushedEggs[i].Active == false)
                    crushedEggs.RemoveAt(i);
            }
        }

        private void CheckGameOver()
        {
            if (health.Value == 0)
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
                eggSpawnTime = eggSpawnTime.Subtract(TimeSpan.FromMilliseconds(500));
                if (eggSpawnTime < t)
                {
                    eggSpawnTime = t;
                }
            }
            else if (score > 100 && eggSpawnTime > TimeSpan.FromMilliseconds(1000))
            {
                // More eggs!!!
                eggSpawnTime = TimeSpan.FromMilliseconds(1000);
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
            else if (score > 150 && eggPace > TimeSpan.FromMilliseconds(400))
            {
                // Fast eggs!!!
                eggPace = TimeSpan.FromMilliseconds(400);
            }

            //eggSpawnTime = TimeSpan.FromMilliseconds(1000);
            //eggPace = TimeSpan.FromMilliseconds(400);

            Debug.Print("Level up! Spawn time: {0}; Egg pace: {1}", eggSpawnTime, eggPace);
        }

        private void Restart()
        {
            // Initial egg spawn time
            eggSpawnTime = TimeSpan.FromSeconds(4);
            previousEggTime = TimeSpan.FromSeconds(-3);
            // Initial egg pace
            eggPace = TimeSpan.FromSeconds(1);

            score = 0;
            health.Clear();
            eggs.Clear();
            crushedEggs.Clear();
        }

        private void AddCrushedEgg(GameTime gameTime, CatcherState position)
        {
            var crushedEgg = new CrushedEgg();
            crushedEgg.Initialize(crushedEggTextures, playerPosition, position, gameTime.TotalGameTime);
            crushedEggs.Add(crushedEgg);
        }

        private void UpdateCatcher(GameTime gameTime)
        {
            var state = CatcherState.Unknown;
            if (kinectStartState == StartResult.KinectStarted)
            {
                var kinectState = kinect.GetCurrentGameState();

                switch (kinectState)
                {
                    case GameState.SecondAreaSelected:
                        state = CatcherState.TopLeft;
                        break;
                    case GameState.ThirdAreaSelected:
                        state = CatcherState.TopRight;
                        break;
                    case GameState.FirstAreaSelected:
                        state = CatcherState.BottomLeft;
                        break;
                    case GameState.FourthAreaSelected:
                        state = CatcherState.BottomRight;
                        break;
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

            // Draw the explosions
            foreach (var crushedEgg in crushedEggs)
            {
                crushedEgg.Draw(spriteBatch);
            }

            health.Draw(spriteBatch);

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
            spriteBatch.DrawString(font, score.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + GraphicsDevice.Viewport.TitleSafeArea.Width / 2 + 100, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), new Color(43,42,41));

            spriteBatch.DrawString(font, "Top:" + highScore.Score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100 , GraphicsDevice.Viewport.TitleSafeArea.Y + 30), new Color(43,42,41));
        }

        private void AddEgg(GameTime gameTime)
        {
            var egg = new Egg();
            var textureSet = eggTextures[random.Next(3)];
            var pace = TimeSpan.FromMilliseconds(1000);
            CatcherState trayNumber;
            do
            {
                trayNumber = (CatcherState)random.Next(1, 4);
            }
            while (trayNumber == prevTrayNumber);

            egg.Initialize(GraphicsDevice.Viewport.TitleSafeArea,
                textureSet, moveSounds, trayNumber, gameTime.TotalGameTime, pace);
            eggs.Add(egg);
            prevTrayNumber = trayNumber;
        }
    }
}
