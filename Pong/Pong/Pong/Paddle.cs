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
    public class Paddle : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Private Members
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;

        // Paddle sprite
        protected Texture2D paddleSprite;

        // Paddle location
        protected Vector2 paddlePosition;

        protected const float DEFAULT_Y_SPEED = 250;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the paddle horizontal speed.
        /// </summary>
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the X position of the paddle.
        /// </summary>
        public float X
        {
            get { return paddlePosition.X; }
            set { paddlePosition.X = value; }
        }

        /// <summary>
        /// Gets or sets the Y position of the paddle.
        /// </summary>
        public float Y
        {
            get { return paddlePosition.Y; }
            set { paddlePosition.Y = value; }
        }

        public int Width
        {
            get { return paddleSprite.Width; }
        }

        /// <summary>
        /// Gets the height of the paddle's sprite.
        /// </summary>
        public int Height
        {
            get { return paddleSprite.Height; }
        }

        /// <summary>
        /// Gets the bounding rectangle of the paddle.
        /// </summary>
        public Rectangle Boundary
        {
            get
            {
                return new Rectangle((int)paddlePosition.X, (int)paddlePosition.Y,
                    paddleSprite.Width, paddleSprite.Height);
            }
        }

        #endregion

        public Paddle(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            paddleSprite = contentManager.Load<Texture2D>(@"Content\Images\hand");
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {           
            spriteBatch.Begin();
            spriteBatch.Draw(paddleSprite, paddlePosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class PaddleHuman : Paddle 
    {
        public PaddleHuman(Game game) : base(game) {}
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            paddleSprite = contentManager.Load<Texture2D>(@"Content\Images\hand");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Scale the movement based on time
            float moveDistance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move paddle, but don't allow movement off the screen

            KeyboardState newKeyState = Keyboard.GetState();
            if (newKeyState.IsKeyDown(Keys.Down) && Y + paddleSprite.Height
                + moveDistance <= GraphicsDevice.Viewport.Height)
            {
                Y += moveDistance;
            }
            else if (newKeyState.IsKeyDown(Keys.Up) && Y - moveDistance >= 0)
            {
                Y -= moveDistance;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Make sure base.Initialize() is called before this or handSprite will be null
            X = 2;
            Y = (GraphicsDevice.Viewport.Height - Height)/2;

            Speed = DEFAULT_Y_SPEED;
        }
    }

    public class PaddleComputer : Paddle 
    {
        public PaddleComputer(Game game) : base(game) {}
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            paddleSprite = contentManager.Load<Texture2D>(@"Content\Images\hand");
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Make sure base.Initialize() is called before this or handSprite will be null
            X = GraphicsDevice.Viewport.Width - Width -  2;
            Y = (GraphicsDevice.Viewport.Height - Height) / 2;

            Speed = DEFAULT_Y_SPEED;
        }

        public override void Update(GameTime gameTime)
        {
            Ball ball = Game.Components[0] as Ball;

            // Scale the movement based on time
            float moveDistance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move paddle, but don't allow movement off the screen

            KeyboardState newKeyState = Keyboard.GetState();
         
            //// down
            //if ((Y + (Height / 2)) < (ball.Y - (ball.Height / 2)) && (Y + paddleSprite.Height + moveDistance) <= (GraphicsDevice.Viewport.Height))
            //{
            //    Y += moveDistance;
            //}
            //// slow down
            //else if ((Y + (Height / 2)) > (ball.Y - (ball.Height / 2)) && (Y + paddleSprite.Height + moveDistance) <= (GraphicsDevice.Viewport.Height))
            //{
            //    Y += moveDistance * ((ball.Y - Y) / 100);
            //}

            //// up
            // else if ((Y - (Height / 2)) > (ball.Y + (ball.Height / 2)) && (Y - paddleSprite.Height - moveDistance) >= 0)
            // {
            //     Y -= moveDistance;
            // }
            // // slow up
            // else if ((Y - (Height / 2)) < (ball.Y + (ball.Height / 2)) && (Y - paddleSprite.Height - moveDistance) >= 0)
            // {
            //     Y -= moveDistance * ((ball.Y + Y) / 100);
            // }



            // down
            if (Y <= ball.Y - Height / 2 - ball.Height / 2 && (Y + paddleSprite.Height + moveDistance) <= (GraphicsDevice.Viewport.Height))
            {
                Y += moveDistance;
            }
            // slow adjust
            else if ((Y > ball.Y - ball.Height/2 && Y < ball.Y + ball.Height/2) && (Y + paddleSprite.Height + moveDistance) <= (GraphicsDevice.Viewport.Height))
            {
                Y += moveDistance * ((ball.Y - Y) / 100);
            }
            // up
            else if (Y >= ball.Y + Height / 2 + ball.Height / 2 && (Y - paddleSprite.Height - moveDistance) >= 0)
            {
                Y -= moveDistance;
            }
            //stop

            else if(Y == ball.Y)
            {
                //don't move
            }
            base.Update(gameTime);
        }
    }
}
