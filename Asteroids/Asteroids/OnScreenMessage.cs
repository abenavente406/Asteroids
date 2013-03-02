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

namespace Asteroids
{
    class OnScreenMessage
    {
        private SpriteFont font;

        private string message  = null;
        private float  duration = 0;
        private float  ticks = 0;

        public float   Duration { get { return duration; } private set { duration = value; } }
        public string  Message  { get { return message; } private set { message = value; } }
        public bool finished = false;

        public OnScreenMessage(string message, float duration, SpriteFont font)
        {
            Message = message;
            Duration = duration;
            this.font = font;
        }

        public void Display(Vector2 position, SpriteBatch batch, GameTime gameTime)
        {
            ticks += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (ticks < duration)
            {
                float opacity = (duration - ticks) / (duration);
                batch.DrawString(font, Message, position, Color.White * opacity);
            }
            else
            {
                finished = true;
                ticks = 0;
            }
        }
    }
}
