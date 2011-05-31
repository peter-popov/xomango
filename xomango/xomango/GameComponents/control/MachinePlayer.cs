using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreCZ;

using Microsoft.Xna.Framework;

namespace xomango.control
{
    class MachinePlayer : BasePlayer
    {
        public MachinePlayer(Board board, Side side)
        {
            playerAI = new SimplePlayer(side);
            this.board = board;
            this.side = side;
        }

        bool timeToMakeTheTurn = false;
        
        public void OnEnemyMadeTurn(Object sender, TurnEventArgs args)
        {
            playerAI.EnemyTurn(args.Turn.position);
            timeToMakeTheTurn = true;
        }

        public override void Reset()
        {
            playerAI = new SimplePlayer(side);
        }

        public override void Update(GameTime gameTime)
        {
            if (timeToMakeTheTurn)
            {
                Position pos = playerAI.MakeNextTurn();
                board[pos] = this.Side;
                madeTurn(pos);
            }
            timeToMakeTheTurn = false;
        }

        public override string Name { get { return "Barnie"; } }

        public override Side Side { get { return playerAI.Side; } }

        private CoreCZ.SimplePlayer playerAI;
        private Board board;
        private Side side;
    }
}
