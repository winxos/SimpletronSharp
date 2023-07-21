/*
 * Simpletron Simulator CSharp Ver.
 * winxos 2012/11/07
 */
using System;
using System.Collections.Generic;

namespace SimpletronCore
{
    class sml
    {
        #region data

        Dictionary<int, string> keywords = new Dictionary<int, string>{
            {10,"READ"},{11,"WRITE"},
            {20,"LOAD"},{21,"STORE"},
            {30,"ADD"},{31,"SUB"},{32,"MUL"},{33,"DIV"},
            {40,"JMP"},{41,"JMPN"},{42,"JMPZ"},{43,"HALT"}};
        const int MAX_MEM = 100;
        const int MAX_VALUE = +9999;
        const int MIN_VALUE = -9999;
        int[] mem = new int[MAX_MEM];
        int acc = 0;
        int pc = 0;
        int opcode = 0;
        int operand = 0;
        int count = 0;
        #endregion
        public void loadCode(int[] cs)
        {
            Array.Clear(mem, 0, mem.Length);
            cs.CopyTo(mem, 0);
        }
        public void dump()
        {
            const int linewidth = 10;
            Console.WriteLine("ACC:{0,5}  PC:{1,5} CNT:{2,5}", acc, pc,count);
            Console.Write("    ");
            for (int i = 0; i < linewidth; i++)
            {
                Console.Write("{0,5}", i);
            }
            Console.WriteLine();
            for (int i = 0; i < MAX_MEM / linewidth; i++)
            {
                Console.Write("{0,3}  ", i * 10);
                for (int j = 0; j < linewidth; j++)
                {
                    Console.Write("{0:0000} ", mem[i * linewidth + j]);
                }
                Console.WriteLine();
            }

        }
        public int run()
        {
            pc = 0;
            count = 0;
            while (pc < mem.Length)
            {
                int instructionCode = mem[pc];
                opcode = instructionCode / mem.Length;
                operand = instructionCode % mem.Length;
                count++;
                if (keywords.ContainsKey(opcode))
                {
                    switch (keywords[opcode])
                    {
                        case "READ": mem[operand] = Convert.ToInt32(Console.ReadLine()); break;
                        case "WRITE": Console.Write("{0} ", mem[operand]); break;
                        case "LOAD": acc = mem[operand]; break;
                        case "STORE": mem[operand] = acc; break;
                        case "ADD": acc += mem[operand]; break;
                        case "SUB": acc -= mem[operand]; break;
                        case "MUL": acc *= mem[operand]; break;
                        case "DIV": acc /= mem[operand]; break;
                        case "JMP": pc = operand - 1; break;
                        case "JMPN": pc = acc < 0 ? operand - 1 : pc; break;
                        case "JMPZ": pc = acc == 0 ? operand - 1 : pc; break;
                        case "HALT": Console.WriteLine("\nHALT"); goto endwhile;
                    }
                }
                else //err
                {

                }
                pc++;
            }
        endwhile:
            dump();
            return 0;
        }
    }
}
