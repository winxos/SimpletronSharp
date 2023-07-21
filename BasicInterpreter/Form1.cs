using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BasicInterpreter
{
    public partial class Form1 : Form
    {
        interpreter it;
        private void button1_Click(object sender, EventArgs e)
        {
            textBox3.Text = "";
            List<string> t = new List<string>();
            string[] st = textBox1.Text.Trim().Split('\n');
            foreach (string s in st)
            {
                if (s[0] == '\r') t.Add("");
                else t.Add(s);
            }
            it = new interpreter(t.ToArray());
            if(!backgroundWorker1.IsBusy)backgroundWorker1.RunWorkerAsync();
            button1.Enabled = false;
        }
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                it.inmsg = textBox2.Text;
                it.flush();
            }
        }
        private void getmsg(string msg)
        {
            textBox3.Text += msg;
            button1.Enabled = true;
        }
        private delegate void refresh(string s); //multi thread, data visited needed delegate.
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (it.doInterpreter())
            {
                this.BeginInvoke(new refresh(getmsg),it.outmsg); //delegate, args
            }
            backgroundWorker1.CancelAsync();
            
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            textBox3.Text += "completed";
        }
    }
}
