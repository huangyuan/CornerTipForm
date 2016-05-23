using System;
using System.Drawing;
using System.Windows.Forms;

namespace CornerTipForm
{
    /// <summary>
    /// 在右下角弹出提示框[当前为测试版本，以后推出更换皮肤和其他功能]
    /// </summary>
    public partial class CornerTips : PerPixelAlphaForm
    {

        // 上一个实例，在运行当前实例前，要判断一下上一个实例是否存在，存在的话，则要退出其。
        static CornerTips _PrevInstance = null;

        Timer _MoveTimer = new Timer();
        Timer _ExitTimer = new Timer();
        int _ShowTime = 6000;
        int _X;
        int _Y;
        int _TaskHeight = 0;
        bool _Up = true;
        Bitmap _Bmp = new Bitmap(Resource.ts_bg);
        PictureBox _CloseButton = new PictureBox();
        private Label label1;
        Font TipFont = new Font("宋体", 9F);

        /// <summary>
        /// 构造一个提示框
        /// </summary>
        /// <param name="title">提示框标题</param>
        /// <param name="message">提示框文本</param>
        /// <param name="time">自动退出时间</param>
        public CornerTips(string title, string message, int time)
        {
            // Form 的基本参数
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;

            // 强制退出上一实例
            if (_PrevInstance != null)
            {
                _PrevInstance.AnimatedExit();
                _PrevInstance = null;
            }
            // 注册当前为上一实例
            _PrevInstance = this;

            // 增加关闭控件
            _CloseButton.Size = Resource.ts_close.Size;
            _CloseButton.Location = new Point(186, 1);
            _CloseButton.Image = Resource.ts_close;
            _CloseButton.Click += new EventHandler(Close_Clicked);
            _CloseButton.MouseEnter += new EventHandler(CloseButton_MouseEnter);
            _CloseButton.MouseLeave += new EventHandler(CloseButton_MouseLeave);
          //  Controls.Add(_CloseButton);

            this.label1 = new System.Windows.Forms.Label();

            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
            Controls.Add(label1);

            // 显示时间
            _ShowTime = time;

            // 画图并设窗体图像
            //Graphics g = Graphics.FromImage(_Bmp);
            //Rectangle rect = new Rectangle(17, 5, 160, 22);
            //StringFormat sf = new StringFormat();
            //sf.Alignment = StringAlignment.Center;
            //sf.LineAlignment = StringAlignment.Center;
            //g.DrawString(title, new Font(TipFont, FontStyle.Bold), new SolidBrush(Color.Black), rect, sf);
            //rect = new Rectangle(67, 68, 165, 75);
            //sf.Alignment = StringAlignment.Near;
            //sf.LineAlignment = StringAlignment.Near;
            //g.DrawString(message, TipFont, new SolidBrush(Color.Black), rect, sf);
            //g.DrawImage(_CloseButton.Image, _CloseButton.Left, _CloseButton.Top, _CloseButton.Width, _CloseButton.Height);
            //this.Width = 400;
            //this.SetBitmap(_Bmp, 222);

            // 计算X,Y超始值
            Rectangle rt = Screen.PrimaryScreen.Bounds;
            _TaskHeight = rt.Height - Screen.PrimaryScreen.WorkingArea.Height;
            this.Location = new Point(rt.Right, rt.Bottom);
            _X = rt.Right - this.Width;
            _Y = rt.Bottom;

            // 上移开坮
            _Up = true;
            _MoveTimer.Interval = 25;
            _MoveTimer.Tick += new EventHandler(MoveTimer_Tick);
            _MoveTimer.Start();
        }

        private void CloseButton_MouseLeave(object sender, EventArgs e)
        {
            _CloseButton.Cursor = Cursors.Arrow;
        }

        private void CloseButton_MouseEnter(object sender, EventArgs e)
        {
            _CloseButton.Cursor = Cursors.Hand;
        }

        private void Close_Clicked(object sender, EventArgs e)
        {
            AnimatedExit();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            if (_Up)
            {
                // 上升
                _Y -= 10;
                if (_Y + this.Height + _TaskHeight <= Screen.PrimaryScreen.Bounds.Height)
                {
                    // 上升级束
                    Location = new Point(_X, Screen.PrimaryScreen.Bounds.Height - Height - _TaskHeight);
                    _MoveTimer.Stop();

                    // 退出定时器开始
                    _ExitTimer.Interval = _ShowTime;
                    _ExitTimer.Tick += new EventHandler(ExitTimer_Tick);
                    _ExitTimer.Start();
                }
                else
                {
                    Location = new Point(_X, _Y);
                }
            }
            else
            {
                // 下降速度快点
                _Y += 20;
                Location = new Point(_X, _Y);
                if (_Y >= Screen.PrimaryScreen.Bounds.Height)
                {
                    _MoveTimer.Stop();
                    Close();
                }
            }
        }

        private void ExitTimer_Tick(object sender, EventArgs e)
        {
            AnimatedExit();
        }

        public void AnimatedExit()
        {
            // 注销上一实例
            _PrevInstance = null;

            _ExitTimer.Stop();
            _Up = false;
            _MoveTimer.Start();
        }

        private void InitializeComponent()
        {//
         //   this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            //this.label1.AutoSize = true;
            //this.label1.Location = new System.Drawing.Point(53, 89);
            //this.label1.Name = "label1";
            //this.label1.Size = new System.Drawing.Size(191, 12);
            //this.label1.TabIndex = 0;
            //this.label1.Text = "DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD";
            // 
            // CornerTips
            // 
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.label1);
            this.Name = "CornerTips";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}