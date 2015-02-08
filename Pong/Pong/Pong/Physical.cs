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


//used resources from http://stackoverflow.com/questions/19189322/proper-sphere-collision-resolution-with-different-sizes-and-mass-using-xna-monog

namespace Pong
{
    public class Physical : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region  Private Members
        
        //sprite handling
        protected SpriteBatch spriteBatch;
        protected ContentManager contentManager;
        protected Texture2D sprite;
        
        //size, movement, collision handling
        private Vector3 size;  //x = width, y = height, z = radius
        private Vector3 position; // z should not be used in this 2D version of Pong
        private Vector3 velocity;  //x = horizontal speed, y = vertical speed
        protected float mass;

        protected bool isObstacle = false;

        protected bool isBall = false;

        protected bool isColliding = false;

        // Default speed of ball
        private const float DEFAULT_SPEED = 150;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 30;

        #endregion

        #region Properties
       
        #region Size
        /// <summary>
        /// Gets or sets the object's size.
        /// </summary>
        public Vector3 Size
        {
            get{return size;}
            set{size = value;}
        }
        /// <summary>
        /// Gets or sets the object's width (Size.X).
        /// </summary>
        public float SizeX
        {
            get {return Size.X;}
            set{size.X = value;}
        }
        /// <summary>
        /// Gets or sets the object's height (Size.Y).
        /// </summary>
        public float SizeY
        {
            get{return Size.Y;}
            set{size.Y = value;}
        }
        /// <summary>
        /// Gets or sets the object's radius (Size.Z).
        /// </summary>
        public float SizeR
        {
            get{return Size.Z;}
            set{size.Z = value;}
        }
        #endregion

        #region Position
        /// <summary>
        /// Get or set the object's position as a Vector3.
        /// </summary>
        public Vector3 Position
        {
            get{return position;}
            set{position = value;}
        }
        /// <summary>
        /// Get or set the object's X position as a float.
        /// </summary>
        public float PositionX
        {
            get{return position.X;}
            set{position.X = value;}
        }
        /// <summary>
        /// Get or set the object's Y position as a float.
        /// </summary>
        public float PositionY
        {
            get{return position.Y;}
            set{position.Y = value;}
        }
        /// <summary>
        /// Get or set the object's Z position as a float. Why would you want it though?
        /// </summary>
        public float PositionZ
        {
            get{return position.Z;}
            set{position.Z = value;}
        }
        /// <summary>
        /// Get or set the object's X and Y positions as a Vector2.
        /// </summary>
        public Vector2 Position_v2
        {
            get{return new Vector2(position.X, position.Y);}
            set{   
                PositionX = value.X;
                PositionY = value.Y;
            }
        }
        #endregion

        #region PositionCentered
        /// <summary>
        /// Returns the object's center as a Vector3.
        /// </summary>
        public Vector3 PositionCentered
        {
            get { return new Vector3(PositionCenteredX, PositionCenteredY, PositionZ); }
        }
        /// <summary>
        /// Returns the object's center as a Vector3.
        /// </summary>
        public float PositionCenteredX
        {
            get { return PositionX + (SizeX / 2); }
            set { PositionX = value; }
        }
        /// <summary>
        /// Returns the object's center as a Vector3.
        /// </summary>
        public float PositionCenteredY
        {
            get { return PositionY + (SizeY / 2); }
            set { PositionY = value; }
        }
        #endregion

