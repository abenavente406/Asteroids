using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameHelperLibrary.Shapes;

namespace Asteroids.Objects
{
    public class AsteroidManager
    {
        public enum Size { SMALL, LARGE }

        public static List<Asteroid> asteroids = new List<Asteroid>();

        private Texture2D smallAsteroidTexture;
        private Texture2D largeAsteroidTexture;

        Random rand = new Random();
        private float topSpeed = 35f;

        public AsteroidManager(ContentManager content)
        {
            smallAsteroidTexture = content.Load<Texture2D>("meteorSmall");
            largeAsteroidTexture = content.Load<Texture2D>("meteorBig");
            Start();
        }

        private void Start()
        {
            asteroids.Clear();

            Random rand = new Random();
            Vector2 asteroidPos = Vector2.Zero;

            for (int asteroidCount = 0; asteroidCount < 2 + Math.Truncate(AsteroidGame.level * 1.3); asteroidCount++)
            {
                // Keep trying to assign a position until it is far enough from the player
                do
                {
                    asteroidPos = new Vector2(rand.Next(0, AsteroidGame.WindowWidth), rand.Next(0, AsteroidGame.WindowHeight));
                } while (Vector2.Distance(new Vector2(AsteroidGame.WindowWidth / 2, 
                    AsteroidGame.WindowHeight / 2), asteroidPos) < 200);
                asteroids.Add(new Asteroid(largeAsteroidTexture, asteroidPos, new Vector2(rand.Next(-(int)topSpeed, (int)topSpeed), 
                    rand.Next(-(int)topSpeed, (int)topSpeed)), (float)rand.NextDouble() * MathHelper.TwoPi, Size.LARGE));

            }
        }

        public void Update(GameTime gameTime)
        {

            asteroids.ForEach(delegate(Asteroid a) {
                AsteroidGame.shots.ForEach(delegate(Shot s)
                {
                    // If the asteroid intersects with a shot
                    if (a.Colliding(s))
                    {
                        DivideAsteroid(a, s);
                    }
                });
                a.Update(gameTime); });
        }

        public void Draw(SpriteBatch batch, GameTime gameTime)
        {
            asteroids.ForEach(delegate(Asteroid a) { a.Draw(batch, gameTime); });
        }

        public void Reset()
        {
            Start();
        }

        public void DivideAsteroid(Asteroid a, Shot s)
        {
            switch (a.size)
            {
                case Size.SMALL:
                    AsteroidGame.score += 100;
                    break;
                case Size.LARGE:
                    asteroids.Add(new Asteroid(smallAsteroidTexture, a.Position, new Vector2(
                        rand.Next(-(int)topSpeed, (int)topSpeed), rand.Next(-(int)topSpeed, (int)topSpeed)), 
                        (float)(rand.NextDouble() * MathHelper.PiOver2), Size.SMALL));
                    asteroids.Add(new Asteroid(smallAsteroidTexture, a.Position, new Vector2(
                        rand.Next(-(int)topSpeed, (int)topSpeed), rand.Next(-(int)topSpeed, (int)topSpeed)), 
                        (float)(rand.NextDouble() * MathHelper.PiOver2), Size.SMALL));
                    AsteroidGame.score += 200;
                    break;
            }

            asteroids.Remove(a);
            s.liveTime = 0;
            AsteroidGame.shots.Remove(s);
        }
    }
}
