using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameHelperLibrary
{
    public class GameStateManager : GameComponent
    {
        #region EventHandler
        // The event called when the current state changes
        public event EventHandler OnStateChanged;
        #endregion

        #region Fields
        // Stack of gameStates that 
        Stack<GameState> gameStates = new Stack<GameState>();

        // Keep track of the current draw order of the stack
        const int startDrawOrder = 5000;
        const int drawOrderInc = 100;
        int drawOrder;

        GameState targetState;
        #endregion

        #region Properties
        // Gets the state at the top of the stack
        public GameState CurrentState
        {
            get { return gameStates.Peek(); }
        }

        public GameState TargetState
        {
            get { return targetState; }
            set { targetState = value; }
        }
        #endregion

        #region Constructor and Initialization
        public GameStateManager(Game game)
            : base(game)
        {
            drawOrder = startDrawOrder;
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        #endregion

        #region Public Helper Methods
        /// <summary>
        /// Removes the state at the top of the stack and sets off the state changed event
        /// </summary>
        public void PopState()
        {
            if (gameStates.Count > 0)
            {
                RemoveState();
                drawOrder -= drawOrderInc;

                if (OnStateChanged != null)
                    OnStateChanged(this, null);
            }
        }

        /// <summary>
        /// Removes the state without setting off the event -- used in conjunction with PopState()
        /// </summary>
        public void RemoveState()
        {
            GameState State = gameStates.Peek();
            OnStateChanged -= State.StateChange;
            Game.Components.Remove(State);
            gameStates.Pop();
        }

        /// <summary>
        /// Pushes a new state to the top of the stack and sets the newstate as active
        /// </summary>
        /// <param name="newState"></param>
        public void PushState(GameState newState)
        {
            drawOrder += drawOrderInc;
            newState.DrawOrder = drawOrder;

            AddState(newState);

            if (OnStateChanged != null)
                OnStateChanged(this, null);
        }

        /// <summary>
        /// Adds a new state without changing the active state
        /// </summary>
        /// <param name="newState"></param>
        private void AddState(GameState newState)
        {
            gameStates.Push(newState);

            Game.Components.Add(newState);

            OnStateChanged += newState.StateChange;
        }

        /// <summary>
        ///  Changes the state and activates the state
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(GameState newState)
        {
            while (gameStates.Count > 0)
                RemoveState();

            newState.DrawOrder = startDrawOrder;
            drawOrder = startDrawOrder;

            AddState(newState);

            if (OnStateChanged != null)
                OnStateChanged(this, null);
        }

        #endregion
    }
}
