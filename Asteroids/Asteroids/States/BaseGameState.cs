using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameHelperLibrary;
using GameHelperLibrary.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids.States
{
    /// <summary>
    /// The blue print for game states
    /// </summary>
    public abstract partial class BaseGameState : GameState
    {
        protected AsteroidGame GameRef;                 // The main game that is referenced in each game state
        protected ControlManager ControlManager;        // Manages all controls on individual states; controls: btn, txtbox, etc.
        protected PlayerIndex playerIndexInControl;     // The player index in control... usually player 1

        public BaseGameState(Game game, GameStateManager manager)
            : base(game, manager)
        {
            GameRef = (AsteroidGame)game;
            playerIndexInControl = PlayerIndex.One;
        }

        protected override void LoadContent()
        {
            ContentManager Content = GameRef.Content;

            SpriteFont menuFont = Content.Load<SpriteFont>("menuFont");  // This loads the default font for the game
            ControlManager = new ControlManager(menuFont);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawState(SpriteBatch batch, GameTime gameTime)
        {
            base.DrawState(batch, gameTime);
        }

        // IMPORTANT! READ THIS BEFORE TRYING TO FIGURE OUT WHAT SwitchState DOES
        // IMPORTANT! READ THIS BEFORE TRYING TO FIGURE OUT WHAT SwitchState DOES
        // IMPORTANT! READ THIS BEFORE TRYING TO FIGURE OUT WHAT SwitchState DOES
        // IMPORTANT! READ THIS BEFORE TRYING TO FIGURE OUT WHAT SwitchState DOES
        // IMPORTANT! READ THIS BEFORE TRYING TO FIGURE OUT WHAT SwitchState DOES
        /// <summary>
        /// Kind of a hack function. Allows the fading from one state to another but you need
        /// to call FadeOutRectangle.Draw(GameRef.spriteBatch, Vector2.Zero, FadeOutColor);
        /// </summary>
        /// <param name="targetState">The state to switch to</param>
        public void SwitchState(GameState targetState)
        {
            StateManager.TargetState = targetState;

            IsExiting = true;
        }
    }
}
