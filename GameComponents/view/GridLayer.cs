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
       
        public GridLayer(ContentManager contentManager)
        {
            content = contentManager;
        }

        public override void Initialize()
        {
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
        }


    }
}
