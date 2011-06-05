using System;
using System.IO;
using System.IO.IsolatedStorage;
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

        private GameControler(Board board)
        {
            gameBoard = board;
        }

        public void Restart()
        {
            gameBoard.Clear();
            currentPlayer = player1;
            player1.Reset();
            player2.Reset();
        }


        public void SetUpGame(PlayerType pt1, PlayerType pt2)
        {
            if (pt1 == PlayerType.Human)
            {
                Player1 = new control.HumanPlayer(gameBoard, "You", Side.Cross);
            }
            else if (pt1 == PlayerType.Machine)
            {
                Player1 = new control.MachinePlayer(gameBoard, Side.Cross);
            }

            if (pt2 == PlayerType.Human)
            {
                Player2 = new control.HumanPlayer(gameBoard, "You", Side.Zero);
                if (pt1 == PlayerType.Machine)
                {
                    Player2.OnTurnMade += (Player1 as control.MachinePlayer).OnEnemyMadeTurn;
                }
            }
            else if (pt2 == PlayerType.Machine)
            {
                Player2 = new control.MachinePlayer(gameBoard, Side.Zero);
                if (pt1 == PlayerType.Human)
                {
                    Player1.OnTurnMade += (Player2 as control.MachinePlayer).OnEnemyMadeTurn;
                }
            }
            currentPlayer = player1;
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
            if (gameBoard.Winner)
            {
                return;
            }
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

        public BasePlayer CurrentPlayer
        {
            get { return currentPlayer; }
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
                if (!gameBoard.Winner)
                {
                    nextPlayer();
                }
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

        #region Serialization
        public static GameControler Load()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;            
            if ( !settings.Contains("Saved") || settings["Saved"] == false.ToString())
            {
                return null;
            }

            Board board = new Board();
            GameControler ctrl;
            try
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isoStream = store.OpenFile(@"board.dat", FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(isoStream);
                        board.Deserialize(br);
                        br.Close();
                    }
                }
                ctrl = new GameControler(board);

                ctrl.SetUpGame(parseType(settings["Game.Player1"].ToString()), parseType(settings["Game.Player2"].ToString()));

                if (settings["Game.CurrentTurnFrom"].ToString() == 1.ToString())
                {
                    ctrl.currentPlayer = ctrl.player1;
                }
                else
                {
                    ctrl.currentPlayer = ctrl.player2;                
                }

                return ctrl;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Save()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove("Saved");

            if (gameBoard.Turns.Count() == 0 || gameBoard.Winner)
            {
                return;
            }
            
            settings["Saved"] = true;            

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isoStream = store.OpenFile(@"board.dat", FileMode.OpenOrCreate))
                {
                    BinaryWriter wr = new BinaryWriter(isoStream);
                    gameBoard.Serialize(wr);
                    wr.Close();
                }
            }

            settings["Game.Player1"] = Player1.Type;
            settings["Game.Player2"] = Player2.Type;
            settings["Game.CurrentTurnFrom"] = (player1 == currentPlayer) ? 1 : 2;
        }

        private static PlayerType parseType(string s)
        {
            if (s == PlayerType.Human.ToString())
            {
                return PlayerType.Human;
            }
            else if (s == PlayerType.Machine.ToString())
            {
                return PlayerType.Machine;
            }
            Debug.Assert(false);
            return PlayerType.Human;            
        }

        #endregion

        public event EventHandler<TurnEventArgs> OnTurn;
    

        private BasePlayer player1;
        private BasePlayer player2;
        private BasePlayer currentPlayer;
        private Board gameBoard;
    }
}
