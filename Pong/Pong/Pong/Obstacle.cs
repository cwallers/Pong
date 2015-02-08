using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Obstacle : Physical
    {
        #region Private Members

        // Default speed of ball
        private const float DEFAULT_SPEED = 150;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 50;

        // Collision limiter
        //private bool isColliding;

        // Texture stuff
        Texture2D texture;
        Point frameSize = new Point(64, 64);
        Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(12, 0);

        // Framerate stuff
        protected int timeSinceLastFrame = 0;
        protected int millisecondsPerFrame = 50;

        //file content handles
        protected String spriteImage = @"Content\Images\pballss";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the width of the ball's sprite.
        /// </summary>
        public int Width
        {
            get { return frameSize.X; }
        }

        /// <summary>
        /// Gets the height of the ball's sprite.
        /// </summary>
        public int Height
        {
            get { return frameSize.Y; }
        }

        #endregion

        public Obstacle(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
        }

        /// <summary>
        /// Set the ball at the top of the screen with default speed.
        /// </summary>
        public void Reset()
        {
            isColliding = false;

            PositionY = (GraphicsDevice.Viewport.Height - Height) / 2;
            PositionX = (GraphicsDevice.Viewport.Width - Width) / 2;

            Random rNumber = new Random();

            VelocityX = (float)rNumber.Next((int)(DEFAULT_SPEED / 2), (int)DEFAULT_SPEED);

            VelocityY = (DEFAULT_SPEED * 2) - VelocityX;

            int direction = rNumber.Next(1, 4);

            if (direction % 2 == 0)
            {
                VelocityX *= -1;
            }
            if (direction < 3)
            {
                VelocityY *= -1;
            }

        }

        

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            Mass = 10;

            PositionX = 360;
            PositionY = 100;

            SizeX = Width;
            SizeY = Height;
            SizeR = Height / 2;

            isObstacle = true;

            Random rNumber = new Random();

            VelocityX = (float)rNumber.Next((int)(DEFAULT_SPEED / 2), (int)DEFAULT_SPEED);

            VelocityY = (DEFAULT_SPEED * 2) - VelocityX;

            int direction = rNumber.Next(1, 4);

            if (direction % 2 == 0)
            {
                VelocityX *= -1;
            }
            if (direction < 3)
            {
                VelocityY *= -1;
            }


            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = contentManager.Load<Texture2D>(spriteImage);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            #region Sprite Image Movement
            // Move the sprite by speed, scaled by elapsed time.
            PositionX += VelocityX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            PositionY += VelocityY * (float)gameTime.ElapsedGameTime.TotalSeconds;

            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds; if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame -= millisecondsPerFrame;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                        currentFrame.Y = 0;
                }
            }
            #endregion

            #region Sprite Physical Movement


            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            spriteBatch.Draw(texture, new Vector2(PositionX, PositionY),
               new Rectangle(currentFrame.X * frameSize.X,
                   currentFrame.Y * frameSize.Y,
                   frameSize.X,
                   frameSize.Y),
               Color.White, 0, Vector2.Zero,
               1, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}
