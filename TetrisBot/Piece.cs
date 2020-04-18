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
    public class Piece : Block
    {
        public Block[] children;
        int cooldown = 50;
        Color[] colors = new Color[] { Color.Teal, Color.Red };
        int mode = 0;
        public int maxMode = 1;
        ShadowPiece shadowPiece;
        public int version;
        public Piece(Vector2 pos, int version, Game1 game, List<GameObj> gameList)
            : base(pos, Color.White, game)
        {
            cooldown = 50;
            this.version = version;
            switch (version)
            {
                //LONG piece
                case 1: position += new Vector2(60, 0); children = new Block[] { new Block(position, Color.Teal, game), new Block(position + new Vector2(20, 0), Color.Teal, game), new Block(position + new Vector2(40, 0), Color.Teal, game)
                    , new Block(position + new Vector2(60, 0), Color.Teal, game) };  break;
                //SQUARE
                case 2: position += new Vector2(80, 0); children = new Block[] { new Block(position, Color.Yellow, game), new Block(position + new Vector2(20, 0), Color.Yellow, game), new Block(position + new Vector2(0, 20), Color.Yellow, game)
                    , new Block(position + new Vector2(20, 20), Color.Yellow, game) }; break;
                //RED T shape
                case 3: position += new Vector2(60, 0); children = new Block[] { new Block(position+ new Vector2(0,20), Color.Purple, game), new Block(position + new Vector2(20, 20), Color.Purple, game), new Block(position + new Vector2(40, 20), Color.Purple, game)
                    , new Block(position + new Vector2(20, 0), Color.Purple, game) }; maxMode = 3; break;
                //PINK L shape
                case 4: position += new Vector2(60, 0); children = new Block[] { new Block(position+ new Vector2(0,20), Color.Orange, game), new Block(position + new Vector2(20, 20), Color.Orange, game), new Block(position + new Vector2(40, 20), Color.Orange, game)
                    , new Block(position + new Vector2(40, 0), Color.Orange, game) }; maxMode = 3; break;
                //BLUE L shape
                case 5: position += new Vector2(60, 0); children = new Block[] { new Block(position+ new Vector2(40,20), Color.Blue, game), new Block(position + new Vector2(20, 20), Color.Blue, game), new Block(position + new Vector2(0, 20), Color.Blue, game)
                    , new Block(position + new Vector2(0, 0), Color.Blue, game) }; maxMode = 3; break;
                //TEAL stairs rising right
                case 6: position += new Vector2(60, 0); children = new Block[] { new Block(position+ new Vector2(0,20), Color.Green, game), new Block(position + new Vector2(20, 20), Color.Green, game), new Block(position + new Vector2(20, 0), Color.Green, game)
                    , new Block(position + new Vector2(40, 0), Color.Green, game) }; break;
                //WHITE stairs rising left
                case 7: position += new Vector2(60, 0); children = new Block[] { new Block(position, Color.Red, game), new Block(position + new Vector2(20, 0), Color.Red, game), new Block(position + new Vector2(20, 20), Color.Red, game)
                    , new Block(position + new Vector2(40, 20), Color.Red, game) }; break;
            }

           
        }
        public override Block copy()
        {
            Piece p = new Piece(game.boardPosition, version, game, null);
            return p;
        }
        public int[] minMax(int mode)
        {
            int check = version;
            if (check == 4 || check == 5)
                check = 3;
            if (check == 7)
                check = 6;
            switch (check)
            {
                case 1: if (mode == 0)
                        return new int[] { -3, 3 };
                    else
                        return new int[] { -5, 4 };
                case 2: return new int[] { -4, 4 };
                case 3: if (mode == 0 || mode == 2)
                        return new int[] { -3, 4 };
                    else if (mode == 1)
                        return new int[] { -4, 4 };
                    else return new int[] { -3, 5 };
                case 6: if (mode == 0)
                        return new int[] { -3, 4 };
                    else
                        return new int[] { -4, 4 };
            }
            return null;
        }
        void changeMode()
        {
            mode++;
            if (mode > maxMode)
                mode = 0;
        }

        public void TurnIfPossible(List<GameObj> gameList,bool alwaysTurn)
        {
            Vector2[] dist = new Vector2[4];
            for (int i = 0; i < 4; i++)
                dist[i] = Vector2.Zero;
            switch (version)
            {
                case 1: int multiplier = mode == 0 ? 1 : -1; dist[0] = new Vector2(40, -40) * multiplier; dist[1] = new Vector2(20,-20)*multiplier; dist[2] = Vector2.Zero;
                    dist[3] = new Vector2(-20, 20) * multiplier; break;
                case 2: return;
                case 3: switch (mode)
                    {
                        case 0: dist[0] = new Vector2(20, 20); break;
                        case 1: dist[3] = new Vector2(-20, 20); break;
                        case 2: dist[2] = new Vector2(-20, -20); break;
                        case 3: dist[0] = new Vector2(-20, -20); dist[3] = new Vector2(20, -20); dist[2] = new Vector2(20, 20); break;
                    } break;
                case 4: switch (mode)
                    {
                        case 0: dist[0] = new Vector2(20, -20); dist[2] = new Vector2(-20, 20);dist[3] = new Vector2(0, 40); break;
                        case 1: dist[0] = new Vector2(20, 20); dist[2] = new Vector2(-20, -20); dist[3] = new Vector2(-40, 0); break;
                        case 2: dist[0] = new Vector2(-20, 20); dist[2] = new Vector2(20, -20); dist[3] = new Vector2(0, -40); break;
                        case 3: dist[0] = new Vector2(-20, -20); dist[2] = new Vector2(20, 20); dist[3] = new Vector2(40, 0); break;
                    } break;
                case 5: switch (mode)
                    {
                        case 0: dist[0] = new Vector2(-20, 20); dist[2] = new Vector2(20, -20); dist[3] = new Vector2(40, 0); break;
                        case 1: dist[0] = new Vector2(-20, -20); dist[2] = new Vector2(20, 20); dist[3] = new Vector2(0, 40); break;
                        case 2: dist[0] = new Vector2(20, -20); dist[2] = new Vector2(-20, 20); dist[3] = new Vector2(-40, 0); break;
                        case 3: dist[0] = new Vector2(20, 20); dist[2] = new Vector2(-20, -20); dist[3] = new Vector2(0, -40); break;
                    } break;
                case 6: multiplier = mode == 0 ? 1 : -1; dist[0] = new Vector2(20, -40)*multiplier; dist[1] = new Vector2(20, 0) * multiplier; break;
                case 7: multiplier = mode == 0 ? 1 : -1; dist[0] = new Vector2(40,0) * multiplier; dist[3] = new Vector2(0, -40) * multiplier; break;

              
            }
            if (alwaysTurn)
            {
                for (int i = 0; i < children.Length; i++)
                    children[i].position += dist[i];
                changeMode();
                return;
            }
            if (!checkForCollision(dist, gameList))
            {
                for (int i = 0; i < children.Length; i++)
                    children[i].position += dist[i];
                changeMode();
            }
        }
        public void updateShadow(List<GameObj> gameList)
        {
     
            Vector2 dist = Vector2.Zero;
            while (!checkForCollision(dist,gameList))
                dist+= new Vector2(0,20);
            dist-= new Vector2(0,20);
            Vector2[] positions = new Vector2[4];
            for (int i = 0; i < children.Length; i++)
                positions[i] = children[i].position + dist;
            shadowPiece = new ShadowPiece(positions, game);

        }
        public void dropDown(List<GameObj> gameList)
        {
            for (int i = 0; i < children.Length; i++)
                children[i].position = shadowPiece.children[i].position;
            foreach (Block b in children)
                gameList.Add(b);
            gameList.Remove(this);
        }
        public void checkMovement(List<GameObj> gameList)
        {
           
            cooldown--;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cooldown -= 7;
            if (cooldown < 0)
            {
                if (!moveIfPossible(new Vector2(0, 20), gameList))
                {
                    
                    foreach (Block b in children)
                        gameList.Add(b);
                    gameList.Remove(this);
                    return;
                }
                cooldown += 50;
            }
            updateShadow(gameList);

    }
        public override bool checkForCollision(Vector2 dist, List<GameObj> gameList)
        {
            foreach (Block b in children)
                if (b.checkForCollision(dist, gameList))
                    return true;
            return false;
        }
        public string test = "";
        public bool checkForCollision(Vector2[] dist, List<GameObj> gameList)
        {
            for (int i = 0; i < children.Length; i++)
                if (children[i].checkForCollision(dist[i], gameList)) {
 
                    return true;                  
                    }
            return false;
        }


        public void changePosition(Vector2 dist)
        {
            foreach (Block b in children)
                b.position += dist;
        }
        public bool moveIfPossible(Vector2 dist, List<GameObj> gameList)
        {
            bool coll = checkForCollision(dist, gameList);
            if (!coll)
                changePosition(dist);
            return !coll;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
         
            
        }
    }
}
