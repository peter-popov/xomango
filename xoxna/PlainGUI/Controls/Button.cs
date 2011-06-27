using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace PlainGUI.Controls
{
    public class Button:Control
    {
        private SpriteFont font;
        private string text;
        private Rectangle boundigBox;
        public Color Color;

        // Actual text to draw
        public string Text
        {
            get { return text; }
            set
            {
                if (text != value)
                {
                    text = value;
                    InvalidateAutoSize();
                }
            }
        }

        // Font to use
        public SpriteFont Font
        {
            get { return Font; }
            set
            {
                if (font != value)
                {
                    font = value;
                    InvalidateAutoSize();
                }
            }
        }

        public Button()
            : this(string.Empty, null, Color.White, Vector2.Zero)
        {
        }

        public Button(string text, SpriteFont font)
            : this(text, font, Color.White, Vector2.Zero)
        {
        }

        public Button(string text, SpriteFont font, Color color)
            : this(text, font, color, Vector2.Zero)
        {
        }

        public Button(string text, SpriteFont font, Color color, Vector2 position)
        {
            this.text = text;
            this.font = font;
            this.Position = position;
            this.Color = color;
            Size = font.MeasureString(text);
        }

        public override void Draw(DrawContext context)
        {
            base.Draw(context);

            context.SpriteBatch.DrawString(font, Text, context.DrawOffset, Color);
        }

        public event EventHandler<EventArgs> Pressed;

        public override void HandleInput(InputState input)
        {
            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
                if (gesture.GestureType == GestureType.Tap)
                {
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
                    Rectangle bb = new Rectangle((int)(Parent.Position.X + Position.X), (int)(Parent.Position.Y + Position.Y), (int)Size.X, (int)Size.Y);
                    if (bb.Contains(tapLocation) && Pressed != null)
                    {
                        Pressed(this, new EventArgs());
                    }
                }
            }
            base.HandleInput(input);
        }
                
    }
}
