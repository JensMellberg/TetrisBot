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
    public class Block : GameObj
    
    {
       
       public Color color;

        public Block(Vector2 pos, Color color, Game1 game)
            : base(pos, 20,20, game)
        {
            
            this.color = color;
            
        }
        public Rectangle[] getArea()
        {
            return new Rectangle[] { new Rectangle((int)position.X, (int)position.Y, 20, 20) };
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
        }
        public virtual bool checkForCollision(Vector2 dist, List<GameObj> gameList)
        {
            Rectangle myRect = getArea()[0];
            myRect.X += (int)dist.X;
            myRect.Y += (int)dist.Y;
            if (myRect.X < game.boardPosition.X || myRect.X >= game.boardPosition.X +game.Board.size.width || myRect.Y >= game.boardPosition.Y +game.Board.size.height)
                return true;
            foreach (GameObj b in gameList)
                if (b != this && b is Block &&!(b is Piece))
                foreach (Rectangle r in ((Block)b).getArea())
                    if (myRect.Intersects(r))
                    {
                       //System.Windows.Forms.MessageBox.Show(b.position.X + ":" + b.position.Y+" "+b.GetType());
                        return true;
                        
                    }
            return false;
        }
        public virtual Block copy()
        {
            return new Block(position, color, game);
        }
        public virtual void destroy()
        {

        }

    }
}
