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

namespace xomango.layers
{
    class LayersCollection: Layer
    {
        private List<Layer> views = new List<Layer>();


        public void AddView(Layer view)
        {
            views.Add(view);
        }

        public void RemoveView(Layer view)
        {
            views.Remove(view);
        }

        public override void Initialize()
        {
            views.ForEach((Layer v) => v.Initialize());
        }

        public override void Update(GameTime gameTime)
        {
            views.ForEach((Layer v) => v.Update(gameTime));    
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            views.ForEach((Layer v) => v.Draw(spriteBatch, rect));
        } 
    }
}
