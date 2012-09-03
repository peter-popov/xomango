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
    class GridLayer: Layer
    {
        ContentManager content;
        Texture2D cell;
        SpriteFont indexFont;
       
        public GridLayer(ContentManager contentManager)
        {
            content = contentManager;
        }

        public override void Initialize()
        {
            indexFont = content.Load<SpriteFont>("MenuFont");
            cell = content.Load<Texture2D>("textures/cell");
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            int cellSize = GameOptions.Instanse.CellSize;
            int refX = rect.X - rect.X % cellSize - cellSize;
            int refY = rect.Y - rect.Y % cellSize - cellSize; 
            int countX = rect.Width / cellSize + 1;
            int countY = rect.Height / cellSize + 3;
            
            for (int i = 0; i <= countX; i++)
            {
                for (int j = 0; j <= countY; j++)
                {
                    Vector2 pos = new Vector2(refX + cellSize * i, refY + cellSize * j);
                    spriteBatch.Draw(cell, pos, Color.White);
                }
            }

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
