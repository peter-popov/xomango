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
            ScreenViewport = boundingRect;            
            VirtualViewport = boundingRect;
            currentView = boundingRect;
            graphicViewport = new Viewport(boundingRect.X, boundingRect.Y,
                Math.Min(boundingRect.Width, boundingRect.Height),
                Math.Max(boundingRect.Width, boundingRect.Height));
            graphicViewport.MaxDepth = 0.9f;
            graphicViewport.MinDepth = 0.1f;

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
        
        public override void HandleInput(GestureSample sample)
        {
            switch (sample.GestureType)
            {
                case GestureType.FreeDrag:
                    Scroll(sample.Delta);
                    break;
            }
            innerView.HandleInput(sample);
        }

        public override void Draw(SpriteBatch spriteBatch, Rectangle viewportRect)
        {          
            //save old viewport
            Viewport oldViewPort = spriteBatch.GraphicsDevice.Viewport;
            try
            {
                spriteBatch.GraphicsDevice.Viewport = new Viewport(viewportRect);
            }
            catch
            {
            }
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

            offset.X = Math.Min(offset.X, VirtualViewport.Right - ScreenViewport.Width);
            offset.Y = Math.Min(offset.Y, VirtualViewport.Bottom - ScreenViewport.Height);

            translation = Matrix.CreateTranslation(-offset.X, -offset.Y, 0);
            currentView = new Rectangle((int)offset.X, (int)offset.Y, ScreenViewport.Width, ScreenViewport.Height);       
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

        public Rectangle ScreenViewport
        {
            get
            {
                return screenViewport;
            }
            set
            {
                screenViewport = value;
                Scroll(new Vector2(0, 0));
            }
        }

        public EventHandler<ViewRectChangedEventArgs> Scrolled;

        private Viewport graphicViewport;
        private Rectangle screenViewport;
        private Rectangle currentView;

        private Matrix translation;
        private Vector2 offset;
        private Layer innerView;
    }
}
