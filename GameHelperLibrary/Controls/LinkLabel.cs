using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameHelperLibrary.Controls
{
    public class LinkLabel : Control
    {
        #region Fields and Properties

        Color selectedColor = Color.CadetBlue;

        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        #endregion

        #region Constructor Region

        public LinkLabel()
        {
            TabStop = true;
            HasFocus = false;
            Position = Vector2.Zero;
        }

        #endregion

        #region Abstract Methods

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {
            if (hasFocus)
            {
                switch (Effect)
                {
                    case ControlEffect.NONE:
                        {
                            spriteBatch.DrawString(SpriteFont, Text, Position, selectedColor);
                            break;
                        }
                    case ControlEffect.FLASH:
                        {
                            if (gametime.TotalGameTime.Milliseconds  % flashDuration > flashDuration / 2)
                                spriteBatch.DrawString(SpriteFont, text, Position, selectedColor);
                            break;
                        }
                    case ControlEffect.GLOW:
                        {
                            if (!reverse)
                            {
                                spriteBatch.DrawString(SpriteFont, text, Position, selectedColor);
                                overlay.A += glowSpeed;
                                spriteBatch.DrawString(SpriteFont, text, position, Overlay);

                            }
                            else
                            {
                                spriteBatch.DrawString(SpriteFont, text, Position, selectedColor);
                                overlay.A -= glowSpeed;
                                spriteBatch.DrawString(SpriteFont, text, position, Overlay);

                            }

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
            else
            {
                spriteBatch.DrawString(SpriteFont, Text, Position, Color);
            }
        }

        public override void HandleInput(PlayerIndex playerIndex)
        {
            if (!HasFocus)
                return;

            if (InputHandler.KeyReleased(Keys.Enter) ||
                InputHandler.ButtonReleased(Buttons.A, playerIndex))
                base.OnSelected(null);
        }

        #endregion
    }
}
