using System;

namespace TetrisBot
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            bool first = true;
            while (true)
            {
                int hold = -1;
                int step = -1;
                bool board = false;
                string type = "";
                System.Collections.Generic.List<string> constants = new System.Collections.Generic.List<string>();
                if (first)
                    Console.WriteLine("Type go followed by ' step:xx 'mode'' where xx is the time in milliseconds and 'mode' is either cult for Cultris and friend for Tetrisfriends. Type -info for extra commands");
                first = false;
                string text = Console.ReadLine();
                switch (text)
                {
                    case "-info":
                        Console.WriteLine("The value of the tetris-board state is given by \"a * (sum of each (column height * f^(column height))) + b * (complete lines) + c * (total holes) + d * (sum of all height differences "
                        + "from each column to the next (inceased by g if difference is > 2)) + h * (Height of the highest column) + e (if no corner lane is empty)\"");
                        Console.WriteLine("a:xx sets value of a to xx. (Height constant, default -0.3)");
                        Console.WriteLine("b:xx sets value of b to xx. (Lines constant, default: friend: 4, cultris: 5)");
                        Console.WriteLine("c:xx sets value of c to xx. (Holes constant, default: friend: -410.5, cultris: -20)");
                        Console.WriteLine("d:xx sets value of d to xx. (Height difference constant, default: friend: -3, cultris: -2)");
                        Console.WriteLine("e:xx sets value of e to xx. (Corner lane punishment, default: friend: -400, cultris: 0)");
                        Console.WriteLine("f:xx sets value of f to xx. (Height factor, default: 1.2)");
                        Console.WriteLine("g:xx sets value of g to xx. (Height difference > 2 punishment default: friend: -35, cultris: -5)");
                        Console.WriteLine("h:xx sets value of h to xx. (Max height constant: default: -0.9)");
                        Console.WriteLine("hold:xx where xx is the time in milliseconds between pressing the key and releasing it");
                        Console.WriteLine("turn:xx where xx is the time in milliseconds the program waits in between each keystroke when turning the piece");
                        Console.WriteLine("-save:xx where xx is any text, xx will be stored for future use");
                        Console.WriteLine("save? displays the current text stored");
                        Console.WriteLine("!save this command will be replaced by the current text stored");
                        Console.WriteLine("board allows you to enter a starting board by editing the text file");
                        Console.WriteLine("next the game will take the next piece in concideration when calculating best move");
                        continue;
                    case "save?":
                        Console.WriteLine("Current stored text: " + Parser.getText());
                        continue;
                }
                if (text.Length > 5)
                    if (text.Substring(0, 6) == "-save:")
                    {
                        Parser.storeText(text.Substring(6, text.Length - 6));
                        continue;
                    }
                text = text.Replace("!save", Parser.getText());
                Parser p = new Parser(text);
                int turn = -1;
                bool cont = false;
                bool go = false;
                bool next = false;
                while (p.hasNext())
                {
                    string token = p.getNextToken();
                    switch (token)
                    {
                        case "go": go = true; break;
                        case "next": next = true; break;
                        case "step":
                            try { step = int.Parse(p.getNextToken()); }
                            catch { Console.WriteLine("Invalid time between steps"); cont = true; }
                            break;
                        case "hold":
                            try { hold = int.Parse(p.getNextToken()); }
                            catch { Console.WriteLine("Invalid time for holding key presses"); cont = true; }
                            break;
                        case "turn":
                            try { turn = int.Parse(p.getNextToken()); }
                            catch { Console.WriteLine("Invalid time between turning key presses"); cont = true; }
                            break;
                        case "cult": type = token; break;
                        case "friend": type = token; break;
                        case "a":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "b":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "c":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "d":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "e":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "f":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "g":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "h":
                            if (!addIfPossible(constants, p.getNextToken(), token))
                                cont = true;
                            break;
                        case "board": board = true; break;
                        default: Console.WriteLine("Invalid instruction: " + token); cont = true; break;
                    }

                }
                if (cont || !go)
                    continue;
                if (type == "")
                {
                    Console.WriteLine("Game type not selected");
                    continue;
                }
                if (step == -1)
                {
                    Console.WriteLine("Time between steps not set");
                    continue;
                }
                if (hold == -1)
                    hold = step;
                if (turn == -1)
                    turn = step;
                if (type == "cult")
                    Console.WriteLine("Game mode set to Cultris");
                else
                    Console.WriteLine("Game mode set to Tetrisfriends");
                if (board)
                    Console.WriteLine("Standard board is ON");
                if (next)
                    Console.WriteLine("Next piece calculation is ON");
                Console.WriteLine("Time between each step set to " + step);
                Console.WriteLine("Time between pressing the key and releasing set to " + hold);
                foreach (string t in constants)
                    Console.WriteLine("Constant " + t[0] + " set to " + t.Substring(1, t.Length - 1));
                Console.WriteLine("Initializing program. . .");


                using (Game1 game = new Game1())
                {
                    game.stepCd = step;
                    game.keyCd = hold;
                    game.turnCd = turn;
                    if (board)
                        game.boardread = Parser.boardFromFile("board.txt");
                    if (type == "cult")
                        game.Cultris();
                    game.useNext = next;
                    game.setParams(constants);
                    game.Run();
                }
            }

        }
        static bool addIfPossible(System.Collections.Generic.List<string> list, string item, string constant)
        {
            double add = 0;
            bool neg = false;
            item = item.Replace('.', ',');
            if (item[0] == '-')
            {
                item = item.Substring(1, item.Length - 1);
                neg = true;
            }
            if (!double.TryParse(item, out add))
            {
                Console.WriteLine("Invalid value for constant");
                return false;
            }
            if (neg)
                add *= -1;
            list.Add(constant + add);
            return true;
        }
    }
#endif
}

