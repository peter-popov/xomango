using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using CoreCZ;
using xomango.utils;
using xomango.control;

namespace xomango.layers
{
    class TurnsLayer: Layer
    {
        ContentManager content;
        Board board;
        Vector2 drawSize;

        public event EventHandler<TouchEventArgs> ScreenTaped;

        public override void HandleInput(GestureSample sample)
        {
            if (sample.GestureType == GestureType.Tap)
            {
                Position pos;
                if (translateTapPosition(sample.Position, out pos) && ScreenTaped != null)
                {
                    ScreenTaped(this, new TouchEventArgs(pos));
                }
            }
        }

        public void Clear()
        {
            curentTurnAnimation = null;
            canInput = true;
            turns.Clear();
        }

        public TurnsLayer(ContentManager contentManager, Board gameBoard, Vector2 drawSize)
        {
            content = contentManager;
            board = gameBoard;
            this.drawSize = drawSize;
        }

        private Animation createTurnAnimation(Texture2D texture, int size, int framesCount)
        {
            Point frameSize = new Point(size, size);
            Point sheetSize = new Point(framesCount, 1);
            TimeSpan frameInterval = TimeSpan.FromSeconds(1.0f / 40);
            return new Animation(texture, frameSize, sheetSize, frameInterval);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
      
            crossFrames = content.Load<Texture2D>("animation/cross_animation");
            cross = createTurnAnimation(crossFrames, 60, 10);
            cross.Stop();
            zeroFrames = content.Load<Texture2D>("animation/zero_animation");
            zero = createTurnAnimation(zeroFrames, 60, 10);
            zero.Stop();
        }

        public override void Update(GameTime gameTime)
        {
            canInput = true;
            if (curentTurnAnimation != null)
            {
                canInput = !curentTurnAnimation.Update(gameTime);
                if (canInput)
                {
                    //Animation ended
                    turns.Add(curentTurn.position);
                    curentTurnAnimation = null;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            foreach (Position p in turns)
            {
                if (board[p] == Side.Cross)
                {
                    cross.Draw(spriteBatch, new Vector2(p.row * cellSize, p.column * cellSize), SpriteEffects.None);
                }
                else
                {
                    zero.Draw(spriteBatch, new Vector2(p.row * cellSize, p.column * cellSize), SpriteEffects.None);
                }
            }


            if (curentTurnAnimation != null)
            {
                curentTurnAnimation.Draw(spriteBatch, new Vector2(curentTurn.position.row * cellSize, curentTurn.position.column * cellSize), SpriteEffects.None);
            }

            lastVisibleRect = rect;
        }

        public void OnTurnDone(object sender, TurnEventArgs args)
        {
            if (curentTurnAnimation != null)
            {
                //Somedoby tapping too fast
                turns.Add(curentTurn.position);
            }

            curentTurn = args.Turn;
            if (curentTurn.side == Side.Cross)
            {
                curentTurnAnimation = createTurnAnimation(crossFrames, 60, 10);
            }
            else if (curentTurn.side == Side.Zero)
            {
                curentTurnAnimation = createTurnAnimation(zeroFrames, 60, 10);
            }
            curentTurnAnimation.Repeat = false;
        }

        private bool translateTapPosition(Vector2 tap, out Position pos)
        {
            float globalX = lastVisibleRect.X + tap.X;
            float globalY = lastVisibleRect.Y + tap.Y;

            if (!lastVisibleRect.Contains(new Point((int)globalX, (int)globalY)))
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

        Rectangle lastVisibleRect;
        int cellSize = GameOptions.Instanse.CellSize;
        bool canInput = true;
        Animation cross, zero;
        Animation curentTurnAnimation;
        Turn curentTurn;
        List<Position> turns = new List<Position>();
        Texture2D crossFrames;
        Texture2D zeroFrames;
        
    }
}
