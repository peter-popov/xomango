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

namespace GameComponents.View
{
    class BackgroundLayer: Layer
    {
        ContentManager content;
        Texture2D background;

        public BackgroundLayer(ContentManager contentManager)
        {
            content = contentManager;
        }

        public override void Initialize()
        {
            background = content.Load<Texture2D>("textures/comicpanel8");
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            //Some kind of very clever logic should be here, goal - make texture to pan with the board
            // TODO: make samrt background paning

            //spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
        
            spriteBatch.Draw(background, new Vector2(rect.X, rect.Y), Color.White);
        }
    }
}
