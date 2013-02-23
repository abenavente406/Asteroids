using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameHelperLibrary.Shapes;

namespace Asteroids.Objects
{
    public abstract class GameObject
    {

        #region Fields
        protected int   _width;
        protected int   _height;
        protected float _mass = 1.0f;

        protected Texture2D texture   = null;

        protected float rotationSpeed = .1f;

        protected Vector2 _pos = Vector2.Zero;
        protected Vector2 _speed        = Vector2.Zero;
        protected Vector2 _acceleration = Vector2.Zero;
        protected Vector2 _resistance = new Vector2(-0.2f, -0.2f);

        protected float topSpeed      = 0.0f;
        protected double _movementAngle = 0;
        protected double _drawAngle   = 0;
        
        protected float  _scale       = 1.0f;
        #endregion

        #region "Properties"

        public int Width   { get { return _width;  } }
        public int Height  { get { return _height; } }
        public float Scale { get { return _scale; } }
        public float Mass  { get { return _mass; } }

        public Texture2D Texture { get { return texture; } }

        public Vector2 Position
        {
            get { return _pos; }
            set
            {
                // Wrap the position of the objects around the screen
                if (value.X < 0)
                    value.X = AsteroidGame.WindowWidth;
                if (value.Y < 0)
                    value.Y = AsteroidGame.WindowHeight;

                if (value.X > AsteroidGame.WindowWidth)
                    value.X = 0;
                if (value.Y > AsteroidGame.WindowHeight)
                    value.Y = 0;

                _pos = Vector2.Clamp(new Vector2(value.X, value.Y), new Vector2(0, 0), 
                       new Vector2(AsteroidGame.WindowWidth, AsteroidGame.WindowHeight));
            }
        }
        public virtual Vector2 Speed { get { return _speed; } set { _speed = value; } }
        public Vector2 Acceleration  { get { return _acceleration; } set { _acceleration = value; } }
        public Vector2 Resistance    { get { return _resistance; } }

        // The center of the texture, NOT RELATIVE to location
        public Vector2 TextureOrigin { get { return new Vector2(texture.Width / 2, texture.Height / 2); } }
        // The center of the texture, RELATIVE to location
        public Vector2 Origin        { get { return new Vector2(Position.X + texture.Width / 2, Position.Y + 
                                             texture.Height / 2); } }

        public float MovementAngle   { get { return (float)_movementAngle; } protected set { _movementAngle = 
                                             MathHelper.WrapAngle((float)value - MathHelper.PiOver2); } }
        public float DrawAngle       { get { return (float)_drawAngle; } protected set { _drawAngle = 
                                             MathHelper.WrapAngle((float)value); } }

        public Rectangle Bounds      { get { return new Rectangle((int)Position.X, (int)Position.Y, 
                                            (int)(texture.Width * _scale), (int)(texture.Height * _scale)); } }

        #endregion

        /// <summary>
        /// Constructor!!!
        /// </summary>
        /// <param name="pos">The position of the object on the screen</param>
        public GameObject(Vector2 pos)
        {
            _pos = pos;
            //AsteroidGame.objects.Add(this);
        }

        public abstract void Update(GameTime gameTime);

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, DrawAngle, TextureOrigin, _scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Sets the texture and dimensions of the object
        /// </summary>
        /// <param name="content"></param>
        /// <param name="assetName">Name of the asset without the .extension</param>
        protected void SetTexture(ContentManager content, string assetName)
        {
            SetTexture(content.Load<Texture2D>(assetName));
        }
        /// <summary>
        /// Sets the texture and dimensions of the object
        /// </summary>
        /// <param name="texture"></param>
        protected void SetTexture(Texture2D texture)
        {
            this.texture = texture;
            _width = (int)(texture.Bounds.Width * Scale);
            _height = (int)(texture.Bounds.Height * Scale);
        }

