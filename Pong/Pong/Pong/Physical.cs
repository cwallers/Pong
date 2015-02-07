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
        private Vector3 position;
        private Vector3 velocity;  //x = horizontal speed, y = vertical speed
        protected float mass;

        #endregion

        #region Properties

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
            get{return Position.X;}
            set{position.X = value;}
        }
        /// <summary>
        /// Get or set the object's Y position as a float.
        /// </summary>
        public float PositionY
        {
            get{return Position.Y;}
            set{position.Y = value;}
        }
        /// <summary>
        /// Get or set the object's Z position as a float.
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
        /// <summary>
        /// Get or set the object's velocity as a Vector3.
        /// </summary>
        public Vector3 Velocity
        {
            get{return velocity;}
            set{velocity = value;}
        }
        /// <summary>
        /// Get or set the object's X position as a float.
        /// </summary>
        public float VelocityX
        {
            get{return Velocity.X;}
            set{velocity.X = value;}
        }
        /// <summary>
        /// Get or set the object's Y Velocity as a float.
        /// </summary>
        public float VelocityY
        {
            get{return Velocity.Y;}
            set{velocity.Y = value;}
        }
        /// <summary>
        /// Get or set the object's Z Velocity as a float.
        /// </summary>
        public float VelocityZ
        {
            get{return Velocity.Z;}
            set{velocity.Z = value;}
        }
        /// <summary>
        /// Get or set the object's velocity in the X and Y directions as a Vector2.
        /// </summary>
        public Vector2 Velocity_v2
        {
            get{return new Vector2(position.X, position.Y);}
            set{Vector2 newVelocity_v2 = new Vector2(value.X, value.Y);}
        }
        /// <summary>
        /// Get or set the object's mass.
        /// </summary>
        public float Mass
        {
            get{return mass;}
            set{mass = value;}
        }
        /// <summary>
        /// Returns a BoundingSphere centered on the object.
        /// </summary>
        public BoundingSphere Boundary
        {
            get {return new BoundingSphere(Position, SizeR);}
        }
        /// <summary>
        /// Returns a Rectangle centered on the object.
        /// </summary>
        public Rectangle Boundary_Rect
        {
            get{return new Rectangle((int)PositionX, (int)PositionY, (int)size.X, (int)size.Y);}
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

        public void resolveCollision(Physical opponent)
        {   
            //find the normal of the two position vectors and calculate the plane of collision
            Vector3 collisionNormal = position - opponent.Position;
            collisionNormal.Normalize();
            Vector3 collisionDirection = new Vector3(-collisionNormal.Y, collisionNormal.X, 0);

            //split this object's velocity along the plane of collision and the normal
            Vector3 v1Parallel = Vector3.Dot(collisionNormal, velocity) * collisionNormal;
            Vector3 v1Ortho = Vector3.Dot(collisionDirection, velocity) * collisionDirection;

            //split the opposing object's velocity along the plane of collision and the normal
            Vector3 v2Parallel = Vector3.Dot(collisionNormal, opponent.Velocity) * collisionNormal;
            Vector3 v2Ortho = Vector3.Dot(collisionDirection, opponent.Velocity) * collisionDirection;

            //float v1Length = v1Parallel.Length();
            //float v2Length = v2Parallel.Length();

            //float commonVelocity = 2 * (mass * v1Length + opponent.Mass * v2Length) / (mass + opponent.Mass);

            //float v1LengthAfterCollision = commonVelocity - v1Length;
            //float v2LengthAfterCollision = commonVelocity - v2Length;
            //v1Parallel = v1Parallel * (v1LengthAfterCollision / v1Length);
            //v1Parallel = v2Parallel * (v2LengthAfterCollision / v2Length);

            if (Vector3.Dot(collisionNormal, velocity) > 0 || Vector3.Dot(collisionNormal, opponent.Velocity) < 0)
            {
                float totalMass = mass + opponent.Mass;

                Vector3 v1ParallelNew = (v1Parallel * (mass - opponent.Mass) + 2 * opponent.Mass * v2Parallel) / totalMass;
                Vector3 v2ParallelNew = (v2Parallel * (opponent.Mass - mass) + 2 * mass * v1Parallel) / totalMass;

                v1Parallel = v1ParallelNew;
                v2Parallel = v2ParallelNew;

                velocity = v1Parallel + v1Ortho;
                opponent.Velocity = v2Parallel + v2Ortho;
            } 
        }
    }
}
