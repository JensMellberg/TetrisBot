using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TetrisBot
{
    public class GameObj
    {
       public Game1 game;
        public Vector2 position;
        public Size size;
        public struct Size
        {
            public int width;
            public int height;
        }
        public GameObj(Vector2 pos, int w, int h, Game1 game)
        {
            size = new Size
            {
                width = w,
                height = h,
            };

            position = pos;
            this.game = game;
        }
        public virtual void Update(List<GameObj> gameList)
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
        }

    }
}
