using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BasicInterpreter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("BASIC INTERPRETER CS V1.0");
            Console.WriteLine("\t\twinxos 2012/11/23");
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
    }
}
