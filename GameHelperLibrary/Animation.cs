using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameHelperLibrary {
    public class Animation {

        #region Properties
        // The current frame to render
        public int   CurrentFrame { get; set; }

        public float Timer { get { return _timer; } private set { _timer = value; } }
        public float Interval { get { return _interval; } set { _interval = value; } }

        public Texture2D[] Images { get { return _images; } }
        #endregion

        #region Fields
        // Number of frames in the animation
        private int frames = 0;

        private float _interval = 0;
        private float _timer    = 0;

        // The position to draw the animation on the screen
        private Vector2 position;
        // The images used in the animation
        private Texture2D[] _images;
        #endregion

        /// <summary>
        /// Creates an animation based on an array of images
        /// </summary>
        /// <param name="images">The array of images the animation is made from</param>
        /// <param name="interval">The amount of time between each frame (default: 100f)</param>
        public Animation(Texture2D[] images, float interval = 100f) {
            _interval = interval;
            frames = images.Length;
            _images = new Texture2D[frames];
            for (int i = 0; i < frames; i++) {
                _images[i] = images[i];
            }
        }


        /// <summary>
        /// Draws the animation to the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        /// <param name="position">The position to draw the animation</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, bool flipped = false, float scale = 1.0f)
        {
            Draw(spriteBatch, gameTime, position.X, position.Y, flipped, scale);
        }

        /// <summary>
        /// Draws the animation to the screen.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        /// <param name="x">X location to draw the animation</param>
        /// <param name="y">Y location to draw the animation</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float x, float y, bool flipped = false, float scale = 1.0f) {

            position = new Vector2(x, y);

            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
 
            //Check the timer is more than the chosen interval
            if (Timer > Interval) {
                 //Show the next frame
                    CurrentFrame++;
                 //Reset the timer
                    Timer = 0f;
            }

            // If we are on the last frame, reset back to the one before the first frame
            if (CurrentFrame == frames) {
                  CurrentFrame = 0;
            }

            if (flipped)
                spriteBatch.Draw(_images[CurrentFrame], position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
            else
                spriteBatch.Draw(_images[CurrentFrame], position, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Rectangle destRect)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            //Check the timer is more than the chosen interval
            if (Timer > Interval)
            {
                //Show the next frame
                CurrentFrame++;
                //Reset the timer
                Timer = 0f;
            }

            // If we are on the last frame, reset back to the one before the first frame
            if (CurrentFrame == frames)
            {
                CurrentFrame = 0;
            }

            spriteBatch.Draw(_images[CurrentFrame], destRect, Color.White);
        }

    }
}
