using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlainGUI.Controls
{
    public class Button:Control
    {
        private SpriteFont font;
        private string text;

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
        }

        public override void Draw(DrawContext context)
        {
            base.Draw(context);

            context.SpriteBatch.DrawString(font, Text, context.DrawOffset, Color);
        }
    }
}
