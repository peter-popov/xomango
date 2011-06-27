//-----------------------------------------------------------------------------
// TextControl.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PlainGUI.Controls
{
    /// <summary>
    /// TextControl is a control that displays a single string of text. By default, the
    /// size is computed from the given text and spritefont.
    /// </summary>
    public class TextControl : Control
    {
        private SpriteFont font;
        private string[] lines;
        private string text;

        public Color Color;
        public float Scale;

        private float lineHeight;
        private static int lineIntreval = 3;

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

        public TextControl()
            : this(string.Empty, null, Color.White, Vector2.Zero)
        {
        }

        public TextControl(string text, SpriteFont font)
            : this(text, font, Color.White, Vector2.Zero)
        {
        }

        public TextControl(string text, SpriteFont font, Color color)
            : this(text, font, color, Vector2.Zero)
        {
        }

        public TextControl(string text, SpriteFont font, Color color, Vector2 position)
        {
            this.text = text;
            this.font = font;
            this.Position = position;
            this.Color = color;
            this.Scale = 1.0f;

            lines = text.Split('\n');
            lineHeight = font.MeasureString("H").Y;
        }

        public override void Draw(DrawContext context)
        {
            base.Draw(context);
            Vector2 pos = Vector2.Zero;
            foreach (string s in lines)
            {
                context.SpriteBatch.DrawString(font, s, context.DrawOffset, Color, 0, pos, Scale, SpriteEffects.None, 0);
                pos.Y -= lineHeight + lineIntreval;
            }
        }

        override public Vector2 ComputeSize()
        {
            if (lines.Length == 0) return Vector2.Zero;
            float w = 0;
            foreach (string s in lines)
            {                
                w = Math.Max(w, font.MeasureString(s).X);
            }

            return new Vector2(w, lineHeight*lines.Length - lineIntreval);
        }
    }
}
