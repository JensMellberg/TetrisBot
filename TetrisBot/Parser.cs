using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TetrisBot
{
    public class Parser
    {
        const string FilePath = "text.txt";
        List<string> list = new List<string>();
        int counter = 0;
        public Parser(string input)
        {
            string[] temps = input.Split(' ');
            for (int i = 0; i< temps.Length; i++) {
                string[] temp2 = temps[i].Split(':');
                for (int x = 0; x < temp2.Length; x++)
                    list.Add(temp2[x]);
            }

        }
        public string getNextToken()
        {
            counter++;
            return list[counter-1];            
            }
        public bool hasNext()
        {
            return counter < list.Count;
        }
        public static void storeText(string text)
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
            StreamWriter write = new StreamWriter(FilePath);
            write.WriteLine(text);
            write.Close();
        }
        public static string getText()
        {
            if (!File.Exists(FilePath))
                return "";
            string ret = "";
            StreamReader read = new StreamReader(FilePath);
            ret = read.ReadLine();
           read.Close();
            return ret;
        }
        public static int[,] boardFromFile(string txt) {
            if (!File.Exists(txt))
                return null;
            StreamReader read = new StreamReader(txt);
            int[,] returner = new int[10,20];
            string line = read.ReadLine();
            int count = 20;
            while (line != null)
            {
                count--;
                for (int i = 0; i < line.Length; i++) {
                    try
                    {
                        returner[i, count] = int.Parse(line.Substring(i, 1));
                    }
                    catch { read.Close();  return null; }
                }
                line = read.ReadLine();
            }
            read.Close();
            if (count == 0)
                return returner;
            else return null;
        }
    }
}
