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
        private const float DEFAULT_X_SPEED = 50;
        private const float DEFAULT_Y_SPEED = 50;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 50;

        // Ball image
        private Texture2D ballSprite;

        // Ball location
        Vector2 ballPosition;

        // Ball's motion
        Vector2 ballSpeed = new Vector2(DEFAULT_X_SPEED, DEFAULT_Y_SPEED);
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
            get { return ballSprite.Width; }
        }

        /// <summary>
        /// Gets the height of the ball's sprite.
        /// </summary>
        public int Height
        {
            get { return ballSprite.Height; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the ball.
        /// </summary>
        public Rectangle Boundary
        {
            get
            {
                return new Rectangle((int)ballPosition.X, (int)ballPosition.Y,
                    ballSprite.Width, ballSprite.Height);
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
            ballSpeed.X = DEFAULT_X_SPEED;
            ballSpeed.Y = DEFAULT_Y_SPEED;

            ballPosition.Y = (GraphicsDevice.Viewport.Height - Height)/2;
            ballPosition.X = (GraphicsDevice.Viewport.Width - Width) / 2;

            Random rNumber = new Random();
            int direction = new int();

            direction = rNumber.Next(0, 2);

            if (direction == 0)
            {
                ballSpeed.X *= -1;
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
        /// Invert the ball's horizontal direction
        /// </summary>
        public void ChangeHorzDirection()
        {
            ballSpeed.X *= -1;
        }

        /// <summary>
        /// Invert the ball's vertical direction
        /// </summary>
        public void ChangeVertDirection()
        {
            ballSpeed.Y *= -1;
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

            // Load the texture if it exists
            ballSprite = contentManager.Load<Texture2D>(@"Content\Images\basketball");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Move the sprite by speed, scaled by elapsed time.
            ballPosition += ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.Draw(ballSprite, ballPosition, Color.White);
            spriteBatch.End();
        }
    }
}
