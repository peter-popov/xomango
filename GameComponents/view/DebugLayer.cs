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
    class DebugLayer : Layer
    {
        ContentManager content;
        SpriteFont indexFont;

        public DebugLayer(ContentManager contentManager)
        {
            content = contentManager;
        }

        public override void Initialize()
        {
            indexFont = content.Load<SpriteFont>("MenuFont");
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            int cellSize = GameOptions.Instanse.CellSize;
            // Print cell numbers
            for (int i = 0; i <= 20; i++)
            {
                for (int j = 0; j <= 20; j++)
                {
                    Vector2 pos = new Vector2(cellSize * i, cellSize * j);
                    spriteBatch.DrawString(indexFont,
                    String.Format("{0},{1}", i, j),
                    pos + new Vector2(3, 3),
                    Color.Gray,
                    0,
                    Vector2.Zero,
                    0.4f,
                    SpriteEffects.None,
                    0);
                }
            }
        }


    }
}
