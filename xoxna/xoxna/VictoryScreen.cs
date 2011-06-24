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
        public VictoryScreen()
        {
            RootControl = new PanelControl();
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
            TextControl text = new TextControl("Test the screen!", ScreenManager.Font, Color.Black, new Vector2(10, 20));
            //background = ScreenManager.Game.Content.Load<Texture2D>("textures/comicpanel10");

            Button ok = new Button("OK", ScreenManager.Font, Color.Black, new Vector2(10, 100));
            
            Button cancel = new Button("Cancel", ScreenManager.Font, Color.Black, new Vector2(250, 100));
            
            RootControl.AddChild(text);
            RootControl.AddChild(ok);
            RootControl.AddChild(cancel);
            
            base.LoadContent();
        }

        public Texture2D Background;

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            ScreenManager.SpriteBatch.Draw(Background, Vector2.Zero, Color.White);

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
