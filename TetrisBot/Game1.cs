using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace TetrisBot
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// Scanning location for first piece: Left+130 to Left+280. Top+160 to Top+190
    /// Second pieces: Left+400 to Left+500. Top+190 to Top+260
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public GameObj Board;
        public List<GameObj> gameList = new List<GameObj>();
        List<GameObj> baseList = new List<GameObj>();
        public Vector2 boardPosition = new Vector2(150, 100);
        Piece currentPiece;
        Piece savedPiece;
        public int turnCd;
        public int stepCd;
        int mode = 1;
        public int keyCd;
        bool ready;
        Piece nextPiece;
        public bool useNext = false;
        int x = 700, y = 250, maxX = 770, maxY = 450, btnPos = 400;
        Params myConstants = new Params();
        Thread holeChecker;
        HoleChecker HoleChecker;
        Vector2 windowLocation;
        bool foundWindowPosition;
        public int[,] boardread = null;
        //Averages: BLUE: 39 40 130 YELLOW: 84 89 30 GREEN: 7 172 9 RED: 128 21 22 PINK: 129 22 131 WHITE: 92 93 94 TEAL: 17 92 93
        Vector3[] firstAvrgs = { new Vector3(0, 0, 0), new Vector3(7, 172, 9), new Vector3(84, 89, 30), new Vector3(128, 20, 20), new Vector3(129, 20, 131), new Vector3(39, 40, 130), new Vector3(16, 92, 93), new Vector3(92, 93, 94) };
        Vector3[] Avrgs = { new Vector3(0, 0, 0), new Vector3(13, 124, 26), new Vector3(135, 147, 61), new Vector3(140, 36, 40), new Vector3(141, 35, 153), new Vector3(48, 56, 145), new Vector3(28, 146, 150), new Vector3(140, 140, 152) };
        public Game1()
        {
            HoleChecker = new HoleChecker(this);
            holeChecker = new Thread(new ThreadStart(HoleChecker.runner));
            //holeChecker.Start();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            myConstants.a = -0.3; myConstants.b = 4; myConstants.c = -410.5; myConstants.d = -3; myConstants.e = -400; myConstants.f = 1.2; myConstants.g = -35; myConstants.h = -0.9;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            Board = new GameObj(boardPosition, 200, 400, this);
            System.Windows.Forms.Form MyGameForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            MyGameForm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MyGameForm.Opacity = 0;
            MyGameForm.Hide();
            if (boardread != null)
                readBoard(boardread);

            return;
            Bitmap bitmap = (Bitmap)Bitmap.FromFile("C:\\Users\\Jens\\source\\repos\\TetrisBot\\TetrisBot\\Content\\testblock2.png");
            Vector3 avrg = new Vector3(0, 0, 0);
            for (int p1 = 0; p1 < 31; p1++)
            {
                for (int p2 = 0; p2 < 31; p2++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(p1, p2);
                    avrg.X += pixel.R;
                    avrg.Y += pixel.G;
                    avrg.Z += pixel.B;
                }
            }
            avrg.X /= (31 * 31);
            avrg.Y /= (31 * 31);
            avrg.Z /= (31 * 31);

            Console.WriteLine("avggg: " + avrg.X + " " + avrg.Y + " " + avrg.Z);
            Thread.Sleep(50000);



        }
        public void readBoard(int[,] board)
        {
            if (board == null)
                return;
            baseList = new List<GameObj>();
            for (int i = 0; i < 10; i++)
                for (int x = 0; x < 20; x++)
                    if (board[i, x] == 1)
                        baseList.Add(new Block(boardPosition + new Vector2(i * 20, 380 - x * 20), Microsoft.Xna.Framework.Color.White, this));
            gameList = baseList;
        }
        public void setParams(List<string> sets)
        {
            foreach (string s in sets)
            {
                double set;
                //double set =int.Parse(s.Substring(1,s.Length-1));
                double.TryParse(s.Substring(1, s.Length - 1), out set);
                switch (s[0])
                {
                    case 'a': myConstants.a = set; break;
                    case 'b': myConstants.b = set; break;
                    case 'c': myConstants.c = set; break;
                    case 'd': myConstants.d = set; break;
                    case 'e': myConstants.e = set; break;
                    case 'f': myConstants.f = set; break;
                    case 'g': myConstants.g = set; break;
                    case 'h': myConstants.h = set; break;
                }
            }
        }
        public void Cultris()
        {
            myConstants.b = 10; myConstants.c = -50; myConstants.d = -4; myConstants.e = 0; myConstants.g = -5;
            mode = 2;
            btnPos = 420;
            x = 400; maxX = 500; //750,850          //400,600
            y = 190; maxY = 260; //350,420          //200,500  new PixelCheck(244,255,88,2,"yellow")
            pixels = new PixelCheck[]{new PixelCheck(50,255,255,6,"teal"),new PixelCheck(237,245,250,7,"white"),new PixelCheck(238,239,240,7,"whiteN"),
                new PixelCheck(255,255,97,2,"yellow"),new PixelCheck(255,44,255,4,"pink"),new PixelCheck(255,69,74,3,"red"),new PixelCheck(255,40,41,3,"redN")
                ,new PixelCheck(57,65,199,5,"blue"),new PixelCheck(81,90,255,5,"blue"),new PixelCheck(64,65,243,5,"blueN")  ,new PixelCheck(13,255,27,1,"green"),new PixelCheck(2,249,4,1,"greenN")};
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void UnloadContent()
        {
        }
        public struct Params
        {
            public double a, b, c, d, e, f, g, h;
        }
        public bool isBlack(System.Drawing.Color pixel)
        {
            if (pixel.R < 40 && pixel.B < 40 && pixel.G < 40)
                return true;
            return false;
        }
        public bool isGrayIsh(System.Drawing.Color pixel)
        {
            if (pixel.R < 50 || pixel.B > 100)
                return false;
            int maxDiff = 5;
            if (Math.Abs(pixel.R - pixel.G) > maxDiff || Math.Abs(pixel.G - pixel.B) > maxDiff)
                return false;
            return true;
        }
        int timer = 10;
        bool btnFound = true;
        public void reset()
        {
            gameList = new List<GameObj>();
            readBoard(boardread);
            savedPiece = null;
            currentPiece = null;
            cansave = true;
            first = true;
            doSpecial = false;
            blocklines = 0;
        }
        public int YtoHeight(int y)
        {
            y -= 900;
            while (y % 32 != 0)
                y++;
            y /= 32;
            y *= -1;
            return y;
        }

        PixelCheck[] pixels = new PixelCheck[]{new PixelCheck(134,234,51,6,"green"),new PixelCheck(234,49,86,7,"red"),new PixelCheck(255,217,59,2,"yellow"),new PixelCheck(255,156,35,4,"orange"),new PixelCheck(68,124,255,5,"blue")
            ,new PixelCheck(232,76,201,3,"pink") ,new PixelCheck(44,209,255,1,"teal")};
        bool first = true;
        public int xToWidth(int x)
        {
            return 376 + 32 * x;
        }

        public int findHole(Bitmap img)
        {
            int count = 0;
            int yCheck = 894;
            while (!isBlack(img.GetPixel(xToWidth(count), yCheck)) && count < 10) {
                // Console.WriteLine(img.GetPixel(xToWidth(count), yCheck).R + " " + img.GetPixel(xToWidth(count), yCheck).G + " " + img.GetPixel(xToWidth(count), yCheck).B + " ");
                count++;
            }
            if (count == 10)
                img.Save("print" + blocklines + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            return count;
        }
        public void addBlockLine(int holePos)
        {
            foreach (GameObj g in gameList)
            {
                g.position.Y -= 20;
            }
            for (int i = 0; i < 10; i++)
            {
                if (i != holePos)
                    gameList.Add(new GrayBlock(boardPosition + new Vector2(20 * i, 380), this));
            }
        }
        public int blocklines = 0;
        Bitmap img;
        protected override void Update(GameTime gameTime)
        {
            if (!foundWindowPosition)
            {
                Process[] processes = Process.GetProcessesByName("Cultris II");
                if (processes.Length == 0)
                    return;

                Rect r = WindowLocation();
                windowLocation = new Vector2(r.Left, r.Top);
                Console.WriteLine("Found window at: " + r.Left + " " + r.Right + " " + r.Top + " " + r.Bottom);
                foundWindowPosition = true;
            }

            counter++;
            if (counter == 200)
            {
                Console.WriteLine("Haven't found a piece in a while, resetting game memory");
                Console.WriteLine("Scanning for pieces");
                timer = 50;
                reset();
                return;
            }
            timer--;
            if (timer > 0)
                return;
            if (!ready)
            {
                ready = true;
                Console.WriteLine("Program is up and running");
                Console.WriteLine("Scanning for pieces");
            }
            timer = 10;





            System.Drawing.Rectangle bounds = System.Windows.Forms.Screen.GetBounds(System.Drawing.Point.Empty);
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
               
               
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                }
                // bitmap.Save("print.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                img = bitmap; //700-750,300-350
                if (mode == 2)
                    HoleChecker.setImg(img);
                int[] loop1 = new int[] { (int)windowLocation.X + x, (int)windowLocation.X + maxX };
                int[] loop2 = new int[] { (int)windowLocation.Y + y, (int)windowLocation.Y + maxY };
                // windowLocation = new Vector2(329, 180);
                Rect r = WindowLocation();
                windowLocation = new Vector2(r.Left, r.Top);
                if (first && mode == 2)
                {
                    loop1 = new int[] { (int)windowLocation.X + 130, (int)windowLocation.X + 280 };
                    loop2 = new int[] { (int)windowLocation.Y + 160, (int)windowLocation.Y + 190 };
                }
                Vector3 avg = scanForAverage(img, loop1, loop2);
                Vector3[] averages = Avrgs;
                if (first)
                    averages = firstAvrgs;
                for (int i = 1; i < averages.Length; i++)
                {
                    Vector3 result = avg - averages[i];
                    if (Math.Abs(result.X) < 10 && Math.Abs(result.Y) < 10 && Math.Abs(result.Z) < 10)
                    {
                        Console.WriteLine("Matching piece: " + i);
                        readBoard(getBoardStateFromImg(img, windowLocation));
                        watch.Stop();
                        Console.WriteLine("Time for processing the image: " + watch.ElapsedMilliseconds);
                        Thread.Sleep(100000);
                        foundPiece(i);
                    }
                }
                //Left + 130 to Left+280.Top + 160 to Top+190   39 40 123
                //Image.FromFile("print.jpg");
                //pictureBox1.Image = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.DontCare);
                watch.Stop();
            }


            base.Update(gameTime);
        }
        Vector3 scanForAverage(Bitmap img, int[] loopX, int[] loopY)
        {
            Vector3 avrg = new Vector3(0, 0, 0);
            int counter = 0;
            for (int i = loopX[0]; i < img.Width && i < loopX[1]; i++)
            {
                for (int j = loopY[0]; j < img.Height && j < loopY[1]; j++)
                {
                    counter++;
                    System.Drawing.Color pixel = img.GetPixel(i, j);
                    avrg.X += pixel.R;
                    avrg.Y += pixel.G;
                    avrg.Z += pixel.B;
                }
            }
            avrg.X /= counter;
            avrg.Y /= counter;
            avrg.Z /= counter;
            return avrg;
        }
        bool cansave = true;
        public void savePiece()
        {
            Console.WriteLine("saving piece");
            cansave = false;
            if (savedPiece == null)
            {
                savedPiece = currentPiece;
                timer = 10;
            }
            else
            {
                Piece temp = currentPiece;
                currentPiece = savedPiece;
                savedPiece = temp;
            }
            Thread.Sleep(stepCd);
            SendKeyDown(System.Windows.Forms.Keys.C);
            Thread.Sleep(keyCd);
            SendKeyUp(System.Windows.Forms.Keys.C);
        }
        bool doSpecial = false;
        int counter = 1000;
        IntPtr windownHandle()
        {
            Process[] b = Process.GetProcessesByName("Cultris II");
            return b[0].MainWindowHandle;
        }
        public void foundPiece(int version)
        {
            counter = 0;
            if (mode == 2)
                cansave = false;
            btnFound = false;
            timer = 1000000000;
            if (mode == 2)
            {
                //Cultris
                if (first)
                {
                    first = false;
                    currentPiece = new Piece(boardPosition, version, this, gameList);
                    currentPiece.updateShadow(gameList);
                    doSpecial = true;
                    timer = 10;
                    return;
                }
                if (!doSpecial)
                    currentPiece = nextPiece;
                else
                    doSpecial = false;

                nextPiece = new Piece(boardPosition, version, this, gameList);
                nextPiece.updateShadow(gameList);
            }
            else
            {
                currentPiece = new Piece(boardPosition, version, this, gameList);
                currentPiece.updateShadow(gameList);
            }
            if (savedPiece == null && mode == 1)
            {
                savePiece();
                return;
            }
            int[] moves = new int[3];
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int[] moves1 = calculateBestMove(currentPiece);
            int[] moves2;
            if (cansave)
                moves2 = calculateBestMove(savedPiece);
            else
                moves2 = new int[] { -5, -5, int.MinValue };
            moves = moves1;
            if (moves2[2] > moves1[2])
            {
                savePiece();
                moves = moves2;
            }
            watch.Stop();
            Console.WriteLine("Time for calculations: " + watch.ElapsedMilliseconds);
            string c = "";
            for (int i = 0; i < pixels.Length; i++)
                if (pixels[i].version == currentPiece.version)
                    c = pixels[i].color;
            Console.WriteLine(moves[1] + " turns " + moves[0] + " steps " + c);
            int multiplier = moves[0] > 0 ? 1 : -1;
            int turn = moves[1];
            int step = moves[0];
            while (moves[1] > 0)
            {
                currentPiece.TurnIfPossible(gameList, false);
                moves[1]--;
            }
            while (moves[0] != 0)
            {
                currentPiece.moveIfPossible(new Vector2(20 * multiplier, 0), gameList);
                moves[0] -= multiplier;
            }
            currentPiece.updateShadow(gameList);
            currentPiece.dropDown(gameList);
            checkForLines();
            string f = "";
            foreach (GameObj b in gameList)
                f += b.position.X + ":" + b.position.Y + " ";
            //Console.WriteLine(f);
            Console.WriteLine("Best result: " + moves[2]);
            doMoveOnBrowser(turn, step);

        }
        public void doMoveOnBrowser(int turn, int steps)
        {
            System.Windows.Forms.Keys turnkey = System.Windows.Forms.Keys.Up;
            System.Windows.Forms.Keys stepkey = System.Windows.Forms.Keys.Right;
            int mult = 1;
            if (steps < 0)
            {
                stepkey = System.Windows.Forms.Keys.Left;
                mult = -1;
            }
            if (turn == 3)
            {
                turn = 1;
                turnkey = System.Windows.Forms.Keys.Down;
            }
            if (turn == 2)
            {
                turn = 1;
                turnkey = System.Windows.Forms.Keys.R;
            }
            while (steps != 0 || turn != 0)
            {

                if (turn != 0)
                    SendKeyDown(turnkey);
                //PostKeyDown(turnkey);

                if (steps != 0)
                {
                    //  PostKeyDown(stepkey);
                    SendKeyDown(stepkey);
                }
                bool doTurn = false;
                Thread.Sleep(keyCd);
                if (turn != 0)
                {
                    doTurn = true;
                    //PostKeyUp(turnkey);
                    SendKeyUp(turnkey);
                    turn--;
                }
                if (steps != 0)
                {
                    //PostKeyUp(stepkey);
                    SendKeyUp(stepkey);
                    steps -= mult;
                }
                if (doTurn)
                    Thread.Sleep(turnCd);
                else
                    Thread.Sleep(stepCd);
            }
            Thread.Sleep(10);
            SendKeyDown(System.Windows.Forms.Keys.Space);
            //PostKeyDown(System.Windows.Forms.Keys.Space);
            Thread.Sleep(turnCd);
            // PostKeyUp(System.Windows.Forms.Keys.Space);
            SendKeyUp(System.Windows.Forms.Keys.Space);
            cansave = true;
            Thread.Sleep(turnCd);
            timer = 1;
            counter = 0;
        }
        bool other = false;
        public int[] calculateBestMove(Piece piece)
        {
            if (!other)
                other = true;
            else
                other = false;
            other = true;
            int[] best = new int[3];
            BoardState bestb = new BoardState(null, null);
            double bestValue = int.MinValue;
            for (int x = 0; x <= piece.maxMode; x++)
            {
                int[] loop = piece.minMax(x);
                for (int i = loop[0]; i <= loop[1]; i++)
                {
                    List<GameObj> temp = new List<GameObj>();
                    foreach (GameObj g in gameList)
                        if (g is Block && !(g is Piece))
                            temp.Add(((Block)g).copy());
                    BoardState b = new BoardState(temp, (Piece)piece.copy());
                    b.setConstants(myConstants);
                    b.movePiece(i, x);
                    double value = -1000000;
                    if (useNext && other)
                        value = b.getScore(1, new Piece[] { nextPiece });
                    else
                        value = b.getScore(0, null);
                    if (value > bestValue)
                    {
                        bestValue = value;
                        best[0] = i;
                        best[1] = x;
                        bestb = b;
                    }
                }
            }
            best[2] = (int)bestValue;
            printField(bestb.getField());
            return best;
        }
        public void printField(int[,] field)
        {
            string h = "";
            for (int x = 19; x > -1; x--)
            {
                for (int i = 0; i < 10; i++)
                    h += field[i, x].ToString();
                h += "\n";
            }
            Console.WriteLine(h);
        }
        List<int> findings;
        public List<int> blockLinesD = new List<int>();
        public void checkForLines()
        {
            blockLinesD = new List<int>();
            findings = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                bool totalfind = true;
                for (int x = 0; x < 10; x++)
                {
                    bool find = false;
                    foreach (GameObj g in gameList)
                    {
                        if (g is Block && g.position.Equals(boardPosition + new Vector2(20 * x, 20 * i)))
                            find = true;
                    }
                    if (!find)
                    {
                        totalfind = false;
                        break;
                    }
                }
                if (totalfind)
                {
                    findings.Add(i);
                }
            }
            if (findings.Count < 1)
            {
                return;
            }
            foreach (int i in findings)
            {
                for (int f = 0; f < gameList.Count; f++)
                    if (gameList[f] is Block && gameList[f].position.Y == boardPosition.Y + 20 * i)
                    {
                        ((Block)gameList[f]).destroy();
                        gameList.RemoveAt(f);
                        f--;
                    }
                foreach (GameObj g in gameList)
                    if (g is Block && g.position.Y < boardPosition.Y + 20 * i)
                        g.position.Y += 20;
                Console.WriteLine("Cleared line " + (19 - i));
            }
            foreach (int d in blockLinesD)
                blocklines--;

        }
        int[,] getBoardStateFromImg(Bitmap img, Vector2 windowPos)
        {
            //+38 +156 start pos 560 total height ca 31x31 boxes
            Vector2 startPos = windowPos + new Vector2(38, 156);
            int[,] board = new int[10, 20];
            for (int x = 0; x < 10; x++)
            {
                for (int i = 3; i < 18; i++)
                {
                    int[] loopX = { (int)startPos.X + 31 * x, (int)startPos.X + 31 * x + 31 };
                    int[] loopY = { (int)startPos.Y + 31 * i, (int)startPos.Y + 31 * i + 31 };
                    Vector3 avrg = scanForAverage(img, loopX, loopY);
                    if (avrg.X < 100 && avrg.Y < 100 && avrg.Z < 100) { }
                    else
                    {
                        //Console.WriteLine("Found block at height: " + i + " width: " + x);
                        board[x, 20-i] = 1;

                      //  Bitmap C = new Bitmap(31, 31);
                        //Graphics G = Graphics.FromImage(C);
                        //System.Drawing.Rectangle src = new System.Drawing.Rectangle((int)startPos.X + 31 * x, (int)startPos.Y + 31 * i, 31, 31);
                        //System.Drawing.Rectangle dst = new System.Drawing.Rectangle(0, 0, 31,31);
                        //G.DrawImage(img, dst, src, GraphicsUnit.Pixel);
                        //G.Dispose();
                        //C.Save(x+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                        //C.Dispose();


                        break;
                    }
                }
            }
            //printField(board);
            return board;

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }


        public static Rect WindowLocation()
        {
            Process[] processes = Process.GetProcessesByName("Cultris II");
            Process lol = processes[0];
            IntPtr ptr = lol.MainWindowHandle;
            Rect rect = new Rect();
            GetWindowRect(ptr, ref rect);
            return rect;
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);



        public static uint Click()
        {
            INPUT structure = new INPUT();
            structure.mi.dx = 0;
            structure.mi.dy = 0;
            structure.mi.mouseData = 0;
            structure.mi.dwFlags = 2;
            INPUT input2 = structure;
            input2.mi.dwFlags = 4;
            INPUT[] pInputs = new INPUT[] { structure, input2 };
            return SendInput(2, pInputs, Marshal.SizeOf(structure));
        }

        public static uint Z()
        {
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wVk = VkKeyScan((char)System.Windows.Forms.Keys.Z);
            structure.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT input2 = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wVk = VkKeyScan((char)System.Windows.Forms.Keys.Z);
            input2.mi.dwFlags = (int)KEYEVENTF.KEYUP;
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { structure, input2 };

            return SendInput(2, pInputs, Marshal.SizeOf(structure));
        }
        public void PostKeyDown(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Right)
                PostMessage(windownHandle(), KEYDOWN, DOWNwP, DOWNlp);
            if (key == System.Windows.Forms.Keys.Left)
                PostMessage(windownHandle(), KEYDOWN, LEFT_DOWNwP, LEFT_DOWNlp);
            if (key == System.Windows.Forms.Keys.Up)
                PostMessage(windownHandle(), KEYDOWN, UP_DOWNwP, UP_DOWNlp);
            if (key == System.Windows.Forms.Keys.Down)
                PostMessage(windownHandle(), KEYDOWN, DOWN_DOWNwP, DOWN_DOWNlp);
            if (key == System.Windows.Forms.Keys.Space)
            {
                PostMessage(windownHandle(), KEYDOWN, SPACE_DOWNwP, SPACE_DOWNlp);
                PostMessage(windownHandle(), CHAR, CHARwP, CHARlP);
            }
        }
        public void PostKeyUp(System.Windows.Forms.Keys key)
        {
            if (key == System.Windows.Forms.Keys.Right)   
                PostMessage(windownHandle(), KEYUP, UPwP, UPlP);
            if (key == System.Windows.Forms.Keys.Left)
                PostMessage(windownHandle(), KEYUP, LEFT_UPwP, LEFT_UPlp);
            if (key == System.Windows.Forms.Keys.Up)
                PostMessage(windownHandle(), KEYUP, UP_UPwP,UP_UPlp);
            if (key == System.Windows.Forms.Keys.Down)
                PostMessage(windownHandle(), KEYUP, DOWN_UPwP, DOWN_UPlp);
             if (key == System.Windows.Forms.Keys.Space)
                PostMessage(windownHandle(), KEYDOWN, SPACE_UPwP, SPACE_UPlp);
        }
        public static void SendKeyUp(System.Windows.Forms.Keys key)
        {
            /*
            INPUT structure = new INPUT();
            structure.type = (int)InputType.INPUT_KEYBOARD;
            structure.ki.wVk = (short)key;
            structure.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
            structure.ki.dwExtraInfo = GetMessageExtraInfo();
            */
            INPUT input2 = new INPUT();
            input2.type = (int)InputType.INPUT_KEYBOARD;
            input2.ki.wVk = (short)key;
            input2.ki.dwFlags = (int)KEYEVENTF.KEYUP;
            input2.ki.dwExtraInfo = GetMessageExtraInfo();

            INPUT[] pInputs = new INPUT[] { input2 };

            SendInput(1, pInputs, Marshal.SizeOf(input2));
        }

        public static void SendKeyDown(System.Windows.Forms.Keys key)
        {
            INPUT INPUT1 = new INPUT();
            INPUT1.type = (int)InputType.INPUT_KEYBOARD;
            INPUT1.ki.wVk = (short)key;
            INPUT1.ki.dwFlags = (int)KEYEVENTF.KEYDOWN;
            INPUT1.ki.dwExtraInfo = GetMessageExtraInfo();
            SendInput(1, new INPUT[] { INPUT1 }, Marshal.SizeOf(INPUT1));
            /*
            WaitForSingleObject((IntPtr)0xACEFDB, (uint)HoldTime);

            INPUT INPUT2 = new INPUT();
            INPUT2.type = (int)InputType.INPUT_KEYBOARD;
            INPUT2.ki.wVk = (short)key;
            INPUT2.mi.dwFlags = (int)KEYEVENTF.KEYUP;
            INPUT2.ki.dwExtraInfo = GetMessageExtraInfo();
            SendInput(1, new INPUT[] { INPUT2 }, Marshal.SizeOf(INPUT2));
             */

        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT
        {
            [FieldOffset(4)]
            public HARDWAREINPUT hi;
            [FieldOffset(4)]
            public KEYBDINPUT ki;
            [FieldOffset(4)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public int type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        public enum InputType
        {
            INPUT_MOUSE = 0,
            INPUT_KEYBOARD = 1,
            INPUT_HARDWARE = 2
        }

        [Flags]
        public enum MOUSEEVENTF
        {
            MOVE = 0x0001, /* mouse move */
            LEFTDOWN = 0x0002, /* left button down */
            LEFTUP = 0x0004, /* left button up */
            RIGHTDOWN = 0x0008, /* right button down */
            RIGHTUP = 0x0010, /* right button up */
            MIDDLEDOWN = 0x0020, /* middle button down */
            MIDDLEUP = 0x0040, /* middle button up */
            XDOWN = 0x0080, /* x button down */
            XUP = 0x0100, /* x button down */
            WHEEL = 0x0800, /* wheel button rolled */
            MOVE_NOCOALESCE = 0x2000, /* do not coalesce mouse moves */
            VIRTUALDESK = 0x4000, /* map to entire virtual desktop */
            ABSOLUTE = 0x8000 /* absolute move */
        }

        [Flags]
        public enum KEYEVENTF
        {
            KEYDOWN = 0,
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            UNICODE = 0x0004,
            SCANCODE = 0x0008,
        }
        uint KEYDOWN = 0x100,
           KEYUP = 0x101,
           UPwP = 0x027,
           DOWNwP = 0x027,
           UPlP = 0xC14D0001,
           DOWNlp = 0x014D0001,
        LEFT_DOWNwP = 0x025,
           LEFT_DOWNlp = 0x014B0001,
           LEFT_UPwP = 0x025,
           LEFT_UPlp = 0xC14B0001,
           CHAR = 0x102,
           CHARwP = 0x20,
           CHARlP = 0x00390001,
        SPACE_DOWNwP = 0x020,
           SPACE_DOWNlp = 0x00390001,
            SPACE_UPwP = 0x020,
           SPACE_UPlp = 0xC0390001,
           UP_DOWNwP = 0x026,
           UP_DOWNlp = 0x01480001,
           UP_UPwP = 0x026,
           UP_UPlp = 0xC1480001,
            DOWN_DOWNwP = 0x028,
           DOWN_DOWNlp = 0x01500001,
            DOWN_UPwP = 0x028,
           DOWN_UPlp = 0xC1500001;
    }
}
