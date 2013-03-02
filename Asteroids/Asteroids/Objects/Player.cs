using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids.Objects
{
    public class Player : GameObject
    {
        KeyboardState newState;
        ContentManager gameContent;

        public bool alive    = true;
        public bool gameOver = false;
        public int lives     = 3;

        private float thrustSpeed = 4.5f;
        private int shotCoolDown  = 0;
        private int shotCoolMax   = 10;

        public override Vector2 Speed
        {
            get { return base.Speed; }
            set
            {
                base.Speed = Vector2.Clamp(value, new Vector2(-topSpeed, -topSpeed), 
                             new Vector2(topSpeed, topSpeed));
            }
        }

        public Player(ContentManager content)
            : base(new Vector2(AsteroidGame.WindowWidth / 2, AsteroidGame.WindowHeight / 2))
        {
            topSpeed = 4;
            SetTexture(content, "player");
            gameContent = content;
            _scale = 0.65f;
            _mass = 40;
        }

        public override void Update(GameTime gameTime)
        {
            newState = Keyboard.GetState();

            if (lives == 0)
                gameOver = true;

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

            if (newState.IsKeyDown(Keys.Up))
            {
                MovementAngle = DrawAngle;
                // Get the angular velocity from the movement angle
                Vector2 thrust = new Vector2(thrustSpeed * (float)Math.Cos(MovementAngle), 
                                             thrustSpeed * (float)Math.Sin(MovementAngle));
                ApplyForce(thrust);
            }

            if (newState.IsKeyDown(Keys.Right))
                DrawAngle += rotationSpeed;
            else if (newState.IsKeyDown(Keys.Left))
                DrawAngle -= rotationSpeed;

            Speed    += Acceleration;
            Position += Speed;

            Acceleration *= 0;  // Reset the acceleration with each iteration

            if (newState.IsKeyDown(Keys.Space) && shotCoolDown == 0)
            {
                new Shot(gameContent, this, DrawAngle);
                shotCoolDown = shotCoolMax;
            }
        }

        /// <summary>
        /// Resets the player's position
        /// </summary>
        public void Reset()
        {
            alive = true;
            Position = new Vector2(AsteroidGame.WindowWidth / 2, AsteroidGame.WindowHeight / 2);
            Speed = Vector2.Zero;
            DrawAngle = 0.0f;
            MovementAngle = 0.0f;
        }
    }
}
