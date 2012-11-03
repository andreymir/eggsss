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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Catcher cather;

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
            // TODO: Add your initialization logic here

            //Initialize the player class
            cather = new Catcher();

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

            // TODO: use this.Content to load your game content here
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

            // Read the current state of the keyboard and gamepad and store it
            //currentKeyboardState = Keyboard.GetState();


            //Update the player
            UpdateCatcher(gameTime);


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

            //// Get Thumbstick Controls
            //player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            //player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;

            //// Use the Keyboard / Dpad
            //if (currentKeyboardState.IsKeyDown(Keys.Left) ||
            //currentGamePadState.DPad.Left == ButtonState.Pressed)
            //{
            //    player.Position.X -= playerMoveSpeed;
            //}
            //if (currentKeyboardState.IsKeyDown(Keys.Right) ||
            //currentGamePadState.DPad.Right == ButtonState.Pressed)
            //{
            //    player.Position.X += playerMoveSpeed;
            //}
            //if (currentKeyboardState.IsKeyDown(Keys.Up) ||
            //currentGamePadState.DPad.Up == ButtonState.Pressed)
            //{
            //    player.Position.Y -= playerMoveSpeed;
            //}
            //if (currentKeyboardState.IsKeyDown(Keys.Down) ||
            //currentGamePadState.DPad.Down == ButtonState.Pressed)
            //{
            //    player.Position.Y += playerMoveSpeed;
            //}


            //// Make sure that the player does not go out of bounds
            //player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            //player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);

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

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
