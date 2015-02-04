/*
 * Basketball Pong
 * by Catherine Wallers, Harding University
 * Spring 2015
 * Base code by Frank McCown, Harding University
 * 
 * Sounds: Creative Commons Sampling Plus 1.0 License.
 * http://www.freesound.org/samplesViewSingle.php?id=34201
 * http://www.freesound.org/samplesViewSingle.php?id=12658
 */

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
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Game1 Properties
        private GraphicsDeviceManager graphics;

        private Ball ball;
        private PaddleHuman paddleHuman;
        private PaddleComputer paddleComputer;
        protected SpriteBatch spriteBatch;
        private SpriteFont font;
        protected int playerScore = 0;
        protected int computerScore = 0;

        // Used to delay between rounds 
        private float delayTimer = 0;

        //end game variables and constants
        private string winnerMsg = "Congrats you WON! \n Press 'N' to play again";
        private string loserMsg = "Sorry, you lost... \n Press 'N' to play again";
        private const int WIN_SCORE = 2;

        //credits
        private bool creditScreen = false;
        private string creditMsg = "Catherine Wallers \n Nathan Roberts \n Press 'E' to play again";
        private KeyboardState oldKeyState; 

        //background 
        Texture2D background;
        Rectangle mainFrame; //hold limits of the mainscreen

        //sound
        protected SoundEffect playerSoundEffect;
        protected SoundEffect computerSoundEffect;
        protected SoundEffect playerPointSoundEffect;
        protected SoundEffect computerPointSoundEffect;
        protected SoundEffect playerWinSoundEffect;
        protected SoundEffect computerWinSoundEffect;
        
        //file content handles
        protected String backgroundImage = @"Images\bg";
        protected String playerAudio = @"Audio\Squirtle";
        protected String computerAudio = @"Audio\Pikachu";
        protected String playerPointAudio = @"Audio\WOW";
        protected String computerPointAudio = @"Audio\Woops that missed";
        protected String playerWinAudio = @"Audio\And there goes the battle";
        protected String computerWinAudio = @"Audio\And there goes the battle";

        //ensures it is not played more than once
        protected bool isPlayingWinSoundEffect = false;
        #endregion
       
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ball = new Ball(this);
            paddleHuman = new PaddleHuman(this);
            paddleComputer = new PaddleComputer(this);

            Components.Add(ball);
            Components.Add(paddleHuman);
            Components.Add(paddleComputer);

            // Call Window_ClientSizeChanged when screen size is changed
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Move paddle back onto screen if it's off
            paddleHuman.Y = GraphicsDevice.Viewport.Height - paddleHuman.Height;
            if (paddleHuman.X + paddleHuman.Width > GraphicsDevice.Viewport.Width)
                paddleHuman.X = GraphicsDevice.Viewport.Width - paddleHuman.Width;
        }

        /// <summary>
        /// Calls initilizeGame() to complete work to enable a restart of the game later on
        /// </summary>
        protected override void Initialize()
        {
            initilizeGame();
        }
        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected void initilizeGame() 
        {
            // Make mouse visible
            IsMouseVisible = true;

            // Set the window's title bar
            Window.Title = "Pong!";

            graphics.ApplyChanges();

            //reset the score!!
            playerScore = 0;
            computerScore = 0;

            // Don't allow ball to move just yet
            ball.Enabled = false;
            isPlayingWinSoundEffect = false;
            creditScreen = false;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //load background
            background = Content.Load<Texture2D>(backgroundImage);
            //set rectangle
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            playerSoundEffect = Content.Load<SoundEffect>(playerAudio);
            computerSoundEffect = Content.Load<SoundEffect>(computerAudio);
            playerPointSoundEffect = Content.Load<SoundEffect>(playerPointAudio);
            computerPointSoundEffect = Content.Load<SoundEffect>(computerPointAudio);
            playerWinSoundEffect = Content.Load<SoundEffect>(playerWinAudio);
            computerWinSoundEffect = Content.Load<SoundEffect>(computerWinAudio);
            font = Content.Load<SpriteFont>("myFont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Get the keyboard state when updating
            KeyboardState newKeyState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Back))
                this.Exit();

            // Press F to toggle full-screen mode
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !oldKeyState.IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                paddleComputer.X = GraphicsDevice.Viewport.Width - paddleComputer.Width - 2;

                mainFrame.Height = GraphicsDevice.Viewport.Height;
                mainFrame.Width = GraphicsDevice.Viewport.Width;
            }

            // Shows game credit screen (pause)
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !oldKeyState.IsKeyDown(Keys.Escape))
            {
                creditScreen = !creditScreen;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && creditScreen)
            {
                ball.Enabled = false;
            }
            else
            {
                // Allows for the game to end and 'winner screen' to be visible
                if (Keyboard.GetState().IsKeyDown(Keys.N) && !oldKeyState.IsKeyDown(Keys.N) 
                    && (playerScore == WIN_SCORE || computerScore == WIN_SCORE))
                {
                    initilizeGame(); // do this if there is a win and the key N is pressed
                }
                else if (!Keyboard.GetState().IsKeyDown(Keys.N) && (playerScore == WIN_SCORE || computerScore == WIN_SCORE))
                {
                    if (playerScore == WIN_SCORE && !isPlayingWinSoundEffect)
                    {
                        isPlayingWinSoundEffect = true;
                        playerWinSoundEffect.Play();
                    }
                    else if (computerScore == WIN_SCORE && !isPlayingWinSoundEffect)
                    {
                        isPlayingWinSoundEffect = true;
                        computerWinSoundEffect.Play();
                    }
                    ball.Enabled = false;
                }
                else
                {

                    // Wait until a second has passed before animating ball 
                    delayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (delayTimer > 1)
                        ball.Enabled = true;

                    int maxX = GraphicsDevice.Viewport.Width - ball.Width;
                    int maxY = GraphicsDevice.Viewport.Height - ball.Height;

                    // Check for bounce. Make sure to place ball back inside the screen
                    // or it could remain outside the screen on the next iteration and cause
                    // a back-and-forth bouncing logic error.
                    if (ball.Y > maxY)
                    {
                        ball.ChangeVertDirection();
                        ball.Y = maxY;
                    }

                    if (ball.Y < 0)
                    {
                        ball.ChangeVertDirection();
                        ball.Y = 0;
                    }
                    else if (ball.X > maxX)
                    {
                        // Game over - reset ball
                        playerPointSoundEffect.Play();
                        playerScore += 1;
                        ball.Reset();

                        // Reset timer and stop ball's Update() from executing
                        delayTimer = 0;
                        ball.Enabled = false;
                    }

                    else if (ball.X < 0)
                    {
                        // Game over - reset ball
                        computerPointSoundEffect.Play();
                        computerScore += 1;
                        ball.Reset();

                        // Reset timer and stop ball's Update() from executing
                        delayTimer = 0;
                        ball.Enabled = false;
                    }

                    // Collision?  Check rectangle intersection between ball and hand
                    if (ball.Boundary.Intersects(paddleHuman.Boundary) && ball.SpeedX < 0)      //changed [ball.SpeedY >] to [ball.SpeedX <]
                    {
                        playerSoundEffect.Play();

                        // If hitting the side of the paddle the ball is coming toward, 
                        // switch the ball's horz direction
                        float ballMiddle = (ball.X + ball.Width) / 2;
                        float paddleMiddle = (paddleHuman.X + paddleHuman.Width) / 2;
                        if ((ballMiddle < paddleMiddle && ball.SpeedX > 0) ||
                            (ballMiddle > paddleMiddle && ball.SpeedX < 0))
                        {
                            ball.ChangeHorzDirection();
                        }

                        // Go back up the screen and speed up
                        //ball.ChangeVertDirection();
                        ball.SpeedUp();
                    }

                    if (ball.Boundary.Intersects(paddleComputer.Boundary) && ball.SpeedX > 0)      //changed [ball.SpeedY <] to [ball.SpeedX >]
                    {
                        computerSoundEffect.Play();

                        // If hitting the side of the paddle the ball is coming toward, 
                        // switch the ball's horz direction
                        float ballMiddle = (ball.X + ball.Width) / 2;
                        float paddleMiddle = (paddleComputer.X + paddleComputer.Width) / 2;
                        if ((ballMiddle < paddleMiddle && ball.SpeedX > 0) ||
                            (ballMiddle > paddleMiddle && ball.SpeedX < 0))
                        {
                            ball.ChangeHorzDirection();
                        }

                        // Go back up the screen and speed up
                        //ball.ChangeVertDirection();
                        ball.SpeedUp();
                    }
                }
            }
            //update oldKeyState
            oldKeyState = newKeyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(background, mainFrame, Color.White);
            DrawText();
            spriteBatch.End();

            base.Draw(gameTime);
        }
       
        /// <summary>
        /// This is called when the game should draw the text
        /// </summary>
        private void DrawText()
        {
            spriteBatch.DrawString(font, playerScore.ToString(), new Vector2(20, 20), Color.Black);
            spriteBatch.DrawString(font, computerScore.ToString(), new Vector2(GraphicsDevice.Viewport.Width - 30, 20), Color.Black);

            if (playerScore == WIN_SCORE)
            {
                spriteBatch.DrawString(font, winnerMsg, new Vector2(GraphicsDevice.Viewport.Width / 2 + winnerMsg.Length
                    , GraphicsDevice.Viewport.Height / 2), Color.Black);
            }
            else if (computerScore == WIN_SCORE)
            {
                spriteBatch.DrawString(font, loserMsg, new Vector2(GraphicsDevice.Viewport.Width / 2 + loserMsg.Length
                    , GraphicsDevice.Viewport.Height / 2), Color.Black);
            }

            if (creditScreen)
            {
                spriteBatch.DrawString(font, creditMsg, new Vector2(GraphicsDevice.Viewport.Width / 2 - creditMsg.Length
                    , GraphicsDevice.Viewport.Height / 2), Color.Blue);
            }
        }
    }
}
