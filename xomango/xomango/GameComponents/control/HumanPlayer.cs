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
            if (Side != board.CurrentPlayer)
            {
               // return;
            }

            if (args.gs.GestureType == GestureType.Tap)
            {
                processTurnGesture(args.gs.Position);
            }
        }

        public override string Name { get { return name; } }

        public override Side Side { get { return side; } }

        public void OnVisibleRectChanged(object sender, xomango.layers.ViewRectChangedEventArgs args)
        {
            visibleRect = args.rect;
        }

        private bool translateTapPosition(Vector2 tap, out Position pos)
        {
            float globalX = visibleRect.X + tap.X;
            float globalY = visibleRect.Y + tap.Y;

            if (!visibleRect.Contains(new Point((int)globalX, (int)globalY)))
            {
                pos = new Position();
                return false;
            }

            pos = new Position((short)(globalX / GameOptions.Instanse.CellSize), (short)(globalY / GameOptions.Instanse.CellSize));
            //problems with divion of negative numbers
            if (globalX < 0) pos.row--;
            if (globalY < 0) pos.column--;
            return true;
        }

        private void processTurnGesture(Vector2 tap)
        {
            Position pos;
            if (!translateTapPosition(tap, out pos))
            {
                return;
            }
            try
            {
                board[pos] = side;
                madeTurn(pos);
            }
            catch (Exception)
            { 
            }
        }               

        private readonly string name;
        private readonly Side side;

        private Board board;
        private Rectangle visibleRect = new Rectangle(0,0,0,0);

    }
}
