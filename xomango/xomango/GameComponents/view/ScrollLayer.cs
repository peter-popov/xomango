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
    public class ViewRectChangedEventArgs : EventArgs
    {
        public ViewRectChangedEventArgs(Rectangle rect)
        {
            this.rect = rect;
        }

        public readonly Rectangle rect;
    }

    class ScrollLayer: Layer
    {
        public ScrollLayer(Layer view, Rectangle boundingRect)
        {
            innerView = view;
            currentView = boundingRect;
            viewport = new Viewport(boundingRect);
            VirtualViewport = boundingRect;
        }

        public void Reset()
        {
            currentView.X = 0;
            currentView.Y = 0;
            VirtualViewport = currentView;
            Scroll(new Vector2(0, 0));
        }

        public override void Initialize()
        {            
            innerView.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            innerView.Update(gameTime);    
        }
        
        public void HandleInput(GestureSample sample)
        {
                switch (sample.GestureType)
                {
                    case GestureType.FreeDrag:
                        Scroll(sample.Delta);
                        break;
                }
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle viewportRect)
        {          
            //save old viewport
            Viewport oldViewPort = spriteBatch.GraphicsDevice.Viewport;
            spriteBatch.GraphicsDevice.Viewport = new Viewport(viewportRect);
            spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, null, translation);

            innerView.Draw(spriteBatch, currentView);

            spriteBatch.End();
            // restore viewport
            spriteBatch.GraphicsDevice.Viewport = oldViewPort;
        }

        public void Scroll(Vector2 delta)
        {
            offset -= delta;

            offset.X = Math.Max(offset.X, VirtualViewport.X);
            offset.Y = Math.Max(offset.Y, VirtualViewport.Y);

            offset.X = Math.Min(offset.X, VirtualViewport.Right - viewport.Width);
            offset.Y = Math.Min(offset.Y, VirtualViewport.Bottom - viewport.Height);

            translation = Matrix.CreateTranslation(-offset.X, -offset.Y, 0);
            currentView = new Rectangle((int)offset.X, (int)offset.Y, viewport.Width, viewport.Height);       
            if (Scrolled != null)
            {
                Scrolled(this, new ViewRectChangedEventArgs(currentView));
            }
        }

        public Rectangle VirtualViewport
        {
            get;
            set;
        }

        public EventHandler<ViewRectChangedEventArgs> Scrolled;

        private Matrix translation;
        private Vector2 offset;
        private Rectangle currentView;
        private Viewport viewport;        
        private Layer innerView;
    }
}
