using System;
using System.IO;
namespace SimpletronCore
{
    class Program
    {
        public static int StrToInt(string str)
        {
            int n=0;
            try
            {
                n = int.Parse(str);
            }
            catch { }
            return n;
        }
        static int[] inputcode()
        {
            int[] buf=new int[100];
            Console.WriteLine("Enter -1 to end input.");
            int ct = 0, li = 0;
            while (true)
            {
                Console.Write("{0:00} ? ", li);
                ct =int.Parse( Console.ReadLine());
                if (ct == -1) break;
                buf[li++] = ct;
            }
            return buf;
        }
        static void Main(string[] args)
        {
            sml sim = new sml();
            if (args.Length == 1) //source code file
            {
                
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    string s = sr.ReadToEnd();
                    s = s.TrimEnd();
                    int[] codes = Array.ConvertAll(s.Split('\n'), new Converter<string, int>(StrToInt));
                    Console.WriteLine("from {0} loaded {1} lines. ", args[0], codes.Length);
                    sim.loadCode(codes);
                    sim.run();
                }


            }
            else if (args.Length > 1)
            {
                int[] codes = Array.ConvertAll(args, new Converter<string, int>(StrToInt));
                Console.WriteLine("loaded {0} lines. ", codes.Length);
                sim.loadCode(codes);
                sim.run();
            }
            else
            {
                Console.WriteLine("\t\tSimpletron Simulator");
                Console.WriteLine("\t\t\tCS Version 1.0");
                Console.WriteLine("\t\t\t\twinxos @ 2012");
                while (true)
                {
                    sim.loadCode(inputcode());
                    sim.run();
                }
            }
        }
    }
}
