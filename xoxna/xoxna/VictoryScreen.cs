using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using PlainGUI;
using PlainGUI.Controls;

namespace xo5
{
    public class VictoryScreen:SingleControlScreen
    {
        public VictoryScreen(GameScreen parent, bool victory)
        {
            this.victory = victory;
            RootControl = new PanelControl();
            mainScreen = (MainMenuScreen)parent;
            this.EnabledGestures = GestureType.Tap;
        }

        public override void HandleInput(InputState input)
        {
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, null, out player))
            {
                ExitScreen();
            }
            base.HandleInput(input);
        }
        
        public override void LoadContent()
        {
            screenBackground = ScreenManager.Game.Content.Load<Texture2D>("textures/comicpanel10");

            Vector2 textPos = position + new Vector2(10, 30);
            TextControl text = new TextControl(victory ? victoryText : defeatText, ScreenManager.Font, Color.Brown, textPos);
            text.Scale = 0.75f;
            
            Button ok = new Button("Yes", ScreenManager.Font, Color.Black, new Vector2(10, 100));
            ok.Pressed += OnOk;
            Button cancel = new Button("No", ScreenManager.Font, Color.Black, new Vector2(250, 100));
            cancel.Pressed += OnCancel;

            float buttonsY = screenRect.Bottom - 10 - ok.Size.Y;            
            float xpading = (480 - ok.Size.X - cancel.Size.X)/3;
            ok.Position = new Vector2(xpading, buttonsY);
            cancel.Position = new Vector2(ok.Position.X + ok.Size.X + xpading, buttonsY);
           
            RootControl.AddChild(text);
            RootControl.AddChild(ok);
            RootControl.AddChild(cancel);
            
            base.LoadContent();
        }



        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(Background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.Draw(screenBackground, screenRect, Color.White);
            

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }


        private void OnOk(object sender, EventArgs args)
        {
            mainScreen.NewGame(this, new PlayerIndexEventArgs(0));
        }

        private void OnCancel(object sender, EventArgs args)
        {
            ExitScreen();
            BattleScreen.ExitScreen();
        }

        private bool victory;
        public Texture2D Background;
        private Texture2D screenBackground;
        private MainMenuScreen mainScreen;
        public BattleScreen BattleScreen = null;
        private static string victoryText = "You win. Well done!\nTry again?";
        private static string defeatText = "You lose. Keep training!\nTry again?";

        private static Vector2 position = new Vector2(0, 200);

        private Rectangle screenRect = new Rectangle((int)position.X, (int)position.Y, 480, 260);
    }
}
