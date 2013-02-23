using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroids.Objects
{
    public class Asteroid : GameObject
    {
        public override Vector2 Speed
        {
            get { return base.Speed; }
            set
            {
                // Keeps the speed wrapped around a set top speed
                base.Speed = Vector2.Clamp(value, new Vector2(-topSpeed, -topSpeed), new Vector2(topSpeed, topSpeed));
            }
        }

        public AsteroidManager.Size size;

        /// <summary>
        /// Creates an asteroid
        /// </summary>
        /// <param name="content"></param>
        /// <param name="origin">Position on screen</param>
        /// <param name="speed"></param>
        /// <param name="angle"></param>
        public Asteroid(Texture2D texture, Vector2 origin, Vector2 speed, float angle, AsteroidManager.Size size)
            : base(origin)
        {
            // Make sure you set the top speed!
            topSpeed = 50f;

            Position = origin;
            Speed = speed;
            MovementAngle = angle;
            switch (size)
            {
                case (AsteroidManager.Size.SMALL):
                    _mass = (float)new Random().NextDouble() * 5.0f + 5.0f;
                    break;
                case (AsteroidManager.Size.LARGE):
                    _mass = (float)new Random().NextDouble() * 10.0f + 5.0f;
                    break;
            }
            this.size = size;
            SetTexture(texture);
        }

        public override void Update(GameTime gameTime)
        {
            // Get the angular velocity from the movement angle
            Vector2 speed = new Vector2(Speed.X * (float)Math.Cos(MovementAngle),
                                        Speed.Y * (float)Math.Sin(MovementAngle));
            ApplyForce(speed);
            Position += Acceleration;
            Acceleration *= 0;
        }
    }
}
