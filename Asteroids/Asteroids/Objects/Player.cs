using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameHelperLibrary;

namespace Asteroids.Objects
{
    public class Player : GameObject
    {
        ContentManager gameContent;

        public bool alive    = true;
        public bool gameOver = false;
        public int lives     = 10;

        private float thrustSpeed = 4.5f;
        private int shotCoolDown  = 0;
        private int shotCoolMax   = 10;

        private int deadTime = 0;

        public override Vector2 Speed
        {
            get { return base.Speed; }
            set
            {
                base.Speed = Vector2.Clamp(value, new Vector2(-topSpeed, -topSpeed), 
                             new Vector2(topSpeed, topSpeed));
            }
        }

        SpriteSheet explosionSheet;
        Animation explosion;

        public Player(ContentManager content)
            : base(new Vector2(AsteroidGame.ScreenBounds.Width / 2, AsteroidGame.ScreenBounds.Height / 2))
        {
            topSpeed = 4;
            SetTexture(content, "player");
            gameContent = content;
            _scale = 0.65f;
            _mass = 40;

            // Load the explosion spritesheet and explosion animation
            explosionSheet = new SpriteSheet(content.Load<Texture2D>("explosion_sheet"),
                90, 90, AsteroidGame.graphics.GraphicsDevice);
            explosion = new Animation(new Texture2D[] {explosionSheet.GetSubImage(0, 0),
                explosionSheet.GetSubImage(1, 0), explosionSheet.GetSubImage(2, 0),
                explosionSheet.GetSubImage(3, 0), explosionSheet.GetSubImage(4, 0),
                explosionSheet.GetSubImage(0, 1),
                explosionSheet.GetSubImage(1, 1), explosionSheet.GetSubImage(2, 1),
                explosionSheet.GetSubImage(3, 1), explosionSheet.GetSubImage(4, 1),
                content.Load<Texture2D>("Blank") , content.Load<Texture2D>("Blank")}, 100f);
        }

        public override void Update(GameTime gameTime)
        {
            if (lives == 0)
            {
                gameOver = true;
                return;
            }

            if (!alive)
            {
                if (deadTime < 75)
                {
                    deadTime++;
                    if (deadTime > 60)      // Keep the explosion blank. Otherwise, it'll keep going
                        explosion.CurrentFrame = explosion.Images.Length - 2;
                    return;
                }
                else
                {
                    Reset();
                    deadTime = 0;
                }
            }

            if (shotCoolDown > 0)
                shotCoolDown--;

            AsteroidManager.asteroids.ForEach(delegate(Asteroid o)
            {
                if (Colliding(o))
                {
                    BounceObjects(this, o);
                    alive = false;
                    lives--;
                    return;
                }
            });

            if (InputHandler.KeyDown(Keys.Up))
            {
                MovementAngle = DrawAngle;
                // Get the angular velocity from the movement angle
                Vector2 thrust = new Vector2(thrustSpeed * (float)Math.Cos(MovementAngle), 
                                             thrustSpeed * (float)Math.Sin(MovementAngle));
                ApplyForce(thrust);
            }

            if (InputHandler.KeyDown(Keys.Right))
                DrawAngle += rotationSpeed;
            else if (InputHandler.KeyDown(Keys.Left))
                DrawAngle -= rotationSpeed;

            Speed    += Acceleration;
            Position += Speed;

            Acceleration *= 0;  // Reset the acceleration with each iteration

            if (InputHandler.KeyDown(Keys.Space) && shotCoolDown == 0)
            {
                new Shot(gameContent, this, DrawAngle);
                shotCoolDown = shotCoolMax;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime) 
        {
            if (alive)
                spriteBatch.Draw(texture, Position, null, Color.White, DrawAngle, TextureOrigin, _scale, SpriteEffects.None, 0f);
            else
                explosion.Draw(spriteBatch, gameTime, Position - TextureOrigin);
        }

        /// <summary>
        /// Resets the player's position
        /// </summary>
        public void Reset()
        {
            alive = true;
            Position = new Vector2(AsteroidGame.ScreenBounds.Width / 2, AsteroidGame.ScreenBounds.Height / 2);
            Speed = Vector2.Zero;
            DrawAngle = 0.0f;
            MovementAngle = 0.0f;
        }
    }
}
