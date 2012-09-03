using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using GameComponents.Control;
using GameComponents.View;

namespace GameComponents
{

    public enum DifficultyLevel : int
    {
        EASY = 1,
        HARD = 2  
    }

    public interface IInputEnumerator
    {
        bool IsGestureAvaliable {get;}
        GestureSample ReadGesture();
    };

    class DefaultInputEnumerator : IInputEnumerator
    {
        public bool IsGestureAvaliable
        {
            get
            {
                return TouchPanel.IsGestureAvailable;
            }
        }

        public GestureSample ReadGesture()
        {
            return TouchPanel.ReadGesture();
        }
    }

    public class XoGame
    {
        GameControler gameController;
        ContentManager content;
        Rectangle screenRect;        
        ScrollLayer scrollLayer;
        LayersCollection gameLayers;
        SpriteBatch spriteBatch;
        TurnsLayer turnsLayer;
        GraphicsDevice device;
        IInputEnumerator inputEnumerator;

        public XoGame(GraphicsDevice device, GameControler gameController, Rectangle rect, ContentManager content)
            : this(device, gameController, rect, content, new DefaultInputEnumerator())
        {            
        }

        public XoGame(GraphicsDevice device, GameControler gameController, Rectangle rect, ContentManager content, IInputEnumerator input)
        {
            this.gameController = gameController;
            //this.spriteBatch = sb;
            this.screenRect = rect;
            this.content = content;
            this.device = device;
            this.inputEnumerator = input;
            gameLayers = new LayersCollection();
            scrollLayer = new ScrollLayer(gameLayers, screenRect);            
        }
        
        public void Reset()
        {
            scrollLayer.Reset();
            turnsLayer.Clear();
        }

        public void LoadContent()
        {
            TouchPanel.EnabledGestures =
                GestureType.Tap |
                GestureType.FreeDrag;

            // Create a new SpriteBatch, which can be used to draw textures.
            if (spriteBatch == null)
            {
                spriteBatch = new SpriteBatch(device);
            }
            // Initialize layers
            scrollLayer.Scroll(new Vector2(0, 0));
            //
            // Turn layer
            turnsLayer = new TurnsLayer(content, 
                                        gameController.GameBoard,
                                        new Vector2(screenRect.Width, screenRect.Height));
            
            turnsLayer.ScreenTaped += gameController.HandleInput;
            gameController.OnTurn += turnsLayer.OnTurnDone;
            
            gameLayers.AddLayer(turnsLayer);            
            gameLayers.AddLayer(new GridLayer(content));
            //gameLayers.AddLayer(new DebugLayer(content));
            gameLayers.AddLayer(new BackgroundLayer(content));
            
            // Will couse initialization of all others layers
            scrollLayer.Initialize();           

            gameController.OnTurn += NewTurnDone;

            UpdateVirtualViewport();
        }

        public void Update(GameTime gameTime)
        {            
            while (inputEnumerator.IsGestureAvaliable)
            {
                scrollLayer.HandleInput(inputEnumerator.ReadGesture());
            }

            scrollLayer.Update(gameTime);
            gameController.Update(gameTime);
        }

        public void Draw()
        {
            scrollLayer.Draw(spriteBatch, screenRect);
        }

        private void NewTurnDone(object sender, TurnEventArgs args)
        {
            UpdateVirtualViewport();
        }

        private void UpdateVirtualViewport()
        {
            //need to calculate new scrolling space
            CoreCZ.Area area = gameController.GameBoard.BoundingBox;

            Point from = new Point(area.sw.row, area.sw.column);
            Point to = new Point(area.ne.row, area.ne.column);
            if (from.X > to.X || from.Y > to.Y)
            {
                //No turns have been made yet, st minimum rectangle
                scrollLayer.VirtualViewport = screenRect;
                return;
            }

            from.X += 1;
            from.Y += 1;

            int cellSize = GameOptions.Instanse.CellSize;

            scrollLayer.VirtualViewport = new Rectangle
              (from.X * cellSize - screenRect.Width,
              from.Y * cellSize - screenRect.Height,
              (to.X - from.X) * cellSize + 2 * screenRect.Width,
              (to.Y - from.Y) * cellSize + 2 * screenRect.Height
             );
        }

        public Rectangle ScreenRectangle
        {
            set
            {
                scrollLayer.ScreenViewport = value;
                screenRect = value;
                UpdateVirtualViewport();
            }
            get
            {
                return screenRect;
            }
        }

        public event EventHandler<EventArgs> TurnAnimationEvent
        {
            add 
            {
                if (turnsLayer != null)
                    turnsLayer.TurnAnimationFinished += value;
            }
            remove
            {
                if (turnsLayer != null)
                    turnsLayer.TurnAnimationFinished -= value;            
            }
        }
    }
}
