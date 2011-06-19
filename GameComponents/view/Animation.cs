using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xomango.utils
{
    class Animation
    {
        #region Fields
        // The texture with animation frames
        private Texture2D animationTexture;
        // The size and structure of whole frames sheet in animationTexture. The animationTexture could
        // hold animaton sequence organized in multiple rows and multiple columns, that's why animation 
        // engine should know how the frames are organized inside a frames sheet
        private Point sheetSize;
        // Amount of time between frames
        private TimeSpan frameInterval;
        // Time passed since last frame
        private TimeSpan nextFrame;

        // Current frame in the animation sequence
        private Point currentFrame;
        // The size of single frame inside the animationTexture
        private Point frameSize;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor of an animation class
        /// </summary>
        /// <param name="frameSheet">Texture with animation frames sheet</param>
        /// <param name="size">Single frame size</param>
        /// <param name="frameSheetSize">The whole frame sheet size</param>
        /// <param name="interval">Interval between progressing to the next frame</param>
        public Animation(Texture2D frameSheet, Point size, Point frameSheetSize, TimeSpan interval)
        {
            animationTexture = frameSheet;
            frameSize = size;
            sheetSize = frameSheetSize;
            frameInterval = interval;
            Repeat = false;
        }


        /// <summary>
        /// Specify wheather we should repeat animation after it's stoped, if set to false the last frame is 
        /// drawn after completion. False by default.
        /// </summary>
        public bool Repeat { get; set; }
        #endregion

        #region Update and Render
        /// <summary>
        /// Updates the animaton progress
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="progressed">Returns true if animation were progressed; in such case 
        /// caller could updated the position of the animated character</param>
        public bool Update(GameTime gameTime)
        {
            if (currentFrame.X == sheetSize.X - 1 && currentFrame.Y == sheetSize.Y - 1 && !Repeat)
            {
                return false;
            }

            // Check is it is a time to progress to the next frame
            if (nextFrame >= frameInterval)
            {
                // Progress to the next frame in the row
                currentFrame.X++;
                // If reached end of the row advance to the next row 
                // and start form the first frame there
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    currentFrame.Y++;
                }
                // If reached last row in the frame sheet jump to the first row again - produce endless loop
                if (currentFrame.Y >= sheetSize.Y)
                    currentFrame.Y = 0;

                // Reset interval for next frame
                nextFrame = TimeSpan.Zero;
            }
            else
            {
                // Wait for the next frame 
                nextFrame += gameTime.ElapsedGameTime;
            }

            return true;
        }

        public void Stop()
        {
            currentFrame.X = sheetSize.X - 1;
            currentFrame.Y = sheetSize.Y - 1;
            Repeat = false;
        }

        /// <summary>
        /// Rendering of the animation
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch in which current frame will be rendered</param>
        /// <param name="position">The position of current frame</param>
        /// <param name="spriteEffect">SpriteEffect to apply on current frame</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
        {
            Draw(spriteBatch, position, 1.0f, spriteEffect);
        }

        /// <summary>
        /// Rendering of the animation
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch in which current frame will be rendered</param>
        /// <param name="position">The position of the current frame</param>
        /// <param name="scale">Scale factor to apply on the current frame</param>
        /// <param name="spriteEffect">SpriteEffect to apply on the current frame</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, SpriteEffects spriteEffect)
        {
            spriteBatch.Draw(animationTexture, position, new Rectangle(
                  frameSize.X * currentFrame.X,
                  frameSize.Y * currentFrame.Y,
                  frameSize.X,
                  frameSize.Y),
                  Color.White, 0f, Vector2.Zero, scale, spriteEffect, 0);
        }
        #endregion
    }
}
