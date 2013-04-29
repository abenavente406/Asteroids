using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameHelperLibrary.Controls
{
    public class PictureBox : Control
    {
        #region Field Region

        Texture2D image;
        Rectangle sourceRect;
        Rectangle destRect;

        #endregion

        #region Property Region

        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        public Rectangle SourceRectangle
        {
            get { return sourceRect; }
            set { sourceRect = value; }
        }

        public Rectangle DestinationRectangle
        {
            get { return destRect; }
            set { destRect = value; }
        }

        #endregion

        #region Constructors

        public PictureBox(Texture2D image, Rectangle destination)
        {
            Image = image;
            DestinationRectangle = destination;
            SourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
            Color = Color.White;
        }

        public PictureBox(Texture2D image, Rectangle destination, Rectangle source)
        {
            Image = image;
            DestinationRectangle = destination;
            SourceRectangle = source;
            Color = Color.White;
        }

        #endregion

        #region Abstract Method Region

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            switch (Effect)
            {
                case ControlEffect.NONE:
                    {
                        spriteBatch.Draw(image, new Rectangle(destRect.X, destRect.Y, (int)(destRect.Width * scale),
                            (int)(destRect.Height * scale)), sourceRect, color);
                        break;
                    }
                case ControlEffect.FLASH:
                    {
                        if (gameTime.TotalGameTime.Milliseconds % flashDuration > flashDuration / 2)
                            spriteBatch.Draw(image, new Rectangle(destRect.X, destRect.Y, (int)(destRect.Width * scale),
                                (int)(destRect.Height * scale)), sourceRect, color);
                        break;
                    }
                case ControlEffect.GLOW:
                    {
                        spriteBatch.Draw(image, new Rectangle(destRect.X, destRect.Y, (int)(destRect.Width * scale),
                               (int)(destRect.Height * scale)), sourceRect, color);

                        spriteBatch.Draw(image, new Rectangle(destRect.X, destRect.Y, (int)(destRect.Width * scale),
                        (int)(destRect.Height * scale)), sourceRect, overlay);

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

        #region Picture Box Methods

        public void SetPosition(Vector2 newPosition)
        {
            destRect = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                sourceRect.Width,
                sourceRect.Height);
        }

        #endregion
    }
}
