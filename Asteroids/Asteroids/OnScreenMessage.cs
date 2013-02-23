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

        private string _message  = null;
        private float  _duration = 0;
        private float  _ticks = 0;

        public float   Duration { get { return _duration; } private set { _duration = value; } }
        public string  Message  { get { return _message; } private set { _message = value; } }
        public bool finished = false;

        public OnScreenMessage(string message, float duration, SpriteFont font)
        {
            Message = message;
            Duration = duration;
            this.font = font;
        }

        public void Display(Vector2 position, SpriteBatch batch, GameTime gameTime)
        {
            _ticks += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_ticks < _duration)
            {
                float opacity = (_duration - _ticks) / (_duration);
                batch.DrawString(font, Message, position, Color.White * opacity);
            }
            else
            {
                finished = true;
                _ticks = 0;
            }
        }
    }
}
