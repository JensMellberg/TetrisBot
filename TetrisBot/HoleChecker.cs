using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TetrisBot
{
    public class HoleChecker
    {
        Bitmap img;
        bool run = false;
        Game1 game;
        public HoleChecker(Game1 game)
        {
            this.game = game;
        }
        public void runner()
        {
            while (true)
            {
                Thread.Sleep(50);
                System.Drawing.Rectangle bounds = System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                    }
                    img = bitmap;
                    searchForGray(img);
                }
            }
        }
        public void setImg(Bitmap img)
        {
            this.img = img;
            run = true;
        }
        public void searchForGray(Bitmap img)
        {
            int j = 888;
            j -= game.blocklines * 32;
            for (int i = 360; i < 665; i++)
            {
                System.Drawing.Color pixel = img.GetPixel(i, j);
                if (game.isGrayIsh(pixel))
                {
                    pixel = img.GetPixel(i + 1, j);
                    int c = 1;
                    while (game.isGrayIsh(pixel))
                    {
                        //  Console.WriteLine("GrayTrain" + c + " :" + (i + c) + " " + j);
                        c++;
                        pixel = img.GetPixel(i + c, j);
                    }
                    if (c > 40 && game.YtoHeight(j) >= game.blocklines)
                    {
                        pixel = img.GetPixel(i, j);
                        Console.WriteLine("Start at " + i + " " + j + " p:" + pixel.R + " " + pixel.G + " " + pixel.B);
                        Console.WriteLine("Found block line at " + game.YtoHeight(j) + " with length " + c);
                       game.blocklines++;
                        int holepos = game.findHole(img);
                        Console.WriteLine("Block hole at x: " + holepos);
                        game.addBlockLine(holepos);
                        //timer = 100000000;
                        //break;
                        //Thread.Sleep(1000);
                    }
                    i += c;
                }
            }
        }
    }
}
