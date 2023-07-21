/*SimpletronX
 *SAL 2  SML
 *winxos 2012/11/05
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SAL2SML
{
    public partial class Form1 : Form
    {

        Dictionary<string, int> langt = new Dictionary<string, int>(); //keywords feature
        Dictionary<string, int> tags = new Dictionary<string, int>(); //store labels
        void loadKeyWords()
        {
            langt.Add("READ", 10);
            langt.Add("WRITE", 11);
            langt.Add("LOAD", 20);
            langt.Add("STORE", 21);
            langt.Add("ADD", 30);
            langt.Add("SUB", 31);
            langt.Add("MUL", 32);
            langt.Add("DIV", 33);
            langt.Add("MOD", 34);
            langt.Add("JMP", 40);
            langt.Add("JMPN", 41);
            langt.Add("JMPZ", 42);
            langt.Add("HALT", 43);
        }
        public Form1()
        {
            InitializeComponent();
        }
        string pretranslate(string s)
        {
            string str = "";
            string[] ls=s.Split('\n');
            for (int i = 0; i < ls.Length; i++) //scan the :label
            {
                if (ls[i].StartsWith(":")) //label
                {
                    string ts = ls[i].Trim();
                    string[] token = ts.Substring(1, ts.Length - 1).Split();
                    if (token[0] != "")
                    {
                        tags[token[0]] = i;
                        if (token.Length == 2) //:x 1, label with value
                        {
                            str += token[1] + "\n";
                        }
                        else
                        {
                            str += "0000\n";
                        } 
                    }
                    continue;
                }
                else if (ls[i] == "") str += "0000\n"; //blank
                else str += ls[i] + "\r\n"; //normal
            }
            foreach (KeyValuePair<string, int> key in tags) //replace label
            {
                str = Regex.Replace(str, " "+key.Key+"\\W",string.Format(" {0:00}",key.Value));
            }
            return str;
        }
        string translate(string s) //to machine language
        {
            string sml = pretranslate(s);
            foreach (KeyValuePair<string, int> key in langt)
            {
                sml = Regex.Replace(sml, key.Key + "\\W", string.Format("{0:00}", key.Value)); //op
            }
            sml = Regex.Replace(sml, "( )+",""); //blank
            sml = Regex.Replace(sml, @"\D43\D", "\n4300\n"); //halt
            return sml;
        }
        void updateLinenum()
        {
            Point pos = new Point(0, 0);
            int firstIndex = this.tb1.GetCharIndexFromPosition(pos);
            int firstLine = this.tb1.GetLineFromCharIndex(firstIndex);
            pos.X += this.tb1.ClientRectangle.Width;
            pos.Y += this.tb1.ClientRectangle.Height;
            int lastIndex = this.tb1.GetCharIndexFromPosition(pos);
            int lastLine = this.tb1.GetLineFromCharIndex(lastIndex);
            label4.Text = "";
            for (int i = firstLine; i <= lastLine+1; i++)
            {
                label4.Text += string.Format("{0:00}\n",i);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loadKeyWords();
            tb1.Focus();
        }

        private void tb1_TextChanged(object sender, EventArgs e)
        {
            updateLinenum();
            int pos = tb1.SelectionStart;
            tb1.Text = tb1.Text.ToUpper();
            tb1.Select(pos, 0);
            string s = translate(tb1.Text);
            string s2 = "";
            string[] ls = s.Split('\n');
            for (int i = 0; i < ls.Length-1; i++)
            {
                s2 += string.Format("{0:00}  {1}\n", i, ls[i]);
            }
            tb2.Text = s2;
            textBox1.Text = s.Replace('\n', ',')+"-1";
        }

        private void tb1_VScroll(object sender, EventArgs e)
        {
            updateLinenum();
        }
    }
}
