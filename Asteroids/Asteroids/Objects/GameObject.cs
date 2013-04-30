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
                    value.X = AsteroidGame.ScreenBounds.Width;
                if (value.Y < 0)
                    value.Y = AsteroidGame.ScreenBounds.Height;

                if (value.X > AsteroidGame.ScreenBounds.Width)
                    value.X = 0;
                if (value.Y > AsteroidGame.ScreenBounds.Height)
                    value.Y = 0;

                _pos = Vector2.Clamp(new Vector2(value.X, value.Y), new Vector2(0, 0), 
                       new Vector2(AsteroidGame.ScreenBounds.Width, AsteroidGame.ScreenBounds.Height));
            }
        }
        public virtual Vector2 Speed { get { return _speed; } set { _speed = value; } }
        public Vector2 Acceleration  { get { return _acceleration; } set { _acceleration = value; } }
        public Vector2 Resistance    { get { return _resistance; } }

        // The center of the texture, NOT RELATIVE to location
        public Vector2 TextureOrigin
        {
            get { return new Vector2(texture.Width / 2, texture.Height / 2); }
        }
        // The center of the texture, RELATIVE to location
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Position.X + texture.Width / 2, Position.Y +
                texture.Height / 2);
            }
        }

        public float MovementAngle
        {
            get { return (float)_movementAngle; }
            protected set
            {
                _movementAngle =
                    MathHelper.WrapAngle((float)value - MathHelper.PiOver2);
            }
        }
        public float DrawAngle
        {
            get { return (float)_drawAngle; }
            protected set
            {
                _drawAngle =
                    MathHelper.WrapAngle((float)value);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                    (int)(texture.Width * _scale), (int)(texture.Height * _scale));
            }
        }
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

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
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
        /// Gets if two rectangles are colliding
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Colliding(GameObject other)
        {
            return this.Bounds.Intersects(other.Bounds);
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
