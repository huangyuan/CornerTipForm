using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CornerTipForm;

namespace Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text;
            string content = textBox2.Text;
            int time = Convert.ToInt32(numericUpDown1.Value)*1000;//换算成秒
            // 在右下角弹出气泡提示
            Tips t = new Tips("a","b",300);
            t.Show();
        }
    }
}