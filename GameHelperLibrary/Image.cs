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

namespace GameHelperLibrary
{
    public class Image
    {

        private Texture2D _image;
        private Color tint = Color.White;

        #region Properties
        public Texture2D ImageTexture { get { return _image; } }
        public int Width { get { return ImageTexture.Width; } }
        public int Height { get { return ImageTexture.Height; } }

        public Color Tint { get { return tint; } }
        #endregion

        /// <summary>
        /// Creates an image that can be easily drawn to the screen
        /// </summary>
        /// <param name="content">Content Manager of game</param>
        /// <param name="assetName">The name of the asset with no extensions</param>
        public Image(ContentManager content, string assetName)
        {
            _image = content.Load<Texture2D>(assetName);
            this.tint = Color.White;
        }
        public Image(ContentManager content, string assetName, Color tint)
        {
            _image = content.Load<Texture2D>(assetName);
            this.tint = tint;
        }
        /// <summary>
        /// Creates an image that can easily be drawn to the screen.
        /// </summary>
        /// <param name="image"></param>
        public Image(Texture2D image)
        {
            _image = image;
        }

        public void Draw(SpriteBatch spriteBatch, float x, float y, bool flipped = false, float scale = 1.0f)
        {
            Draw(spriteBatch, new Vector2(x, y), flipped, scale);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 pos, bool flipped = false, float scale = 1.0f)
        {
            if (flipped)
                spriteBatch.Draw(_image, pos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
            else
                spriteBatch.Draw(_image, pos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

    }
}
