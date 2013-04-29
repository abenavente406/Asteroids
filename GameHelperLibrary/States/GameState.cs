#region Class Descripton
/*
 * Author: Jamie McMahon 
 * URL: xnagpa.net
 * 
 * Modified By: Anthony Benavente
 * 
 * */

///<summary>
///This class declares a game state which can be used in 
///organizing different parts of a game.  I.E. game states 
///can be used to add main menu screens, gameplay screens,
///pause screens, etc. .
///</summary>
#endregion

#region 'using' Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameHelperLibrary.Shapes;
#endregion

namespace GameHelperLibrary
{
    public enum StateStatus
    {
        Hidden,
        TransitionOff,
        TransitionOn,
        Active
    }

    public abstract partial class GameState : DrawableGameComponent
    {
        #region Fields and Properties
        /// <summary>
        /// The game components within the game state
        /// </summary>
        public List<GameComponent> Components
        {
            get { return childComponents; }
        }
        // The components of the game state -- Controls
        List<GameComponent> childComponents;

        /// <summary>
        /// The tag identifier of the game state (instead of using 'this')
        /// </summary>
        public GameState Tag
        {
            get { return tag; }
        }
        GameState tag;

        // The game state manager... self-explanatory
        protected GameStateManager StateManager;

        /// <summary>
        /// This is the current state of the game state; it could be either active,
        /// transitioning, or hidden
        /// </summary>
        public StateStatus StateStatus
        {
            get { return stateStatus; }
            set { stateStatus = value; }
        }
        StateStatus stateStatus = StateStatus.TransitionOn;

        /// <summary>
        /// When this is true, the game state will be disposed of
        /// for good.  Otherwise, it's state would be "Hidden"
        /// </summary>
        public bool IsExiting
        {
            get { return isExiting; }
            set { isExiting = value; }
        }
        bool isExiting = false;

        public bool IsEntering
        {
            get { return isEntering; }
            set { isEntering = value; }
        }
        bool isEntering = true;

        public DrawableRectangle FadeOutRect;

        public Color FadeOutColor
        {
            get { return new Color(0, 0, 0, Alpha); }
        }

        public float Alpha
        {
            get { return alpha; }
            set
            {
                alpha = MathHelper.Clamp(value, 0, 1);
            }
        }
        float alpha = 1.0f;

        /// <summary>
        /// Gets if the game state can accept user input (because it is active)
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
        }
        bool isActive = false;

        #endregion  

        #region Constructor and Initialization
        /// <summary>
        /// Creates a new game state
        /// </summary>
        /// <param name="game">Main XNA game</param>
        /// <param name="manager">The manager in charge of this state</param>
        public GameState(Game game, GameStateManager manager)
            : base(game)
        {
            StateManager = manager;
            childComponents = new List<GameComponent>();
            tag = this;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            FadeOutRect = new DrawableRectangle(GraphicsDevice, new Vector2(GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height), Color.Black, true);
        }
        #endregion

        #region Update and Draw
        public virtual void DrawState(SpriteBatch batch, GameTime gameTime)
        {
            DrawableGameComponent drawComponent;
            
            // Iterate through each component in the game state and if it is drawable, draw it
            foreach (GameComponent component in childComponents)
            {
                if (component is DrawableGameComponent)
                {
                    drawComponent = component as DrawableGameComponent;
                    if (drawComponent.Visible)
                        drawComponent.Draw(gameTime);
                }
            }

            FadeOutRect.Draw(batch, Vector2.Zero, FadeOutColor);
        }

        public override void Update(GameTime gameTime)
        {

            if (isExiting)
                Alpha += .05f;
            if (isEntering)
                Alpha -= .02f;

            if (1 - Alpha <= .05f && isExiting)
            {
                isExiting = false;
                Alpha = 0.0f;
                StateManager.TargetState.IsEntering = true;
                StateManager.TargetState.alpha = 1.0f;
                StateManager.PushState(StateManager.TargetState);
                StateManager.TargetState = null;
                return;
            }

            if (Alpha <= 0 && IsEntering)
            {
                isEntering = false;
                Alpha = 0.0f;
            }

                foreach (GameComponent component in childComponents)
                    if (component.Enabled)
                        component.Update(gameTime);

                base.Update(gameTime);
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that gets called when the state manager changes states
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal protected virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
                Show();
            else
                Hide();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Shows the game state to the screen disregarding the manager i.e. make everything visible
        /// </summary>
        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;
            foreach (GameComponent component in childComponents)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent)component).Visible = true;
            }
        }

        /// <summary>
        /// Hides the game state disregarding the manager
        /// </summary>
        protected void Hide()
        {
            Visible = false;
            Enabled = false;
            foreach (GameComponent component in childComponents)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent)component).Visible = false;
            }
        }
        #endregion
    }
}
