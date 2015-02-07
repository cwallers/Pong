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
    public class Obstacle : Microsoft.Xna.Framework.DrawableGameComponent
    {

        #region Private Members
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;

        // Default speed of obstacle
        private const float DEFAULT_SPEED = 0;

        // Increase in speed each hit
        private const float INCREASE_SPEED = 50;

        // Ball location
        Vector3 ballPosition;

        // Ball's motion
        Vector2 ballSpeed = new Vector2(DEFAULT_SPEED, DEFAULT_SPEED);

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
        
        #endregion

        public Obstacle(Game game)
            : base(game)
        {
            contentManager = new ContentManager(game.Services);
        }
    }
}
