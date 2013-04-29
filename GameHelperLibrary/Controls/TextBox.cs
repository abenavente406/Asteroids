using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelperLibrary.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameHelperLibrary.Controls
{
    public class TextBox : Control
    {
        #region Fields
        private Keys[] lastPressedKeys;

        private SpriteFont font;
        private Texture2D cursor;

        private Rectangle cursorPos;

        private Color realForeColor;
        private Color realBackColor;
        private Color foreColor;
        private Color backColor;
        private Color flashColor;

        private int maxCharLength = 7;

        private DrawableRectangle cur;
        #endregion

        #region Properties
        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; realForeColor = value; }
        }
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; realBackColor = value; }
        }
        public Color FlashColor
        {
            get { return flashColor; }
            set { flashColor = value; }
        }
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = Vector2.Clamp(value, new Vector2(1, 1), new Vector2(360000, 360000));
            }
        }
        public int MaxSize
        {
            get { return maxCharLength; }
            set { maxCharLength = value; }
        }
        #endregion

        public TextBox(GraphicsDevice graphics)
        {
            Size = Vector2.One;
            Font = SpriteFont;
            flashColor = Color.Transparent;

            cur = new DrawableRectangle(graphics, new Vector2(64, 64), Color.White, true);
            cursor = cur.Texture;
        }

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            Keys[] pressedKeys = InputHandler.KeyboardState.GetPressedKeys();
           
            foreach (Keys key in pressedKeys)
            {
                if (!pressedKeys.Contains(key))
                    OnKeyUp(key);
            }

            foreach (Keys key in pressedKeys)
            {
                if (!(lastPressedKeys == null))
                    if (!lastPressedKeys.Contains(key))
                        OnKeyDown(key);
            }

            lastPressedKeys = pressedKeys;

            Size = SpriteFont.MeasureString(this.text);
            
            if (text.Length == 0)
                cursorPos = new Rectangle((int)(Position.X + size.X + font.MeasureString("O").X / 2),
                    (int)(Position.Y + Size.Y / 2), (int)spriteFont.MeasureString(" ").X, 
                    (int)spriteFont.MeasureString(" ").Y);
            else
                cursorPos = new Rectangle((int)(Position.X + size.X + font.MeasureString("O").X / 2),
                    (int)(Position.Y + Size.Y / 2 - spriteFont.MeasureString(" ").Y / 2), 
                    (int)spriteFont.MeasureString(" ").X, (int)spriteFont.MeasureString(" ").Y);

            if (gameTime.TotalGameTime.Milliseconds % 400 > 200)
            {
                backColor = flashColor; 
                foreColor = flashColor;
            }
            else
            {
                backColor = realBackColor; 
                foreColor = realForeColor;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.DrawString(SpriteFont, text, position, realForeColor);
            spriteBatch.Draw(cursor, cursorPos, foreColor);
        }
        #endregion

        #region Events
        private void OnKeyUp(Keys key)
        {

        }
        private void OnKeyDown(Keys key)
        {
            if (text.Length < maxCharLength)
            {
                if (IsNumKey(key))
                    text += key.ToString()[key.ToString().Length - 1];
                else if (IsLetterKey(key))
                    text += key.ToString();
                else if (key == Keys.Space)
                    text += " ";
            }
            
            if (key == Keys.Back)
                if (text.Length > 0) text = text.Remove(text.Length - 1, 1);
        }
        public override void HandleInput(PlayerIndex playerIndex)
        {
        }
        #endregion

        #region Helper Functions
        private bool IsNumKey(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9) ||
                (key >= Keys.NumPad0 && key <= Keys.NumPad9);
        }

        private bool IsLetterKey(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }
        #endregion
    }
}
