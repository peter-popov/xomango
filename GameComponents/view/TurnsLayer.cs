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
using GameComponents.Control;

namespace GameComponents.View
{
    class TurnsLayer: Layer
    {
        public event EventHandler<TouchEventArgs> ScreenTaped;
        public event EventHandler<EventArgs> TurnAnimationFinished;

        public override void HandleInput(GestureSample sample)
        {
            if (sample.GestureType == GestureType.Tap && canInput)
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
            lastCellHighlight = content.Load<Texture2D>("textures/cell_highlight");
        }

        public override void Update(GameTime gameTime)
        {
            canInput = true;
            if (curentTurnAnimation != null)
            {
                canInput = !curentTurnAnimation.Update(gameTime);
                if (canInput)
                {
                    if (TurnAnimationFinished != null)
                    {
                        TurnAnimationFinished(this, new EventArgs());
                    }
                    curentTurnAnimation = null;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            foreach (Turn turn in board.Turns)
            {
                Position p = turn.position;                
                //Do not draw turn which is currently animated
                if (curentTurnAnimation != null && p == curentTurn.position)
                {
                    continue;
                }
                if (p == curentTurn.position)
                {
                    spriteBatch.Draw(lastCellHighlight, new Vector2(p.row * cellSize, p.column * cellSize), Color.White);
                }
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

        ContentManager content;
        Board board;
        Vector2 drawSize;
        Rectangle lastVisibleRect;
        int cellSize = GameOptions.Instanse.CellSize;
        bool canInput = true;
        Animation cross, zero;
        Animation curentTurnAnimation;
        Turn curentTurn;
        Texture2D crossFrames;
        Texture2D zeroFrames;
        Texture2D lastCellHighlight;
    }
}
