using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using CoreCZ;

namespace xomango
{
    public class TurnEventArgs : EventArgs
    {
        public TurnEventArgs(Turn turn)
        {
            this.Turn = turn;
        }
        public readonly Turn Turn;
        public BasePlayer Winner { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePlayer
    {
        public abstract void Update(GameTime gameTime);

        public abstract string Name { get; }

        public abstract Side Side { get; }

        public abstract void Reset( );

        public event EventHandler<TurnEventArgs> OnTurnMade;

        protected void madeTurn(Position pos)
        {
            if (OnTurnMade != null)
            {
                OnTurnMade(this, new TurnEventArgs(new Turn(pos, Side, 0)));
            }
        }
    }
}