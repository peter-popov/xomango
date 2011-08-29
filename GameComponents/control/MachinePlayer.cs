using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CoreCZ;

using Microsoft.Xna.Framework;

namespace GameComponents.Control
{
    class MachinePlayer : BasePlayer
    {
        public MachinePlayer(Board board, Side side)
        {
            playerAI = new SimplePlayer(side, board);
            this.board = board;
            this.side = side;
        }

        
        public override PlayerType Type { get { return PlayerType.Machine; } }

        
        public void OnEnemyMadeTurn(Object sender, TurnEventArgs args)
        {
            playerAI.EnemyTurn(args.Turn.position);
            timeToMakeTheTurn = true;
        }

        public override void Reset()
        {
            playerAI = new SimplePlayer(side);
        }

        #region Worker
        private void thinker()
        {
            readyToAnswer = false;
            Position pos = playerAI.MakeNextTurn();
            Thread.Sleep(300);
            lock (this)
            {                
                readyToAnswer = true;
                answer = pos;
            }
        }
      
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (timeToMakeTheTurn)
            {
                Thread th = new Thread(new ThreadStart(thinker));
                th.Start();   
                timeToMakeTheTurn = false;
                return;                             
            }
            if (readyToAnswer)
            {
                board[answer] = this.Side;
                madeTurn(answer);
                readyToAnswer = false;
            }
        }

        public override void Undo()
        {
            playerAI.Undo();
        }

        public override string Name { get { return "Barnie"; } }

        public override Side Side { get { return playerAI.Side; } }

        private CoreCZ.SimplePlayer playerAI;
        private Board board;
        private Side side;
        private bool timeToMakeTheTurn = false;
        private bool readyToAnswer = false;
        private Position answer = new Position(0, 0);
    }
}
