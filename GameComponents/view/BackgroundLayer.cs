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
            background = content.Load<Texture2D>("textures/BoardBackground");
        }

        public override void Update(GameTime gameTime)
        {

        }


        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            var w = background.Width;
            var h = background.Height;
            var x = rect.X - rect.X % w;
            var y = rect.Y - rect.Y % h;
            h *= Math.Sign(rect.Y);
            w *= Math.Sign(rect.X);

            spriteBatch.Draw(background, new Vector2(x, y), Color.White);
            spriteBatch.Draw(background, new Vector2(x+w, y), Color.White);
            spriteBatch.Draw(background, new Vector2(x, y+h), Color.White);
            spriteBatch.Draw(background, new Vector2(x+w, y+h), Color.White);
        }
    }
}
