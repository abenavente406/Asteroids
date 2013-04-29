using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GameHelperLibrary {
    public class SpriteSheet
    {
        #region Fields
        private int imageWidth;
        private int imageHeight;
        private int _spriteWidth;
        private int _spriteHeight;

        private Texture2D sourceImage;
        private GraphicsDevice graphics;
        #endregion

        #region Properties
        public int SpriteWidth  { get { return _spriteWidth;  } set { _spriteWidth = value;  } }
        public int SpriteHeight { get { return _spriteHeight; } set { _spriteHeight = value; } }

        public int Width { get { return imageWidth; } }
        public int Height { get { return imageHeight; } }

        public string Name { get; set; }
        #endregion

        /// <summary>
        /// Create a sprite sheet from a Texture2D
        /// </summary>
        /// <param name="sourceImage">The sprite sheet image</param>
        /// <param name="spriteWidth">The width of each sprite</param>
        /// <param name="spriteHeight">The height of each sprite</param>
        /// <param name="graphics">The graphics device used by the game</param>
        public SpriteSheet(Texture2D sourceImage, int spriteWidth, int spriteHeight, GraphicsDevice graphics) {
            this.sourceImage = sourceImage;
            imageWidth = sourceImage.Width;
            imageHeight = sourceImage.Height;
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            this.graphics = graphics;

            this.Name = sourceImage.Name;
        }

        /// <summary>
        /// Gets a sub image from the sprite sheet
        /// </summary>
        /// <param name="x">The x tile of the image</param>
        /// <param name="y">The y tile of the image</param>
        /// <returns>A texture2D that is the sub image</returns>
        public Texture2D GetSubImage(int x, int y) {
            Rectangle sourceRect = new Rectangle(x * SpriteWidth, y * SpriteHeight, SpriteWidth, SpriteHeight);

            Color[] data = new Color[sourceRect.Width * sourceRect.Height];
            sourceImage.GetData<Color>(0, sourceRect, data, 0, data.Length);

            var newTexture = new Texture2D(graphics, sourceRect.Width, sourceRect.Height);
            newTexture.SetData<Color>(data);

            return newTexture;
        }
    }
}
