using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace eggsss
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Catcher cather;
        Texture2D mainBackground;
        private bool isPause;
        Random random;

        //Number that holds the player score
        int score;
        // The font used to display UI elements
        SpriteFont font;

        public Game1(Random random)
        {
            this.random = random;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            cather = new Catcher();
            score = 0;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                isPause = true;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                isPause = false;

            if (!isPause)
            {
                UpdateCatcher(gameTime);
            }
            base.Update(gameTime);
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

        private void AddEgg()
        {
            Vector2 position = random.Next(100, GraphicsDevice.Viewport.Height - 100));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to the active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEgg(GameTime gameTime)
        {
            // Spawn a new enemy enemy every 1.5 seconds
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;

                // Add an Enemy
                AddEnemy();
            }

            // Update the Enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);

                if (enemies[i].Active == false)
                {
                    // If not active and health <= 0
                    if (enemies[i].Health <= 0)
                    {
                        // Add an explosion
                        AddExplosion(enemies[i].Position);

                        // Play the explosion sound
                        explosionSound.Play();

                        //Add to the player's score
                        score += enemies[i].Value;
                    }

                    enemies.RemoveAt(i);
                }
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
            
            // Draw the score
            spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            // Draw the player health
            spriteBatch.DrawString(font, "health: " + cather.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
