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
using xomango.utils;
using xomango.layers;

namespace xomango
{
    public class XoGame
    {
        GameControler gameController;
        ContentManager content;
        Rectangle screenRect;
        layers.ScrollLayer scrollLayer;
        layers.LayersCollection gameLayers;
        SpriteBatch spriteBatch;
        TurnsLayer turnsLayer;
        IGraphicsDeviceService deviceService;

        public XoGame(IGraphicsDeviceService deviceService, GameControler gameController, Rectangle rect, ContentManager content)
        {
            this.gameController = gameController;
            this.screenRect = rect;
            this.content = content;
            this.deviceService = deviceService;
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
            spriteBatch = new SpriteBatch(deviceService.GraphicsDevice);
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
            gameLayers.AddLayer(new BackgroundLayer(content));

            // Will couse initialization of all others layers
            scrollLayer.Initialize();           

            gameController.OnTurn += NewTurnDone;

            UpdateVirtualViewport();
        }

        public void Update(GameTime gameTime)
        {            
            while (TouchPanel.IsGestureAvailable)
            {
                scrollLayer.HandleInput(TouchPanel.ReadGesture());
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
