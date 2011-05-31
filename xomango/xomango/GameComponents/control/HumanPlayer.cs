using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using CoreCZ;

namespace xomango.control
{
    class HumanPlayer:BasePlayer
    {
        public HumanPlayer(Board board, string name, Side side)
        {
            this.board = board;
            this.name = name;
            this.side = side;

            // Enable tap gestures
            TouchPanel.EnabledGestures = TouchPanel.EnabledGestures | GestureType.Tap;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Reset()
        {
            
        }

        public void HandleInput(object sender, control.TouchEventArgs args)
        {
            try
            {
                board[args.Position] = side;
                madeTurn(args.Position);
            }
            catch (Exception)
            {
            }
        }

        public override string Name { get { return name; } }

        public override Side Side { get { return side; } }

        private readonly string name;
        private readonly Side side;

        private Board board;        
    }
}