        #region Velocity
        /// <summary>
        /// Get or set the object's velocity as a Vector3.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        /// <summary>
        /// Get or set the object's X velocity as a float.
        /// </summary>
        public float VelocityX
        {
            get { return velocity.X; }
            set { velocity.X = value; }
        }
        /// <summary>
        /// Get or set the object's Y velocity as a float.
        /// </summary>
        public float VelocityY
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }
        /// <summary>
        /// Get or set the object's Z velocity as a float.
        /// </summary>
        public float VelocityZ
        {
            get { return velocity.Z; }
            set { velocity.Z = value; }
        }
        /// <summary>
        /// Get or set the object's velocity in the X and Y directions as a Vector2.
        /// </summary>
        public Vector2 Velocity_v2
        {
            get { return new Vector2(position.X, position.Y); }
            set { Vector2 newVelocity_v2 = new Vector2(value.X, value.Y); }
        }
        #endregion

        #region Boundary
        /// <summary>
        /// Returns a BoundingSphere centered on the object.
        /// </summary>
        public BoundingSphere Boundary
        {
            get { return new BoundingSphere(PositionCentered, SizeR); }
        }
        /// <summary>
        /// Returns a Rectangle centered on the object.
        /// </summary>
        public Rectangle Boundary_Rect
        {
            get { return new Rectangle((int)PositionX, (int)PositionY, (int)SizeX, (int)SizeY); }
        }
        #endregion
        
        public bool IsColliding
        {
            get { return isColliding; }
            set { isColliding = value; }
        }
        
        public bool IsBall
        {
            get { return isBall; }
            set { isBall = value; }
        }
       
        public bool IsObstacle
        {
            get { return isObstacle; }
            set { isObstacle = value; }
        }

        public float Mass
        {
            get{return mass;}
            set{mass = value;}
        }

        #endregion

        public Physical(Game game)
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
        }

        /// <summary>
        /// This function is called when the 'R' key is pressed during gameplay
        /// </summary>
        public void Reset()
        {
            isColliding = false;

            PositionY = (GraphicsDevice.Viewport.Height - SizeY) / 2;
            PositionX = (GraphicsDevice.Viewport.Width - SizeX) / 2;

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
        /// Move the sprite(the image, not by location) by speed, scaled by elapsed time.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="sheetSize"></param>
        /// <param name="currentFrame"></param>
        /// <param name="timeSinceLastFrame"></param>
        /// <param name="millisecondsPerFrame"></param>
        protected void UpdateSpriteImage(GameTime gameTime, Point sheetSize, 
            ref Point currentFrame, ref int timeSinceLastFrame, int millisecondsPerFrame)
        {
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
        }

        /// <summary>
        /// Increase the ball's speed in the X and Y directions.
        /// </summary>
        public void SpeedUp()
        {
            if (!IsObstacle)
            {
                if (VelocityY < 0)
                {
                    VelocityY -= INCREASE_SPEED;
                }
                else
                {
                    VelocityY += INCREASE_SPEED;
                }

                if (VelocityX < 0)
                {
                    VelocityX -= INCREASE_SPEED;
                }
                else
                {
                    VelocityX += INCREASE_SPEED;
                }
            }
        }

        virtual public void checkCollision(Physical opponent)
        {
            if (!(this == opponent) && Boundary.Intersects(opponent.Boundary))
            {
                resolveCollision(opponent);
            }
        }
        
        public void resolveCollision(Physical opponent)
        {
            //find the normal of the two position vectors and calculate the plane of collision
            Vector3 collisionNormal = PositionCentered - opponent.PositionCentered;
            collisionNormal.Normalize();
            Vector3 collisionDirection = new Vector3(-collisionNormal.Y, collisionNormal.X, 0);

            Vector3 v1Parallel = Vector3.Dot(collisionNormal, Velocity) * collisionNormal;
            Vector3 v1Ortho = Vector3.Dot(collisionDirection, Velocity) * collisionDirection;
            Vector3 v2Parallel = Vector3.Dot(collisionNormal, opponent.Velocity) * collisionNormal;
            Vector3 v2Ortho = Vector3.Dot(collisionDirection, opponent.Velocity) * collisionDirection;

            if (Vector3.Dot(collisionNormal, Velocity) > 0 || Vector3.Dot(collisionNormal, opponent.Velocity) > 0)
            {

                float v1Length = v1Parallel.Length();
                float v2Length = v2Parallel.Length();
                float commonVelocity = 2 * (this.Mass * v1Length + opponent.Mass * v2Length) / (this.Mass + opponent.Mass);
                float v1LengthAfterCollision = commonVelocity - v1Length;
                float v2LengthAfterCollision = commonVelocity - v2Length;
                v1Parallel = v1Parallel * (v1LengthAfterCollision / v1Length);
                v2Parallel = v2Parallel * (v2LengthAfterCollision / v2Length);

                this.Velocity = v1Parallel + v1Ortho;
                opponent.Velocity = v2Parallel + v2Ortho;
            }

        }
    }
}
