using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Ball : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Private Members
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        // Default speed of ball
        private const float DEFAULT_SPEED = 150;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 50;

        // Ball location
        private Vector3 ballPosition;

        // Ball's motion
        private Vector2 ballSpeed = new Vector2(DEFAULT_SPEED, DEFAULT_SPEED);

        // Collision limiter
        private bool isColliding;

        // Texture stuff
        Texture2D texture;
        Point frameSize = new Point(64, 64);
        Point currentFrame = new Point(0, 0);
        Point sheetSize = new Point(12, 0);

        // Framerate stuff
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 50;

        //file content handles
        protected String spriteImage = @"Content\Images\pballss";
        #endregion
       
        #region Properties
        /// <summary>
        /// Gets or sets the ball's horizontal speed.
        /// </summary>
        public float SpeedX
        {
            get { return ballSpeed.X; }
            set { ballSpeed.X = value; }
        }

        /// <summary>
        /// Gets or sets the ball's vertical speed.
        /// </summary>
        public float SpeedY
        {
            get { return ballSpeed.Y; }
            set { ballSpeed.Y = value; }
        }

        /// <summary>
        /// Gets or sets the X position of the ball.
        /// </summary>
        public float X
        {
            get { return ballPosition.X; }
            set { ballPosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the ball.
        /// </summary>
        public float Y
        {
            get { return ballPosition.Y; }
            set { ballPosition.Y = value; }
        }

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

        /// <summary>
        /// Gets the bounding rectangle of the ball.
        /// </summary>
        public Rectangle BoundaryRectangle
        {
            get
            {
                return new Rectangle((int)ballPosition.X, (int)ballPosition.Y,
                    64, 64);
            }
        }
        /// <summary>
        /// Gets the bounding sphere of the ball.
        /// </summary>
        public BoundingSphere Boundary
        {
            get
            {
                return new BoundingSphere(ballPosition, 32);
            }
        }

        public bool IsColliding
        {
            get
            {
                return isColliding;
            }
            set
            {
                isColliding = value;
            }
        }
        #endregion

        public Ball(Game game)
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

            ballPosition.Y = (GraphicsDevice.Viewport.Height - Height)/2;
            ballPosition.X = (GraphicsDevice.Viewport.Width - Width) / 2;

            Random rNumber = new Random();

            ballSpeed.X = (float)rNumber.Next((int)(DEFAULT_SPEED / 2), (int)DEFAULT_SPEED);

            ballSpeed.Y = (DEFAULT_SPEED * 2) - ballSpeed.X;

            int direction = rNumber.Next(1, 4);

            if (direction % 2 == 0)
            {
                ballSpeed.X *= -1;
            }
            if (direction < 3)
            {
                ballSpeed.Y *= -1;
            }
            
        }

        /// <summary>
        /// Increase the ball's speed in the X and Y directions.
        /// </summary>
        public void SpeedUp()
        {
            if (ballSpeed.Y < 0)
                ballSpeed.Y -= INCREASE_SPEED;
            else
                ballSpeed.Y += INCREASE_SPEED;

            if (ballSpeed.X < 0)
                ballSpeed.X -= INCREASE_SPEED;
            else
                ballSpeed.X += INCREASE_SPEED;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            ballPosition.X = 360;
            ballPosition.Y = 200;

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
            // Move the sprite by speed, scaled by elapsed time.
            X += ballSpeed.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            Y += ballSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            Vector2 ballPosition2 = new Vector2();
            ballPosition2.X = X;
            ballPosition2.Y = Y;

            spriteBatch.Draw(texture, ballPosition2,
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
