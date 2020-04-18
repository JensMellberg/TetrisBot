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
    public class ShadowPiece
    {
        public Block[] children = new Block[4];
        public ShadowPiece(Vector2[] positions,Game1 game)
        {
           for (int i = 0; i < positions.Length; i++)
               children[i] = new Block(positions[i],Color.Black,game);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Block b in children)
                b.Draw(spriteBatch);
        }
    }
}
