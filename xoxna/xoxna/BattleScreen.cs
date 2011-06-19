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
    class ScreenManagerInputEnumerator : IInputEnumerator
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

    class BattleScreen: GameScreen
    {
        GameControler gameController;
        Rectangle screenRect;
        XoGame game;
        IGraphicsDeviceService deviceSrvice;
        ScreenManagerInputEnumerator inputEnumerator = new ScreenManagerInputEnumerator();

        public BattleScreen(IGraphicsDeviceService deviceSrvice, GameControler gameController, Rectangle rect)
        {
            this.deviceSrvice = deviceSrvice;
            this.gameController = gameController;
            this.screenRect = rect;
            this.EnabledGestures = GestureType.FreeDrag | GestureType.Tap;        
        }

        public void Reset()
        {
        }

        public override void LoadContent()
        {
            game = new XoGame(deviceSrvice, gameController, screenRect, ScreenManager.Game.Content, inputEnumerator);
            game.LoadContent();
            base.LoadContent();
        }

        public override void HandleInput(InputState input)
        {
            inputEnumerator.add(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {            
            game.Update(gameTime);                        
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            game.Draw();
            base.Draw(gameTime);
        }
    }
}
