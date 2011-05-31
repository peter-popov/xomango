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
    class XoGame
    {
        GameControler gameController;
        ContentManager content;
        Rectangle screenRect;
        layers.ScrollLayer scrollLayer;
        layers.LayersCollection gameLayers;
        SpriteBatch spriteBatch;
        TurnsLayer turnsLayer;

        public XoGame(GameControler gameController, Rectangle rect, ContentManager content)
        {
            this.gameController = gameController;
            this.screenRect = rect;
            this.content = content;
            //this.EnabledGestures = GestureType.FreeDrag | GestureType.Tap;
            gameLayers = new LayersCollection();
            scrollLayer = new ScrollLayer(gameLayers, screenRect);            
        }

        public event EventHandler<ViewRectChangedEventArgs> VisibleRectChanged
        {
            add
            {
                scrollLayer.Scrolled += value;
            }
            remove
            {
                scrollLayer.Scrolled -= value;            
            }
        }

        public void Reset()
        {
            scrollLayer.Reset();
            turnsLayer.Clear();
        }

        private void InitPlayers()
        {
            control.HumanPlayer player1 = new control.HumanPlayer(gameController.GameBoard, "Player1", CoreCZ.Side.Cross);
           
            
            control.MachinePlayer player2 = new control.MachinePlayer(gameController.GameBoard, CoreCZ.Side.Zero);
            gameController.Player1 = player1;
            gameController.Player2 = player2;
            player1.OnTurnMade += player2.OnEnemyMadeTurn;
        }

        public void LoadContent()
        {
            InitPlayers();

            TouchPanel.EnabledGestures =
                GestureType.Tap |
                GestureType.FreeDrag;

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);
            // Initialize layers
            scrollLayer.Scroll(new Vector2(0, 0));
            //
            // Turn layer
            turnsLayer = new TurnsLayer(content, 
                                        gameController.GameBoard,
                                        new Vector2(screenRect.Width, screenRect.Height));
            
            turnsLayer.ScreenTaped += (gameController.Player1 as control.HumanPlayer).HandleInput;

            gameController.OnTurn += turnsLayer.OnTurnDone;

            gameLayers.AddLayer(turnsLayer);
            gameLayers.AddLayer(new GridLayer(content));
            gameLayers.AddLayer(new BackgroundLayer(content));

            // Will couse initialization of all others layers
            scrollLayer.Initialize();           

            gameController.OnTurn += NewTurnDone;

            gameController.Restart();
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

        public void Draw(GameTime gameTime)
        {
            scrollLayer.Draw(spriteBatch, GetScreenRectWithOrientation());
        }

        public event EventHandler<control.TouchEventArgs> ScreenTouched;

        private void NewTurnDone(object sender, TurnEventArgs args)
        {
            //need to calculate new scrolling space
            CoreCZ.Area area = gameController.GameBoard.BoundingBox;

            Point from = new Point(area.sw.row, area.sw.column);
            Point to = new Point(area.ne.row, area.ne.column);
            if (from.X > to.X || from.Y > to.Y)
            {
                //This is fucking imposible!
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

        private Rectangle GetScreenRectWithOrientation()
        {
            //if (ScreenManager.Game.Window.CurrentOrientation != DisplayOrientation.Portrait)
            //{
            //    return screenRect;
            //}
            return new Rectangle(screenRect.X, screenRect.Y, screenRect.Height, screenRect.Width);
        }

       

    }
}
