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
    public class Shot : GameObject
    {
        public int liveTime = 80;
        public bool live    = true;

        /// <summary>
        /// Creates a shot
        /// </summary>
        /// <param name="content"></param>
        /// <param name="player"></param>
        /// <param name="drawAngle">Angle to draw and propel the shot</param>
        public Shot(ContentManager content, Player player, double drawAngle)
            : base(player.Position)
        {
            Position = player.Position;
            MovementAngle = (float)drawAngle;
            DrawAngle = (float)drawAngle;
            Speed = new Vector2(0, 15.0f);

            _scale = 0.5f;
            SetTexture(content, "laserGreen");

            GameplayState.shots.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (liveTime > 0)
            {
                liveTime--;
                // Since speed is constant for shots, just add the speed to the position immediately
                Position += new Vector2(Speed.Y * (float)Math.Cos(MovementAngle), Speed.Y * (float)Math.Sin(MovementAngle));
            }
            else    // When the livetime reaches 0, remove it
            {
                GameplayState.objects.Remove(this);
                GameplayState.shots.Remove(this);
                live = false;
            }
        }
    }
}
