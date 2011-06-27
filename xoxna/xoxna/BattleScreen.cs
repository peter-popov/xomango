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
using PlainGUI;
using GameComponents;
using GameComponents.Control;
using GameComponents.View;

namespace xo5
{
    public class EitbitRenderTarget2D : RenderTarget2D
    {

        public EitbitRenderTarget2D(GraphicsDevice device, int width, int height)
            : base(device, width, height)
        {
        }

        public void MakeGrayscale()
        {
            Color[] bitmap = new Color[Width * Height];

            GetData(bitmap);

            for (int i = 0; i < Width * Height; i++)
            {
                byte grayscale = (byte)(bitmap[i].R * 0.3f +
                    bitmap[i].G * 0.59f + bitmap[i].B * 0.11f);
                bitmap[i].R = grayscale;
                bitmap[i].G = grayscale;
                bitmap[i].B = grayscale;
            }

            SetData(bitmap);
        }
    }


    public class ScreenManagerInputEnumerator : IInputEnumerator
    {
        public bool IsGestureAvaliable
        {
            get
            {
                return input != null && index >= 0 && index < input.Gestures.Count;
            }
        }

        public GestureSample ReadGesture()
        {
            return input.Gestures[index++];
        }

        public void add(InputState input)
        {
            this.input = input;
            index = 0;
        }

        private InputState input;
        private int index = 0;
    }

    public class BattleScreen: GameScreen
    {
        GameScreen parent;
        GameControler gameControler;
        Rectangle screenRect;
        XoGame game;
        GraphicsDevice device;
        ScreenManagerInputEnumerator inputEnumerator = new ScreenManagerInputEnumerator();

        public BattleScreen(GameScreen parent, GraphicsDevice device, GameControler gameController, Rectangle rect)
        {
            this.parent = parent;
            this.device = device;
            this.screenRect = rect;
            this.gameControler = gameController;
            this.EnabledGestures = GestureType.FreeDrag | GestureType.Tap;        
        }

        public override void LoadContent()
        {
            game = new XoGame(device, gameControler, screenRect, ScreenManager.Game.Content, inputEnumerator);
            game.LoadContent();
            game.TurnAnimationEvent += TurnAnimationEnded;
            base.LoadContent();
        }

        public void RenderGrayscaleToTexture(EitbitRenderTarget2D target)
        {
            ScreenManager.GraphicsDevice.SetRenderTarget(target);
            game.Draw();
            ScreenManager.GraphicsDevice.SetRenderTarget(null);
            target.MakeGrayscale();
        }

        public void TurnAnimationEnded(object sender, EventArgs args)
        {
            if (gameControler.GameBoard.Winner)
            {
                VictoryScreen scr = new VictoryScreen(parent, gameControler.CurrentPlayer.Type == PlayerType.Human);
                EitbitRenderTarget2D bg = new EitbitRenderTarget2D(ScreenManager.GraphicsDevice, screenRect.Width, screenRect.Height);
                RenderGrayscaleToTexture(bg);
                scr.Background = bg;
                scr.BattleScreen = this;

                ScreenManager.AddScreen(scr, null);
            }
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, null, out player))
            {
                if (gameControler != null)
                {
                    gameControler.Save();
                }
                ExitScreen();
            }

            inputEnumerator.add(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            
            
            if (game != null) game.Update(gameTime);
            

            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            if (game != null) game.Draw();
            base.Draw(gameTime);
        }
    }
}
