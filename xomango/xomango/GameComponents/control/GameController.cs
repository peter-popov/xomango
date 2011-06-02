using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using CoreCZ;

namespace xomango
{
    class GameControler
    {
        public GameControler()
        {
            gameBoard = new Board();
        }

        public void Restart()
        {
            gameBoard.Clear();
            currentPlayer = player1;
            player1.Reset();
            player2.Reset();
        }

        public void Update(GameTime gameTime)
        {
            if (currentPlayer != null && !gameBoard.Winner)
            {
                currentPlayer.Update(gameTime);
            }
        }

        public void HandleInput(object sender, control.TouchEventArgs args)
        {
            //TODO: think about it
            if (player1 is control.HumanPlayer)
            {
                (player1 as control.HumanPlayer).HandleInput(sender, args);
            }
            if (player2 is control.HumanPlayer)
            {
                (player2 as control.HumanPlayer).HandleInput(sender, args);
            }
        }

        public BasePlayer Player1
        {
            get { return player1; }
            set
            {
                if (player1 != null)
                {
                    player1.OnTurnMade -= this.playerMadeTurn;
                }
                player1 = value;
                player1.OnTurnMade += this.playerMadeTurn;
            }
        }

        public BasePlayer Player2
        {
            get { return player2; }
            set
            {
                if (player2 != null)
                {
                    player2.OnTurnMade -= this.playerMadeTurn;
                }
                player2 = value;
                player2.OnTurnMade += this.playerMadeTurn;
            }
        }

        private void playerMadeTurn(object sender, TurnEventArgs args)
        {
            if (sender == currentPlayer)
            {
                if (OnTurn != null)
                {
                    if (gameBoard.Winner)
                    {
                        args.Winner = (BasePlayer)(sender);
                    }
                    OnTurn(this, args);
                }
                nextPlayer();
            }
            else
            {
                //throw something really terrible here!
            }
        }

        private void nextPlayer()
        {
            if (currentPlayer == player1)
            {
                currentPlayer = player2;
            }
            else if (currentPlayer == player2)
            {
                currentPlayer = player1;
            }
            else
            {
                //WTF!!!
                Debug.Assert(false, "WTF in GameControler::nextPlayer()!");
            }

        }

        public Board GameBoard
        {
            get { return gameBoard; }
        }

        public event EventHandler<TurnEventArgs> OnTurn;
    

        private BasePlayer player1;
        private BasePlayer player2;
        private BasePlayer currentPlayer;
        private Board gameBoard;



    }
}
