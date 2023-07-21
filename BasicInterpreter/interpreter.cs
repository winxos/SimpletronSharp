/*
 * Basic Interpreter C# version
 * winxos 2012/11/22
 * NOT FINISHED, BUT ALMOST.
 * USAGE:
 *  interpreter i = new interpreter(string[] codes);
 *  while(i.doIntertrepter())
 *  {
 *      i.outmsg;
 *  }
 * KEYWORDS:DIM CALL FUNCTION FOR IF BREAK ENDIF NEXT RETURN READ PRINT END
 * EXAMPLE:
        DIM A B I F
        FOR I=3 TO 200
        CALL ISP
        IF F=1
        PRINT I
        ENDIF
        NEXT
        END

        FUNCTION ISP I,F
        F=1
        FOR A=2 TO I-1
        IF I%A=0
        F=0
        BREAK
        ENDIF
        NEXT
        RETURN
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicInterpreter
{
    class interpreter
    {
        #region data Definition
        struct tagFor //for label info
        {
            public string varName;
            public int lineNum;
            public string expFrom;
            public string expTo;
        }
        struct tagFunction
        {
            public string name;
            public int calledLine;
            public List<KeyValuePair<string, int>> savedVars;
        }
        string[] sources=null; //raw source code

        Dictionary<string, int> vars = new Dictionary<string, int>(); //variables memory
        Stack<tagFor> forStack = new Stack<tagFor>(); //stored nested for label info
        Stack<tagFunction> functionStack = new Stack<tagFunction>(); //stored nested for label info
        Dictionary<int, int> ifTable = new Dictionary<int, int>(); //if / endif nested match relation
        Dictionary<int, int> forTable = new Dictionary<int, int>(); //for / next nested match relation
        Dictionary<string, int> functionTable = new Dictionary<string, int>(); //function start line

        int codepointer = 0; //current code pointer (trimed)
        string curcmd = ""; //current running code = sources[codepointer]
        List<string> errlogs = new List<string>(); //errors log garage
        string[] errmsgs = new string[] {  //error messages
        "",
        "",
        "",
        ""
        };
        int calledline = 0;
        public string outmsg = ""; //for output
        public string inmsg = "";//for input
        List<string> inlist = new List<string>();
        #endregion
        public interpreter(string[] s)
        {
            sources=s;
            prescan();

        }
        public void flush()
        {
            foreach (string s in inmsg.Split())
            {
                if (s != "")
                {
                    inlist.Add(s);
                }
            }
        }
        public bool doInterpreter() //deal (alter state)
        {
            outmsg = "";
            if (codepointer >= sources.Length) return false;
            curcmd = sources[codepointer].Trim();
            string op = curcmd.Split()[0].ToUpper();
            switch (op)
            {
                case "IF":
                    doif(); break;
                case "FOR":
                    dofor(); break;
                case "NEXT":
                    donext(); break;
                case "ENDIF":
                    doendif(); break;
                case "DIM":
                    dodim(); break;
                case "READ":
                    doread(); break;
                case "PRINT":
                    doprint(); break;
                case "CALL":
                    docall();break;
                case "BREAK":
                    dobreak(); break;
                case "FUNCTION":
                    dofunction();break;
                case "RETURN":
                    doreturn();break;
                case "END":
                    doend(); return false;
                default:
                    string[] ns = curcmd.Split('=');
                    if (ns.Length == 2)
                    {
                        if (vars.ContainsKey(ns[0]))
                        {
                            vars[ns[0]] = Convert.ToInt32(notation.eval(varReplace(ns[1])));
                        }
                    }
                    break;
            }
            Console.Write(outmsg);
            codepointer++;
            return true;
        }
        bool prescan() //scan if / for nested match
        {
            Stack<int> iftmp = new Stack<int>();
            Stack<int> fortmp = new Stack<int>();
            ifTable.Clear();
            forTable.Clear();
            for (int i = 0; i < sources.Length; i++)
            {
                string[] ts = sources[i].Trim().Split();
                string op = ts[0].ToUpper();
                switch (op)
                {
                    case "IF":
                        iftmp.Push(i);
                        break;
                    case "ENDIF":
                        ifTable.Add(iftmp.Pop(), i);
                        break;
                    case "FOR":
                        fortmp.Push(i);
                        break;
                    case "NEXT":
                        forTable.Add(fortmp.Pop(), i);
                        break;
                    case "FUNCTION":
                        functionTable.Add(ts[1].ToUpper(), i);
                        break;
                }
            }

            if (false) //if || for not match
            {
                //return false;
            }
            return true;
        }
        string varReplace(string s) //replace vars before eval expression
        {
            string[] vs = notation.expressionSplit(s);
            string exp = "";
            foreach (string a in vs)
            {
                if (vars.ContainsKey(a)) //replace
                {
                    exp += vars[a].ToString();
                }
                else
                {
                    exp += a;
                }
            }
            return exp;
        }
        #region detail deal method
        private int docall() //call name
        {
            string sub = curcmd.Substring(curcmd.Split()[0].Length).Trim();
            if (!functionTable.ContainsKey(sub)) return -1; //err
            calledline = codepointer;
            codepointer = functionTable[sub]-1;
            
            return 0;
        }
        private int doreturn()
        {
            if (functionStack.Count == 0) return -1; //err
            tagFunction tf = functionStack.Pop();
            codepointer = tf.calledLine;

            foreach (KeyValuePair<string, int> si in tf.savedVars)
            {
                vars[si.Key] = si.Value;
            }
            return 0;
        }
        private int dofunction() //function name in1 in2 in3 , out1
        {
            string sub = curcmd.Substring(curcmd.Split()[0].Length).Trim();
            string[] sp = sub.Split(',');
            tagFunction tf;
            tf.savedVars = new List<KeyValuePair<string, int>>();
            tf.calledLine = calledline;

            string[] ivars = sp[0].Split();
            tf.name = ivars[0];
            for (int i = 1; i < ivars.Length; i++) //in vars
            {
                if (!vars.ContainsKey(ivars[i])) return -1; //err
                tf.savedVars.Add(new KeyValuePair<string,int>(ivars[i], vars[ivars[i]]));
            }
            functionStack.Push(tf);

            if (sp.Length == 2) //with out
            {
                string[] ovars = sp[1].Split();
                foreach (string o in ovars)
                {
                    if (!vars.ContainsKey(o)) //out vars needed dim first
                    {
                        return -1;
                    }
                }
            }
            return 0;
        }
        private int doend() //
        {
            outmsg += "end.";
            return 0;
        }
        private int dobreak()
        {
            tagFor f = forStack.Pop();
            codepointer = forTable[f.lineNum];
            return 0;
        }

        private int getinput()
        {
            while (inlist.Count == 0)
            {
                System.Threading.Thread.Sleep(50);
            }
            int ans = int.Parse(inlist[0]);
            inlist.RemoveAt(0);
            return ans;
        }
        private int doread()
        {
            string[] sub = curcmd.Substring(curcmd.Split()[0].Length).Trim().Split();
            Console.WriteLine("请输入{0}个数据，用空格隔开。", sub.Length);
            foreach (string s in sub)
            {
                if (!vars.ContainsKey(s)) return -1;
                vars[s] = getinput();
            }
            return 0;
        }
        string[] mysplit(string s) //split deal with blank in quotes
        {
            bool quotestart=false;
            string tmp="";
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\"')
                {
                    quotestart = !quotestart;
                }
                if (quotestart && s[i] == ' ')
                {
                    tmp += '\b';
                }
                else tmp += s[i];
            }
            string[] ta = tmp.Split();
            for (int i = 0; i < ta.Length; i++)
            {
                ta[i] = ta[i].Replace('\b', ' ');
            }
            return ta;
        }
        private int doprint() //print x "n" 12*4
        {
            string sub = curcmd.Substring(curcmd.Split()[0].Length).Trim();
            bool isnewline = true;
            if (sub.EndsWith(","))
            {
                sub=sub.Remove(sub.Length - 1);
                isnewline = false;
            }
            foreach (string s in mysplit(sub))
            {
                if (s.StartsWith("\"")) //const string
                {
                    string t=s.Substring(1,s.Length-2);
                    t = t.Replace("\\n", "\n"); //deal \r\n
                    t = t.Replace("\\r", "\r");
                    outmsg +=t;
                }
                else if (vars.ContainsKey(s)) //var
                {
                    outmsg+=vars[s];
                }
                else //expression eval
                {
                    outmsg += notation.eval(varReplace(s));
                }
            }
            if(isnewline)outmsg += "\r\n"; //print xx,  not start a new line.
            return 0;
        }
        private int dodim() //dim a b c
        {
            string[] t = curcmd.Trim().Split();
            for (int i = 1; i < t.Length; i++)
            {
                vars.Add(t[i], 0);
            }
            return 0;
        }

        private int doendif()
        {
            return 0;
        }

        private int donext()
        {
            if (forStack.Count == 0) return -1;
            tagFor f = forStack.Peek();
            if (vars[f.varName] >= notation.eval(varReplace(f.expTo))) //finished
            {
                forStack.Pop();
            }
            else //goto for label
            {
                vars[f.varName]++;
                codepointer = f.lineNum;
            }

            return 0;
        }

        private int dofor() //
        {
            string sub = curcmd.Substring(curcmd.Split()[0].Length).Trim();
            sub = sub.Replace("=", " = ");
            sub = sub.Trim();
            string[] ns = sub.Split();
            if (ns.Length != 5 || ns[1] != "=" || ns[3] != "TO") return -1;
            tagFor f;
            f.lineNum = codepointer;
            f.expFrom = ns[2];
            f.expTo = ns[4];
            f.varName = ns[0];
            forStack.Push(f);
            vars[f.varName] = Convert.ToInt32(notation.eval(varReplace(f.expFrom)));
            if (vars[f.varName] > Convert.ToInt32(notation.eval(varReplace(f.expTo))))
            {
                codepointer = forTable[f.lineNum];
            }
            return 0;
        }

        private int doif() //if a=b   a b may expression
        {
            string sub = curcmd.Substring(curcmd.Split()[0].Length).Trim();// < > = <> >= <=  
            sub = sub.Replace("=", " = ");
            sub = sub.Replace(">", " > ");
            sub = sub.Replace("<", " < ");
            sub = sub.Trim();
            string[] ns = sub.Split();
            bool istrue = false;
            int a, b;
            switch (ns.Length)
            {
                case 3: // < > =
                    a = Convert.ToInt32(notation.eval(varReplace(ns[0])));
                    b = Convert.ToInt32(notation.eval(varReplace(ns[2])));
                    switch (ns[1])
                    {
                        case ">":
                            istrue = a > b;
                            break;
                        case "<":
                            istrue = a < b;
                            break;
                        case "=":
                            istrue = a == b;
                            break;
                        default: return -1;
                    }
                    break;
                case 4:
                    a = Convert.ToInt32(notation.eval(varReplace(ns[0])));
                    b = Convert.ToInt32(notation.eval(varReplace(ns[3])));
                    string stt = ns[1] + ns[2];
                    switch (stt)
                    {
                        case ">=":
                            istrue = a >= b;
                            break;
                        case "<=":
                            istrue = a <= b;
                            break;
                        case "<>":
                            istrue = a != b;
                            break;
                        default: return -1;
                    }
                    break;
                default:
                    return -1;
            }
            if (!istrue)
            {
                codepointer = ifTable[codepointer];
            }
            return 0;
        }
        #endregion
    }
}
