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
    class SpriteManager
    {
        #region SpriteManager Properties
        protected Texture2D texture;
        public Vector2 position = Vector2.Zero;
        public Color color = Color.White;
        public Vector2 origin;
        public float rotation = 0f;
        public float scale = 1f;
        public SpriteEffects spriteEffect;
        protected Rectangle[] rectangles;
        protected int frameIndex = 0;

        private float timeElapsed;
        public bool isLooping = false;

        //default to 20 frames per second
        private float timeToUpdate = 0.05f;
        #endregion

        #region Sprite Methods
        public int framesPerSecond
        {
            set { timeToUpdate = (1f / value); }
        }
        #endregion

        public SpriteManager(Game game, int frames)
        {
            loadContent(game.Content.Load<Texture2D>(@"Images\pballss"), frames);
            position = new Vector2(100, 100);
            isLooping = true;
            framesPerSecond = 9;
        }

        public void loadContent(Texture2D texture, int frames)
        {
            this.texture = texture;
            int width = texture.Width / frames;
            rectangles = new Rectangle[frames];
            for (int i = 0; i < frames; i++)
            {
                rectangles[i] = new Rectangle(i * width, 0, width, texture.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, rectangles[frameIndex],
                color, rotation, origin, scale, spriteEffect, 0f);
        }

        public void Update(GameTime gameTime)
        {
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeElapsed > timeToUpdate)
            {
                timeElapsed -= timeToUpdate;
                if (frameIndex < rectangles.Length - 1)
                {
                    frameIndex++;
                }
                else if (isLooping)
                {
                    frameIndex = 0;
                }
            }
        }
    }
}
