/*
 * infix to postfix notation &
 * postfix calculate.
 * winxos 2012/11/22
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BasicInterpreter
{
    public static class notation
    {
        static Dictionary<string, int> opPrecedence = new Dictionary<string, int>
        {
            {"+",1},{"-",1},
            {"*",2},{"/",2},{"%",2},
            {"(",3},{")",3}
        };

        /// <summary>
        /// split the expression to notation
        /// </summary>
        /// <param name="s"></param>
        /// <returns>splitted notations</returns>
        public static string[] expressionSplit(string s)
        {
            s = s.Replace("+", " + ");
            s = s.Replace("-", " - ");
            s = s.Replace("*", " *  ");
            s = s.Replace("/", " / ");
            s = s.Replace("%", " % ");
            s = s.Replace("(", " ( ");
            s = s.Replace(")", " ) ");
            s = Regex.Replace(s, "( )+", " ");
            s = s.Trim();
            return s.Split();
        }
        /// <summary>
        /// calculate infix notation
        /// </summary>
        /// <param name="e">infix string</param>
        /// <returns>double value</returns>
        public static double eval(string e)
        {
            string[] postfix;
            shuntingYard(expressionSplit(e), out postfix);
            return calcPostfix(postfix);
        }
        /// <summary>
        /// infix to postfix
        /// </summary>
        /// <param name="n">infix</param>
        /// <param name="ans">out postfix</param>
        /// <returns>success state</returns>
        public static bool shuntingYard(string[] n, out string[] ans)
        {
            Stack<string> ops = new Stack<string>(); //operators stack
            List<string> ls = new List<string>(); //out stack

            foreach (string s in n)
            {
                if (isOp(s)) // is operator
                {
                    if (s == "(")
                    {
                        ops.Push(s);
                    }
                    else if (s == ")") //pop until meet '('
                    {
                        while (ops.Peek() != "(")
                        {
                            ls.Add(ops.Pop());
                        }
                        ops.Pop();
                    }
                    else //normal operators, judge its precedence
                    {

                        while (true)
                        {
                            if (ops.Count == 0 ||
                                ops.Peek() == "(" ||
                                opPrecedence[ops.Peek()] < opPrecedence[s])
                            {
                                ops.Push(s);
                                break;
                            }
                            else
                            {
                                ls.Add(ops.Pop());
                            }
                        }
                    }
                }
                else //is operand(number)
                {
                    ls.Add(s);
                }
            }
            while (ops.Count != 0) ls.Add(ops.Pop()); //pop remainer
            ans = ls.ToArray();
            return true;
        }
        static double calcPostfix(string[] n)
        {
            Stack<double> ans = new Stack<double>();
            foreach (string s in n)
            {
                if (!isOp(s))
                {
                    ans.Push(int.Parse(s));
                }
                else
                {
                    double b = ans.Pop();
                    double a = ans.Pop();
                    switch (s)
                    {
                        case "+": ans.Push(a + b); break;
                        case "-": ans.Push(a - b); break;
                        case "*": ans.Push(a * b); break;
                        case "/": ans.Push(a / b); break;
                        case "%": ans.Push(a % b); break;
                    }
                }
            }
            return ans.Pop();
        }
        static bool isOp(string c)
        {
            return opPrecedence.ContainsKey(c);
        }
    }
}
