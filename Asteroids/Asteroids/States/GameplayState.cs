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
using Asteroids.Objects;
using GameHelperLibrary;
using Asteroids.States;

namespace Asteroids
{
    public class GameplayState : BaseGameState
    {
        private enum GameplayStates { PLAYING, ROUNDOVER, GAMEOVER }
        private GameplayStates gameplayState = GameplayStates.PLAYING;

        #region Fields
        public static uint score = 0;
        public static uint level = 1;
        private float lastTime = 0;

        public static List<GameObject> objects = new List<GameObject>();
        public static List<Shot> shots = new List<Shot>();

        private SpriteFont font;
        public Player player;
        private ScrollingBackground scrollingBackground;
        private Texture2D background;
        private AsteroidManager asteroids;
        private OnScreenMessage gameOverMessage;
        #endregion

        #region Initialization
        public GameplayState(Game game, GameStateManager manager) :
            base(game, manager)
        {
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            var Content = GameRef.Content;

            // Add the basic font to be used
            font = Content.Load<SpriteFont>("menuFont");

            // Create the scrolling background
            background = Content.Load<Texture2D>("starfield_large");
            scrollingBackground = new ScrollingBackground();
            scrollingBackground.Load(GraphicsDevice, background);

            // Reset the instance of the game field
            ResetGameFull();

            gameOverMessage = new OnScreenMessage("G A M E    O V E R !", 3000f, font);
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            // Calculates updates per second and outputs it to the console
            Console.WriteLine("UPS: " + AsteroidGame.CalculateFPS(gameTime, ref lastTime));

            // Update the scrolling background
            scrollingBackground.Update((float)gameTime.ElapsedGameTime.TotalSeconds * 10);

            // Checks the state of the game
            switch (gameplayState)
            {
                case GameplayStates.PLAYING:
                    {
                        if (AsteroidManager.asteroids.Count == 0)
                            LevelUp();
                        asteroids.Update(gameTime);

                        if (player.gameOver)
                        {
                            gameplayState = GameplayStates.GAMEOVER;
                            return;
                        }

                        player.Update(gameTime);
                        shots.ForEach(delegate(Shot s) { s.Update(gameTime); });
                        break;
                    }
                case GameplayStates.GAMEOVER:
                    {
                        break;
                    }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = GameRef.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            {
                base.Draw(gameTime);

                // Draw the scrolling background in the back (of course)
                scrollingBackground.Draw(spriteBatch);

                switch (gameplayState)
                {
                    case GameplayStates.PLAYING:
                        {
                            player.Draw(spriteBatch, gameTime);
                            foreach (Shot s in shots)
                                s.Draw(spriteBatch, gameTime);
                            asteroids.Draw(spriteBatch, gameTime);
                            break;
                        }
                    case GameplayStates.GAMEOVER:
                        {
                            gameOverMessage.Display(new Vector2(AsteroidGame.ScreenBounds.Width / 2 - font.MeasureString(gameOverMessage.Message).X / 2,
                                AsteroidGame.ScreenBounds.Height / 2 - 100), spriteBatch, gameTime);

                            if (gameOverMessage.finished)
                            {
                                gameplayState = GameplayStates.PLAYING;
                                gameOverMessage.finished = false;
                                ResetGameFull();
                            }
                            break;
                        }
                }

                // Draw information to the screen
                spriteBatch.DrawString(font, "S C O R E : " + GetScore(), new Vector2((AsteroidGame.ScreenBounds.Width / 2) -
                    (font.MeasureString("S C O R E : " + GetScore()).X / 2), 10), Color.White);
                spriteBatch.DrawString(font, "L I V E S : " + player.lives, new Vector2(10, AsteroidGame.ScreenBounds.Height - 30),
                    Color.White);
                spriteBatch.DrawString(font, "L E V E L : " + level, new Vector2(10, AsteroidGame.ScreenBounds.Height - 100),
                    Color.White);

                FadeOutRect.Draw(spriteBatch, Vector2.Zero, FadeOutColor);
            }
            spriteBatch.End();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Resets the game field
        /// </summary>
        public void ResetGameFull()
        {
            score = 0;
            level = 1;

            Random rand = new Random();
            player = new Player(GameRef.Content);
            asteroids = new AsteroidManager(GameRef.Content);
            shots = new List<Shot>();
        }
        public void LevelUp()
        {
            level++;

            Random rand = new Random();
            player.Reset();
            asteroids.Reset();
            shots = new List<Shot>();
        }

        private string GetScore()
        {
            return score.ToString();
        }
        #endregion
    }
}