        /// <summary>Applies a force to an object using Newton's second law of motion
        /// F = M * A   or   A = F / M   or   M = F / A
        /// F = Force   M = Mass   A = Acceleration</summary>
        /// <param name="force">The amount of force to add</param>
        protected void ApplyForce(Vector2 force)
        {
            force /= _mass;
            _acceleration += force;
        }

        /// <summary>
        /// Gets if two circular objects are colliding
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>If a collision is detected</returns>
        public bool CollidingCircles(GameObject A, GameObject B)
        {
            return Vector2.Distance(A.Origin, B.Origin) < A.Bounds.Width / 2 + B.Bounds.Width / 2;
        }

        /// <summary>
        /// Gets if two irregular shaped objects are colliding (Per-Pixel)
        /// </summary>
        /// <param name="this"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CollidingWith(GameObject @this, GameObject other)
        {
            // Get dimensions of texture
            int widthOther = other.texture.Width * (int)Scale;
            int heightOther = other.texture.Height * (int)Scale;
            int widthMe = @this.texture.Width * (int)Scale;
            int heightMe = @this.texture.Height * (int)Scale;

            if (((Math.Min(widthOther, heightOther) > 100) ||  // at least avoid doing it
                (Math.Min(widthMe, heightMe) > 100)))          // for small sizes (performance)
            {
                return Bounds.Intersects(other.Bounds) // If simple intersection fails, don't even bother with per-pixel
                    && PerPixelCollision(@this, other);
            }

            return Bounds.Intersects(other.Bounds);
        }

        /// <summary>
        /// Gets if two rectangles are colliding
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Colliding(GameObject other)
        {
            return this.Bounds.Intersects(other.Bounds);
        }

        /// <summary>
        /// Checks per pixel if two objects are colliding
        /// </summary>
        /// <param name="this"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        static bool PerPixelCollision(GameObject @this, GameObject other)
        {
            // Get Color data of each Texture
            Color[] bitsA = new Color[(int)(@this.texture.Width * @this.texture.Height * @this._scale)];
            @this.texture.GetData(bitsA);
            Color[] bitsB = new Color[(int)(other.texture.Width * other.texture.Height * other.Scale)];
            other.texture.GetData(bitsB);

            // Calculate the intersecting rectangle
            int x1 = Math.Max(@this.Bounds.X, other.Bounds.X);
            int x2 = Math.Min(@this.Bounds.X + @this.Bounds.Width, other.Bounds.X + other.Bounds.Width);

            int y1 = Math.Max(@this.Bounds.Y, other.Bounds.Y);
            int y2 = Math.Min(@this.Bounds.Y + @this.Bounds.Height, other.Bounds.Y + other.Bounds.Height);

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    Color a = bitsA[(x - @this.Bounds.X) + (y - @this.Bounds.Y) * (int)(@this.texture.Width * @this._scale)];
                    Color b = bitsB[(x - other.Bounds.X) + (y - other.Bounds.Y) * (int)(other.texture.Width * other._scale)];

                    if (a.A != 0 && b.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            // If no collision occurred by now, we're clear.
            return false;
        }

        /// <summary>
        /// Bounces the asteroids based on their mass **Very buggy, needs work**
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        protected void BounceObjects(GameObject object1, GameObject object2)
        {
            // Found from the center of mass formula
            // (M1*V1 + M2*V2) / (M1 + M2)
            // M = Mass   V = Velocity
            Vector2 cOfMass = (object1._mass * object1.Speed + object2._mass * object2.Speed) /
                (object1._mass + object2._mass);

            Vector2 normal1 = object2.Origin - object1.Origin;
            normal1.Normalize();
            Vector2 normal2 = object1.Origin - object2.Origin;
            normal2.Normalize();

            object1.Speed -= cOfMass;
            object1.Speed = Vector2.Reflect(object1.Speed, normal1);
            object1.Speed += cOfMass;

            object2.Speed -= cOfMass;
            object2.Speed = Vector2.Reflect(object2.Speed, normal2);
            object2.Speed += cOfMass;
        }
    }
}
