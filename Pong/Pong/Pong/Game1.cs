/*
 * Pokemon Pong with obstacle ball
 * by Catherine Wallers and Nathan Roberts, Harding University
 * Game Development, Spring 2015
 * Base code by Frank McCown, Harding University
 * 
 * Sounds: 
 * Stadium Announcer Voice: http://www.soundboard.com/sb/pokemonstadium
 * Pokemon Voice: http://www.soundboard.com/
 * 
 * Images:
 * Pokemon Balls: http://imgarcade.com/1/pokeball-sprite-sheet/
 * Pikachu: http://www.smwcentral.net/?p=profile&id=21382
 * Squirtle: http://www.fanpop.com/clubs/squirtle/images/1054498/title/squirtle-fanart
 * Background: http://www.wallpapervortex.com/wallpaper-35547_pokemon.html#.VNfJMvnF-So
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
        private Obstacle obstacle;
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
        private string creditMsg = "Catherine Wallers & Nathan Roberts \n\n\n\n\n\n\n\n\n Press 'E' to play again";
        private KeyboardState oldKeyState; 

        //background 
        Texture2D background;
        Texture2D pauseScreen;
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
        protected String pauseScreenImage = @"Images\PokemonPause";
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
            obstacle = new Obstacle(this);

            Components.Add(ball);
            Components.Add(paddleHuman);
            Components.Add(paddleComputer);
            Components.Add(obstacle);

            // Call Window_ClientSizeChanged when screen size is changed
            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            // Move paddle back onto screen if it's off
            paddleHuman.PositionY = GraphicsDevice.Viewport.Height - paddleHuman.SizeY;
            if (paddleHuman.PositionX + paddleHuman.SizeX > GraphicsDevice.Viewport.Width)
            {
                paddleHuman.PositionX = GraphicsDevice.Viewport.Width - paddleHuman.SizeX;
            }
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
            Window.Title = "Pokemon Pong!";

            graphics.ApplyChanges();

            // Don't allow ball or obstacle to move just yet
            ball.Enabled = false;
            obstacle.Enabled = false;

            //reset score and boolians
            playerScore = 0;
            computerScore = 0;
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

            //load background and pauseScreen
            background = Content.Load<Texture2D>(backgroundImage);
            pauseScreen = Content.Load<Texture2D>(pauseScreenImage);

            //set rectangle
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Load sound effects from the audio folder
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

            #region Reset 'R'
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !oldKeyState.IsKeyDown(Keys.R))
            {
                ball.Reset();
            }
            #endregion 

            #region Exit Game 'Backspace'
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Back))
                this.Exit();
            #endregion 

            #region Full Screen 'F'
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !oldKeyState.IsKeyDown(Keys.F))
            {
                graphics.IsFullScreen = !graphics.IsFullScreen;
                graphics.ApplyChanges();
                paddleComputer.PositionX = GraphicsDevice.Viewport.Width - paddleComputer.SizeX - 2;

                mainFrame.Height = GraphicsDevice.Viewport.Height;
                mainFrame.Width = GraphicsDevice.Viewport.Width;
            }
            #endregion

            #region Credit/Pause Screen 'Esc'
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && !oldKeyState.IsKeyDown(Keys.Escape))
            {
                creditScreen = !creditScreen;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.Escape) && creditScreen)
            {
                ball.Enabled = false;
                obstacle.Enabled = false;
                paddleComputer.Enabled = false;
                paddleHuman.Enabled = false;
            }
            #endregion 
            else
            {
                #region Winner Screen and New Game 'N'
                if (Keyboard.GetState().IsKeyDown(Keys.N) && !oldKeyState.IsKeyDown(Keys.N) 
                    && (playerScore == WIN_SCORE || computerScore == WIN_SCORE))
                {
                    initilizeGame();
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
                #endregion
                else
                {
                    // call collision detection on the ball and the obstacle
                    Collision(ball, gameTime);
                    Collision(obstacle,gameTime);
                }
            }
            
            oldKeyState = newKeyState; //update oldKeyState
            base.Update(gameTime);
        }

        protected void Collision(Physical physicalObject, GameTime gameTime)
        {
             // Wait until a second has passed before animating thing 
            delayTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delayTimer > 1)
            {
                ball.Enabled = true;
                obstacle.Enabled = true;
                paddleHuman.Enabled = true;
                paddleComputer.Enabled = true;
            }

            int maxX = GraphicsDevice.Viewport.Width - (int)physicalObject.SizeX;
            int maxY = GraphicsDevice.Viewport.Height - (int)physicalObject.SizeY;

            #region Collision resolution with walls
            // collision with the bottom and top of the screen
            if (physicalObject.PositionY > maxY)
            {
                physicalObject.VelocityY *= -1;
                physicalObject.PositionY = maxY;
            }
            else if (physicalObject.PositionY < 0)
            {
                physicalObject.VelocityY *= -1;
                physicalObject.PositionY = 0;
            }
                // collision with the left and right of the screen
            else if (physicalObject.PositionX > maxX)
            {
                if (physicalObject.IsObstacle)
                {
                    physicalObject.VelocityX *= -1;
                    physicalObject.PositionX = maxX;
                }
                else if (physicalObject.IsBall)
                {
                    // Game over - reset thing
                    playerPointSoundEffect.Play();
                    playerScore += 1;
                    physicalObject.Reset();

                    // Reset timer and stop physicalObject's Update() from executing
                    delayTimer = 0;
                    physicalObject.Enabled = false;
                }
                
            }

            else if (physicalObject.PositionX < 0)
            {
                if (physicalObject.IsObstacle)
                {
                    physicalObject.VelocityX *= -1;
                    physicalObject.PositionX = 0;
                }
                else if (physicalObject.IsBall)
                {
                    // Game over - reset thing
                    computerPointSoundEffect.Play();
                    computerScore += 1;
                    physicalObject.Reset();
                    // Reset timer and stop physicalObject's Update() from executing
                    delayTimer = 0;
                    physicalObject.Enabled = false;
                }


            }
            #endregion 

            #region Collision resolution with other Physical Objects
            if (physicalObject.IsColliding)
            {
                if (!((physicalObject != ball           && physicalObject.Boundary.Intersects(ball.Boundary)) 
                   || (physicalObject != obstacle       && physicalObject.Boundary.Intersects(obstacle.Boundary))
                   || (physicalObject != paddleHuman    && physicalObject.Boundary.Intersects(paddleHuman.Boundary))
                   || (physicalObject != paddleComputer && physicalObject.Boundary.Intersects(paddleComputer.Boundary))))
                {
                    physicalObject.IsColliding = false;
                }
            }
            else
            {
                #region intersection with a paddleHuman
                if ((physicalObject != paddleHuman && physicalObject.Boundary.Intersects(paddleHuman.Boundary)))
                {
                    physicalObject.IsColliding = true;
                    if (physicalObject.IsBall)
                    {
                        playerSoundEffect.Play();
                    }

                    Vector2 A = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    Vector2 B = new Vector2(paddleHuman.PositionCenteredX, paddleHuman.PositionCenteredY);
                    Vector2 C = A - B;
                    C.Normalize();
                    Vector2 D = new Vector2(physicalObject.VelocityX, physicalObject.VelocityY);
                    Vector2.Reflect(D, C);
                    physicalObject.VelocityX = -D.X;
                    physicalObject.VelocityY = D.Y;
                    physicalObject.SpeedUp();
                }
                #endregion 
                
                #region intersection with paddleComputer
                else if (physicalObject != paddleComputer && physicalObject.Boundary.Intersects(paddleComputer.Boundary))
                {
                    physicalObject.IsColliding = true;
                    if (physicalObject.IsBall)
                    {
                        computerSoundEffect.Play();
                    }

                    Vector2 A = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    Vector2 B = new Vector2(paddleComputer.PositionCenteredX, paddleComputer.PositionCenteredY);
                    Vector2 C = A - B;
                    C.Normalize();
                    Vector2 D = new Vector2(physicalObject.VelocityX, physicalObject.VelocityY);
                    Vector2.Reflect(D, C);
                    physicalObject.VelocityX = -D.X;
                    physicalObject.VelocityY = D.Y;
                    physicalObject.SpeedUp();
                }
                #endregion 
                
                #region intersection with an obstacle
                else if (physicalObject != obstacle && physicalObject.Boundary.Intersects(obstacle.Boundary)) 
                {
                    Vector2 A = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    Vector2 B = new Vector2(obstacle.PositionCenteredX, obstacle.PositionCenteredY);
                    Vector2 C = A - B;
                    C.Normalize();
                    Vector2 D = new Vector2(physicalObject.VelocityX, physicalObject.VelocityY);
                    Vector2.Reflect(D, C);

                    A = new Vector2(obstacle.PositionCenteredX, obstacle.PositionCenteredY);
                    B = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    C = A - B;
                    C.Normalize();
                    Vector2 E = new Vector2(obstacle.VelocityX, obstacle.VelocityY);
                    
                    Vector2.Reflect(E, C);

                    float direction;

                    if (Math.Sign(physicalObject.VelocityX) == Math.Sign(D.X) 
                     && Math.Sign(obstacle.VelocityX) == Math.Sign(E.X)
                     && Math.Sign(D.X) == Math.Sign(E.X))
                    {
                        direction = D.X;
                        D.X = E.X;
                        E.X = direction;
                    }
                    if (Math.Sign(physicalObject.VelocityY) == Math.Sign(D.Y)
                     && Math.Sign(obstacle.VelocityY) == Math.Sign(E.Y)
                     && Math.Sign(D.Y) == Math.Sign(E.Y))
                    {
                        direction = D.Y;
                        D.Y = E.Y;
                        E.Y = direction;
                    }

                    physicalObject.VelocityX = E.X;
                    physicalObject.VelocityY = E.Y;

                    obstacle.VelocityX = D.X;
                    obstacle.VelocityY = D.Y;

                    ball.IsColliding = true;
                    obstacle.IsColliding = true;

                }
                #endregion

                #region intersection with a ball
                else if (physicalObject != ball && physicalObject.Boundary.Intersects(ball.Boundary))
                {
                    Vector2 A = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    Vector2 B = new Vector2(ball.PositionCenteredX, ball.PositionCenteredY);
                    Vector2 C = A - B;
                    C.Normalize();
                    Vector2 D = new Vector2(physicalObject.VelocityX, physicalObject.VelocityY);
                    Vector2.Reflect(D, C);

                    A = new Vector2(ball.PositionCenteredX, ball.PositionCenteredY);
                    B = new Vector2(physicalObject.PositionCenteredX, physicalObject.PositionCenteredY);
                    C = A - B;
                    C.Normalize();
                    Vector2 E = new Vector2(ball.VelocityX, ball.VelocityY);

                    Vector2.Reflect(E, C);

                    physicalObject.VelocityX = E.X;
                    physicalObject.VelocityY = E.Y;

                    ball.VelocityX = D.X;
                    ball.VelocityY = D.Y;

                    ball.IsColliding = true;
                    obstacle.IsColliding = true;
                }
                #endregion 
                
                else
                {
                    physicalObject.IsColliding = false;
                }
            }
            #endregion 
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            if (!creditScreen)
            {
                spriteBatch.Draw(background, mainFrame, Color.White);
            }
            else if (creditScreen)
            {
                spriteBatch.Draw(pauseScreen, mainFrame, Color.White);
            }

            DrawText(); // draw text after the images so that it is in the foreground
            spriteBatch.End();

            base.Draw(gameTime);
        }
       
        /// <summary>
        /// This is called when the game should draw the text
        /// </summary>
        private void DrawText()
        {
            // draw the score to the upperhand corners
            spriteBatch.DrawString(font, playerScore.ToString(), new Vector2(20, 20), Color.Black);
            spriteBatch.DrawString(font, computerScore.ToString(), new Vector2(GraphicsDevice.Viewport.Width - 30, 20), Color.Black);

            // draw the win messages when applicable
            if (playerScore == WIN_SCORE && !creditScreen)
            {
                spriteBatch.DrawString(font, winnerMsg, new Vector2((GraphicsDevice.Viewport.Width / 3) - winnerMsg.Length/2
                    , GraphicsDevice.Viewport.Height / 14), Color.Yellow);
            }
            else if (computerScore == WIN_SCORE && !creditScreen)
            {
                spriteBatch.DrawString(font, loserMsg, new Vector2((GraphicsDevice.Viewport.Width / 3) - loserMsg.Length/2
                    , GraphicsDevice.Viewport.Height / 14), Color.Yellow);
            }

            // sraw the credit/pause screen info
            if (creditScreen)
            {
                spriteBatch.DrawString(font, creditMsg, new Vector2(GraphicsDevice.Viewport.Width / 4 - creditMsg.Length
                    , GraphicsDevice.Viewport.Height / 5), Color.Black);
            }
        }
    }
}
