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
    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector2 scorePos = new Vector2(20, 10);

        public SpriteFont Font { get; set; }

        public int personScore { get; set; }

        public int compScore { get; set; }

        public HUD(Game game) : base(game)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Score in the top-left of screen
            spriteBatch.DrawString(
                Font,                          // SpriteFont
                "Score: " + personScore.ToString(),  // Text
                scorePos,                      // Position
                Color.White);                  // Tint
        }
    }
}
