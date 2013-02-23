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

namespace Asteroids
{

    public enum GameStates
    {
        MAINMENU, PLAYING, ROUNDOVER, GAMEOVER
    }

    public class AsteroidGame : Microsoft.Xna.Framework.Game
    {

        public static GameStates gameState = GameStates.PLAYING;
        public static ContentManager content;
        public static Player player;

        public static int WindowWidth { get { return 1280; } }
        public static int WindowHeight { get { return 720; } }
        public static uint score = 0;
        public static uint level = 1;

        public static List<GameObject> objects = new List<GameObject>();
        //public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Shot> shots = new List<Shot>();

        private SpriteBatch spriteBatch;
        private GraphicsDeviceManager graphics;

        private SpriteFont font;

        private SpriteSheet explosionSheet;
        private Animation explosion;
        private ScrollingBackground scrollingBackground;

        private float deadTime = 0f;
        private int lastTime   = 0;

        private Texture2D background;

        AsteroidManager asteroids;

        OnScreenMessage test;

        public AsteroidGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            content = Content;

            // Reset the instance of the game field
            ResetGameFull();
            scrollingBackground = new ScrollingBackground();
            asteroids = new AsteroidManager(Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the explosion spritesheet and explosion animation
            explosionSheet = new SpriteSheet(Content.Load<Texture2D>("explosion_sheet"),
                90, 90, GraphicsDevice);
            explosion = new Animation(new Texture2D[] {explosionSheet.getSubImage(0, 0),
                explosionSheet.getSubImage(1, 0), explosionSheet.getSubImage(2, 0),
                explosionSheet.getSubImage(3, 0), explosionSheet.getSubImage(4, 0),
                explosionSheet.getSubImage(0, 1),
                explosionSheet.getSubImage(1, 1), explosionSheet.getSubImage(2, 1),
                explosionSheet.getSubImage(3, 1), explosionSheet.getSubImage(4, 1),
                Content.Load<Texture2D>("Blank") , Content.Load<Texture2D>("Blank")}, 100f);

            // Add the basic font to be used
            font = Content.Load<SpriteFont>("SpriteFont1");

            // Create the scrolling background
            background = Content.Load<Texture2D>("starfield_large");
            scrollingBackground.Load(GraphicsDevice, background);
            test = new OnScreenMessage("G A M E    O V E R !", 3000f, font);
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Calculates updates per second and outputs it to the console
            int now = gameTime.TotalGameTime.Seconds;
            if (now != lastTime)
            {
                Console.WriteLine("UPS: " + Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds));
                lastTime = now;
            }


            // To make sure that the background moves at the right speed
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * 10;
            scrollingBackground.Update(elapsed);

            // Checks the state of the game
            // The game updates like normal during playing and round over
            if (gameState == GameStates.PLAYING || gameState == GameStates.ROUNDOVER)
            {

                if (AsteroidManager.asteroids.Count == 0)
                    ResetGamePart();
                asteroids.Update(gameTime);

                // Only update when the player has lives, and it isn't a round over
                if (gameState == GameStates.PLAYING)
                    if (!player.gameOver)
                        player.Update(gameTime);
                    else
                    {
                        gameState = GameStates.GAMEOVER;
                        return;
                    }

                // Update each shot in the List<Shot>
                shots.ForEach(delegate(Shot s) { s.Update(gameTime); });

                // If the player has died, the round is over
                if (!player.alive)
                    gameState = GameStates.ROUNDOVER;

                if (gameState == GameStates.ROUNDOVER)
                {
                    // Start a timer for how long the explosion animation goes on
                    if (deadTime < 75)
                    {
                        deadTime++;
                        if (deadTime > 60)
                            explosion.currentFrame = explosion.images.Length - 2;
                    }
                    else    // If the timer reaches the limit, reset timer and game
                    {
                        player.Reset();
                        deadTime = 0;
                        gameState = GameStates.PLAYING;
                    }
                }

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            {

                // Draw the scrolling background in the back (of course)
                scrollingBackground.Draw(spriteBatch);

                if (gameState == GameStates.PLAYING || gameState == GameStates.ROUNDOVER)
                {
                    // Draw the player, whether it be the ship or explosion
                    if (player.alive)
                        player.Draw(spriteBatch, gameTime);
                    else
                        explosion.Draw(spriteBatch, gameTime, player.Position - player.TextureOrigin);

                    foreach (Shot s in shots)
                        s.Draw(spriteBatch, gameTime);

                    // Draw each object in the List<GameObject>
                    asteroids.Draw(spriteBatch, gameTime);
                }
                if (gameState == GameStates.GAMEOVER)
                {
                    test.Display(new Vector2(WindowWidth / 2 - font.MeasureString(test.Message).X / 2,
                        WindowHeight / 2 - 100), spriteBatch, gameTime);

                    if (test.finished)
                    {
                        gameState = GameStates.PLAYING;
                        test.finished = false;
                        ResetGameFull();
                    }
                }

                // Draw information to the screen
                spriteBatch.DrawString(font, "S C O R E : " + GetScore(), new Vector2((WindowWidth / 2) -
                    (font.MeasureString("S C O R E : " + GetScore()).X / 2), 10), Color.White);
                spriteBatch.DrawString(font, "L I V E S : " + player.lives, new Vector2(10, WindowHeight - 30),
                    Color.White);
                spriteBatch.DrawString(font, "L E V E L : " + level, new Vector2(10, WindowHeight - 100),
                    Color.White);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Resets the game field
        /// </summary>
        public void ResetGameFull()
        {
            score = 0;
            level = 1;

            Random rand = new Random();
            player = new Player(content);
            asteroids = new AsteroidManager(content);
            shots = new List<Shot>();
        }
        public void ResetGamePart()
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
    }
}
