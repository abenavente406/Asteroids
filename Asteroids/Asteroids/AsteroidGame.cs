using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameHelperLibrary;
using Asteroids.States;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    public class AsteroidGame : Game
    {
        public static GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        GameStateManager stateManager;

        public static Rectangle ScreenBounds;

        public AsteroidGame()
        {
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this);

            ScreenBounds = new Rectangle(0, 0, 1280, 720);

            graphics.PreferredBackBufferWidth = ScreenBounds.Width;
            graphics.PreferredBackBufferHeight = ScreenBounds.Height;
            graphics.ApplyChanges();

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            stateManager = new GameStateManager(this);

            Components.Add(new InputHandler(this));
            Components.Add(stateManager);

            stateManager.ChangeState(new GameplayState(this, stateManager));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);   
            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        public static float CalculateFPS(GameTime gameTime, ref float last)
        {
            // Calculates updates per second and outputs it to the console
            int now = gameTime.TotalGameTime.Seconds;
            if (now != last)
                last = now;

            return (float)Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}
