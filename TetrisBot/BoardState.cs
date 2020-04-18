using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace TetrisBot
{
    public class BoardState
    {
        List<GameObj> board = new List<GameObj>();
        Piece currentPiece;
        public Block[,] field = new Block[50, 50];
        double a = -0.3; //Height
        double b = 4; //Lines
        double c = -410.5; //Holes
        double d = -3; //Bumpyness
        double e = -400; //Line 1 filled
        double f = 1.2; //Exponential height increase
        double g = -35; //bumpiness punishment
        double h = -0.9; //maxHeight
        Game1.Params constants;
        bool hasCustomized = false;
        bool[] boolValue = {false, false, true };
        public BoardState(List<GameObj> list, Piece currentPiece)
        {
            board = list;
            this.currentPiece = currentPiece;
        }
        public int[,] getField()
        {
            int[,] fields = new int[10, 20];
            for (int i = 0; i < 10; i++)
                for (int x = 0; x < 20; x++)
                    if (field[i, x] == null)
                        fields[i, x] = 0;
                    else
                        fields[i, x] = 1;
            return fields;
        }
        public void setConstants(Game1.Params g) {
            hasCustomized = true;
            this.constants = g;
            a = g.a; b = g.b; c = g.c; d = g.d; e = g.e; f = g.f; this.g = g.g;
        }
        public void ConstructField()
        {
            field = new Block[10, 20];
            foreach (GameObj g in board)
                if (g is Block)
                {
                    int x = ((int)g.position.X - 150) / 20;
                    int y = ((480 - (int)g.position.Y)) / 20;
                    if (y > 19)
                        continue;
                    if (x < 0)
                    {
                        Console.WriteLine(currentPiece.version);
                        Thread.Sleep(1000000);
                    }
                    field[x, y] = (Block)g;
                }
            /*
            string f = "";
            for (int i = 0; i < 10; i++)
                for (int x = 0; x < 20; x++)
                    if (field[i, x] != null)
                        f += i + ":" + x+" ";
            System.Windows.Forms.MessageBox.Show(f);
            */

        }
        public void movePiece(int steps, int spins)
        {
            int multiplier = steps > 0 ? 1 : -1;
            while (spins != 0)
            {
                currentPiece.TurnIfPossible(board,true);
                spins--;
            }
            while (steps != 0)
            {
                currentPiece.changePosition(new Vector2(20 * multiplier, 0));
                steps -= multiplier;
            }

            currentPiece.updateShadow(board);
            currentPiece.dropDown(board);
            ConstructField();
          
        }
        double cTotal = -1;
        public double getHeight()
        {
            if (cTotal != -1)
                return cTotal;
            double total = 0;
            for (int i = 0; i < 10; i++) {
                total += columnHeight(i) * Math.Pow(f,columnHeight(i));
            }
            cTotal = total; 
            return total;
        }
        int[] cHeight = new int[10];
        public int columnHeight(int col)
        {
            if (cHeight[col] != 0)
                return cHeight[col] - 1;
            int h = 19;
            while (field[col, h] == null && h != 0)
                h--;
            if (h == 0 && field[col, 0] == null)
                h = -1;
            cHeight[col] = h + 2;
            return h + 1;
        }
        public int Lines()
        {
            int lines = 0;
            for (int i = 0; i < 20; i++)
            {
                if (isLine(i))
                    lines++;
            }
            return lines;
        }
        int[] cLines = new int[20];
        public bool isLine(int line)
        {
            if (cLines[line] != 0)
                return boolValue[cLines[line]];
            for (int x = 0; x < 10; x++)
                if (field[x, line] == null)
                {
                    cLines[line] = 1;
                    return false;
                }
            cLines[line] = 2;
            return true;
        }
        int cHoles = 0;
        public int Holes()
        {
            if (cHoles != 0)
                return cHoles - 1;
            int holes = 0;
            for (int i = 0; i < 10; i++)
                for (int x = 0; x < 20; x++)
                    if (isHole(i, x))
                        holes++;
            cHoles = holes + 1;
            return holes;
        }
        int[,] cHole = new int[10, 20];
        public bool isHole(int x, int y)
        {
            if (field[x, y] != null)
                return false;
            while (y != 19)
            {
                if (field[x, y + 1] != null)
                    if (!isLine(y+1))
                    return true;
                y++;
            }
            return false;
        }
        int cMax = 0;
        public int maxHeight()
        {
            if (cMax != 0)
                return cMax -1;
            int max = 0;
            for (int i = 0; i < 10; i++)
                if (columnHeight(i) > max)
                    max = columnHeight(i);
            cMax = max + 1;
            return max;
        }
        public int Bumpiness()
        {

            int total = 0;
            for (int i = 0; i < 9; i++)
            {
                int abs = Math.Abs(columnHeight(i) - columnHeight(i + 1));
                if (abs > 2 && (e > 0 || (i != 0 || (i == 0 && columnHeight(0) > 0)) && (i != 8 || (i == 8 && columnHeight(9) > 0))))
                    abs -= (int)g;
                total += abs;
            }
            return total;
        }
        public double getScore(int count, Piece[] nextPieces)
        {
            if (count == 0)
            {
                double bonus = 0;
                if (columnHeight(0) > 0 && columnHeight(9) > 0)
                    bonus += e;
                if (Lines() > 3)
                    bonus += 1000;
                if (Lines() == 3 && Holes() > 0)
                    bonus += 500;
                if (maxHeight() > 12)
                {
                    bonus += Lines() * 200;
                    bonus -= 8*(maxHeight() - 12);
                }
                if (maxHeight() == Lines())
                    bonus += 500000;
                return a * getHeight() + b * Lines() + c * Holes() + d * Bumpiness() + bonus + h * maxHeight();
            }
            double bestValue = int.MinValue;
            Piece myPiece = nextPieces[nextPieces.Length - count];
            for (int x = 0; x <= myPiece.maxMode; x++)
            {
                int[] loop = myPiece.minMax(x);
            for (int i = loop[0]; i < loop[1]; i++)
            {              
                    List<GameObj> temp = new List<GameObj>();
                    foreach (GameObj g in board)
                        if (g is Block && !(g is Piece))
                            temp.Add(((Block)g).copy());
                    BoardState b2 = new BoardState(temp, (Piece)myPiece.copy());
                    if (hasCustomized)
                        b2.setConstants(constants);
                    b2.movePiece(i, x);
                    double value = b2.getScore(count - 1, nextPieces);
                    if (value > bestValue)
                    {
                        bestValue = value;
                    }
                }

            }
            return bestValue;
        }
    }
}
