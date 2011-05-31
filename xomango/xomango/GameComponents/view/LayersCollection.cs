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
        private List<Layer> layers = new List<Layer>();


        public void AddLayer(Layer view)
        {
            layers.Add(view);
        }

        public void RemoveLayer(Layer view)
        {
            layers.Remove(view);
        }

        public override void Initialize()
        {
            layers.ForEach((Layer v) => v.Initialize());
        }

        public override void Update(GameTime gameTime)
        {
            layers.ForEach((Layer v) => v.Update(gameTime));    
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle rect)
        {
            layers.ForEach((Layer v) => v.Draw(spriteBatch, rect));
        }

        public override void HandleInput(GestureSample sample)
        {
            layers.ForEach( l => l.HandleInput(sample) );
        }
    }
}
