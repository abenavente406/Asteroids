using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameHelperLibrary.Controls
{
    public class Label : Control
    {
        #region Constructor Region

        public Label()
        {
            tabStop = false;
        }

        #endregion

        #region Abstract Methods

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (Effect)
            {
                case ControlEffect.NONE:
                    {
                        spriteBatch.DrawString(SpriteFont, Text, Position, Color);
                        break;
                    }
                case ControlEffect.FLASH:
                    {
                        if (gameTime.TotalGameTime.Milliseconds % flashDuration > flashDuration / 2)
                            spriteBatch.DrawString(spriteFont, Text, Position, Color);
                        break;
                    }
                case ControlEffect.GLOW:
                    {
                        spriteBatch.DrawString(SpriteFont, text, Position, Color);
                        spriteBatch.DrawString(SpriteFont, text, position, Overlay);

                        if (!reverse)
                            overlay.A += glowSpeed;
                        else
                            overlay.A -= glowSpeed;

                        if (overlay.A > 250)
                        {
                            overlay.A = 250;
                            reverse = true;
                        }
                        else if (overlay.A < glowSpeed)
                        {
                            overlay.A = glowSpeed;
                            reverse = false;
                        }

                        break;
                    }
            }
        }

        public override void HandleInput(PlayerIndex playerIndex)
        {
        }

        #endregion
    }
}
