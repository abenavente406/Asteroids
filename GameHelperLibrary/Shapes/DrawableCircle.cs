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
    public class DrawableCircle
    {
        private GraphicsDevice graphics;

        private Texture2D texture  = null;
        private Vector2  _position = Vector2.Zero;
        private int      _radius   = 0;
        private Color     color    = Color.White;

        public Vector2 Position { get { return _position; } set { _position = value; } }
        public int     Radius   { get { return _radius; } }

        public DrawableCircle(GraphicsDevice graphics, int radius, Color color, bool filled)
        {
            this.graphics = graphics;
            this.color = color;

            _radius = radius;

            texture = CreateCircle(_radius, filled);
        }

        public Texture2D CreateCircle(int radius, bool filled)
        {
            int outerRadius = radius * 2 + 2; // So circle doesn't go out of bounds
            Texture2D texture = new Texture2D(graphics, outerRadius, outerRadius);

            Color[] data = new Color[outerRadius * outerRadius];
            Color[,] data2d = new Color[outerRadius, outerRadius];

            Vector2 center = new Vector2(Position.X + radius, Position.Y + radius);

            // Colour the entire texture transparent first.
            for (int i = 0; i < data.Length; i++)
                data[i] = Color.Transparent;

            // Work out the minimum step necessary using trigonometry + sine approximation.
            double angleStep = 1f / radius;

            if (!filled)
                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                    int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                    data[y * outerRadius + x + 1] = Color.White;
                }
            else
            {
                for (double angle = 0; angle < Math.PI * 2; angle += angleStep)
                {
                    int x = (int)Math.Round(radius + radius * Math.Cos(angle));
                    int y = (int)Math.Round(radius + radius * Math.Sin(angle));

                    if (x > radius)
                        for (int xx = (int)Math.Round(radius + radius * Math.Cos(angle)); xx > radius; xx--)
                            data2d[xx, y] = Color.White;
                    else
                        for (int xx = (int)Math.Round(radius + radius * Math.Cos(angle)); xx <= radius; xx++)
                            data2d[xx, y] = Color.White;


                }

                List<Color> colors = new List<Color>(radius * radius);

                foreach (Color c in data2d)
                    colors.Add(c);

                for (int i = 0; i < colors.Count; i++)
                    data[i] = colors[i];
            }

            
            texture.SetData(data);
            return texture;
        }

        public void Draw(SpriteBatch batch, float x, float y)
        {
            Draw(batch, new Vector2(x, y));
        }

        public void Draw(SpriteBatch batch, Vector2 location)
        {
            Position = location;
            batch.Draw(texture, location, color);
        }
    }
}
