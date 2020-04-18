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
    public class GrayBlock : Block
    {
        public GrayBlock(Vector2 pos,Game1 game) : base(pos,Color.Gray,game)
        {

        }
        public override void destroy()
        {
            if (!game.blockLinesD.Contains((int)position.Y))
            game.blockLinesD.Add((int)position.Y);
        }
    }
}
