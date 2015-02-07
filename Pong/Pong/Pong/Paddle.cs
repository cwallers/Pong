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
    public class Paddle : Physical
    {
        #region Private Members

        // Paddle sprite
        protected Texture2D paddleSprite;

        //Default paddle speed
        protected const float DEFAULT_Y_SPEED = 500;

        //used for the switching of controls
        protected bool isMouseControl = false;
        protected KeyboardState oldKBState = new KeyboardState();
        protected MouseState mouseStateCurrent, mouseStatePrevious;
        
        //file content handles
        protected String playerImage = @"Content\Images\squirtle";
        protected String computerImage = @"Content\Images\Pikachu2";
        #endregion

        #region Properties
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
            
            paddleSprite = contentManager.Load<Texture2D>(playerImage);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(paddleSprite, new Vector2(PositionX,PositionY), Color.White);
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
            paddleSprite = contentManager.Load<Texture2D>(playerImage);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            Ball ball = Game.Components[0] as Ball;

            //allows for use of mouse and key shift by pressing the space bar
            if (Keyboard.GetState().IsKeyDown(Keys.M) && !oldKBState.IsKeyDown(Keys.M))
            {
                isMouseControl = !isMouseControl; // reverse the control keys
            }

            // Scale the movement based on time
            float moveDistance = DEFAULT_Y_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move paddle, but don't allow movement off the screen
            
            KeyboardState newKeyState = Keyboard.GetState();
            mouseStatePrevious = mouseStateCurrent;
            mouseStateCurrent = Mouse.GetState();

            float yBeforeMove = PositionY;

            if (isMouseControl)
            {
                float move = moveDistance;
                float edge = GraphicsDevice.Viewport.Height - SizeY;

                if (PositionY > mouseStateCurrent.Y)
                {
                    move *= -1;
                    edge = 0;
                }

                if (Math.Abs(PositionY - mouseStateCurrent.Y) < ((ball.SizeY / 2) + (SizeY / 2)))     //paren - thesis!!
                {
                    move *= (Math.Abs(PositionY - mouseStateCurrent.Y) / ((ball.SizeY / 2) + (SizeY / 2)));
                }

                PositionY += move;

                if (PositionY < 0 || PositionY + SizeY > GraphicsDevice.Viewport.Height)
                {
                    PositionY = edge;
                }

            }
            else
            {
                if (newKeyState.IsKeyDown(Keys.Down) && PositionY + SizeY
                    + moveDistance <= GraphicsDevice.Viewport.Height)
                {
                    PositionY += moveDistance;
                }
                else if (newKeyState.IsKeyDown(Keys.Up) && PositionY - moveDistance >= 0)
                {
                    PositionY -= moveDistance;
                }
            }

            oldKBState = newKeyState;
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
            SizeX = (float)paddleSprite.Width;
            SizeY = (float)paddleSprite.Height;
            SizeR = (float)(paddleSprite.Height / 2);
            
            PositionX = 2;
            PositionY = (GraphicsDevice.Viewport.Height - SizeY) / 2;

            VelocityY = DEFAULT_Y_SPEED;

            
        }
    }

    public class PaddleComputer : Paddle 
    {
        public PaddleComputer(Game game) : base(game) {}
        
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            paddleSprite = contentManager.Load<Texture2D>(computerImage);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Make sure base.Initialize() is called before this or handSprite will be null
            SizeX = (float)paddleSprite.Width;
            SizeY = (float)paddleSprite.Height;
            SizeR = (float)(paddleSprite.Height / 2);

            // Make sure base.Initialize() is called before this or handSprite will be null
            PositionX = GraphicsDevice.Viewport.Width - SizeX -  2;
            PositionY = (GraphicsDevice.Viewport.Height - SizeY) / 2;

            
        }

        public override void Update(GameTime gameTime)
        {
            Ball ball = Game.Components[0] as Ball;

            // Scale the movement based on time
            float moveDistance = DEFAULT_Y_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move paddle, but don't allow movement off the screen

            KeyboardState newKeyState = Keyboard.GetState();

            float move = moveDistance;
            float edge = GraphicsDevice.Viewport.Height - SizeY;


            if (PositionY > ball.PositionY)
            {
                move *= -1;
                edge = 0;
            }

            if (Math.Abs(PositionY - ball.PositionY) < ((ball.SizeY / 2) + (SizeY / 2)))     //paren - thesis!!
            {
                move *= (Math.Abs(PositionY - ball.PositionY) / ((ball.SizeY / 2) + (SizeY / 2)));
            }

            PositionY += move;

            if (PositionY < 0 || PositionY + SizeY > GraphicsDevice.Viewport.Height)
            {
                PositionY = edge;
            }
            base.Update(gameTime);
        }
    }
}
