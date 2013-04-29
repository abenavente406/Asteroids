using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace GameHelperLibrary
{
    public enum MouseButton
    {
        LeftButton,
        RightButton,
        MiddleButton
    }

    public class InputHandler : Microsoft.Xna.Framework.GameComponent
    {

        #region Keyboard Fields and Properties
        static KeyboardState keyboardState;
        static KeyboardState lastKeyboardState;

        public static KeyboardState KeyboardState
        {
            get { return keyboardState; }
        }

        public static KeyboardState LastKeyboardState
        {
            get { return lastKeyboardState; }
        }
        #endregion

        #region Gamepad Fields and Properties
        static bool gamePadConnected = false;

        static GamePadState[] gamePadStates;
        static GamePadState[] lastGamePadStates;

        public static GamePadState[] GamePadStates
        {
            get { return gamePadStates; }
        }

        public static GamePadState[] LastGamePadStates
        {
            get { return lastGamePadStates; }
        }

        public static bool GamePadConnected
        {
            get { return gamePadConnected; }
        }
        #endregion

        #region Mouse Fields and Properties
        static MouseState mouseState;
        static MouseState lastMouseState;

        public static MouseState MouseState
        {
            get { return mouseState; }
        }

        public static MouseState LastMouseState
        {
            get { return lastMouseState; }
        }

        public static Vector2 MousePos
        {
            get { return new Vector2(mouseState.X, mouseState.Y); }
        }
        #endregion

        public InputHandler(Game game)
            : base(game)
        {
            keyboardState = Keyboard.GetState();
            gamePadStates = new GamePadState[Enum.GetValues(typeof(PlayerIndex)).Length];
            mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                gamePadConnected = true;

            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            lastGamePadStates = (GamePadState[])gamePadStates.Clone();
            foreach (PlayerIndex index in Enum.GetValues(typeof(PlayerIndex)))
                gamePadStates[(int)index] = GamePad.GetState(index);

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        public static void Flush()
        {
            lastKeyboardState = keyboardState;
        }

        #region Keyboard Methods
        public static bool KeyReleased(Keys key)
        {
            return keyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key);
        }

        public static bool KeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        #endregion

        #region Gamepad Methods
        public static bool ButtonReleased(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonUp(button) &&
                gamePadStates[(int)index].IsButtonDown(button);
        }

        public static bool ButtonPressed(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button) &&
                gamePadStates[(int)index].IsButtonUp(button);
        }

        public static bool ButtonDown(Buttons button, PlayerIndex index)
        {
            return gamePadStates[(int)index].IsButtonDown(button);
        }
        #endregion

        #region Mouse Methods
        public static bool MouseButtonPressed(MouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case MouseButton.LeftButton:
                    result = mouseState.LeftButton == ButtonState.Pressed &&
                             lastMouseState.LeftButton == ButtonState.Released;
                    break;
                case MouseButton.RightButton:
                    result = mouseState.RightButton == ButtonState.Pressed &&
                             lastMouseState.RightButton == ButtonState.Released;
                    break;
                case MouseButton.MiddleButton:
                    result = mouseState.MiddleButton == ButtonState.Pressed &&
                             lastMouseState.MiddleButton == ButtonState.Released;
                    break;
            }

            return result;
        }

        public static bool MouseButtonReleased(MouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case MouseButton.LeftButton:
                    result = mouseState.LeftButton == ButtonState.Released &&
                             lastMouseState.LeftButton == ButtonState.Pressed;
                    break;
                case MouseButton.RightButton:
                    result = mouseState.RightButton == ButtonState.Released &&
                             lastMouseState.RightButton == ButtonState.Pressed;
                    break;
                case MouseButton.MiddleButton:
                    result = mouseState.MiddleButton == ButtonState.Released &&
                             lastMouseState.MiddleButton == ButtonState.Pressed;
                    break;
            }

            return result;
        }

        public static bool MouseButtonDown(MouseButton button)
        {
            bool result = false;

            switch (button)
            {
                case MouseButton.LeftButton:
                    result = mouseState.LeftButton == ButtonState.Pressed;
                    break;
                case MouseButton.RightButton:
                    result = mouseState.RightButton == ButtonState.Pressed;
                    break;
                case MouseButton.MiddleButton:
                    result = mouseState.MiddleButton == ButtonState.Pressed;
                    break;
            }

            return result;
        }

        public static void SetMousePosition(Vector2 pos)
        {
            Mouse.SetPosition((int)pos.X, (int)pos.Y);
        }
        #endregion
    }
}
