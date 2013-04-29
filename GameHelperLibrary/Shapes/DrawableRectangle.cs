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

namespace GameHelperLibrary.Shapes
{
    public class DrawableRectangle
    {

        private GraphicsDevice graphics;

        private Texture2D texture  = null;
        private Vector2  _position = Vector2.Zero;
        private Vector2  _size     = Vector2.Zero;
        private Color     color    = Color.White;

        public Vector2 Position { get { return _position; } set { _position = value; } }

        public int Width  { get { return (int)_size.X; } }
        public int Height { get { return (int)_size.Y; } }
        public Texture2D Texture
        {
            get { return texture; }
        }

        public DrawableRectangle(GraphicsDevice graphics, Vector2 size, Color color, bool filled)
        {
            _size = size;

            this.graphics = graphics;
            this.color = color;

            texture = SetTexture(filled);
        }

        private Texture2D SetTexture(bool filled)
        {
            var result = new Texture2D(graphics, Width, Height);
            Color[,] data = new Color[Width, Height];
            Color[] data1d = new Color[Width * Height];

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    data[x, y] = Color.Transparent;

            if (filled)
                for (int i = 0; i < data1d.Length; i++)
                    data1d[i] = Color.White;
            else
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                        {
                            data[x, y] = Color.White;
                            data1d[y * Width + x] = Color.White;
                        }
            result.SetData<Color>(data1d);
            return result;
        }

        public void Draw(SpriteBatch batch, Vector2 position, Color color)
        {
            this.color = color;

            Position = position;
            batch.Draw(texture, position, color);
        }

    }
}
