using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace CornerTipForm
{
    public partial class Tips : Form
    {
        static Tips _PrevInstance = null;

        int _ShowTime = 6000;
        int _X;
        int _Y;
        int _TaskHeight = 0;
        bool _Up = true;

        public Tips()
        {
            InitializeComponent();
            this.TopMost = true;

            this.ShowIcon = 
            this.MaximizeBox=
            this.MinimizeBox=
            this.ShowInTaskbar = false;           
            this.StartPosition = FormStartPosition.Manual;

        }   /// <summary>
        /// 构造一个提示框
        /// </summary>
        /// <param name="title">提示框标题</param>
        /// <param name="message">提示框文本</param>
        /// <param name="time">自动退出时间</param>
        public Tips(string title, string message, int time)     
            : this()
        {
            Text = title;
            label1.Text = message;
            _ShowTime =time<=0?3000: time;

            // 强制退出上一实例
            if (_PrevInstance != null)
            {
                _PrevInstance.Close();
                _PrevInstance = null;
            }
            // 注册当前为上一实例
            _PrevInstance = this;

            Rectangle rt = Screen.PrimaryScreen.Bounds;
            _TaskHeight = rt.Height - Screen.PrimaryScreen.WorkingArea.Height;
            this.Location = new Point(rt.Right, rt.Bottom);
            _X = rt.Right - this.Width;
            _Y = rt.Bottom;

            // 上移开坮
            _Up = true;
            _ExitTimer.Enabled = true;
            _MoveTimer.Enabled = true;
            _MoveTimer.Interval = 25;
            _MoveTimer.Tick += new EventHandler(MoveTimer_Tick);
            _MoveTimer.Start();
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



        /// <summary>
        /// 是否允许绘制图片
        /// add by kaicui 2010-12-16 10:10:07 
        /// </summary>
        public bool IsEnableSetBitMap = true;

        /// <para>Changes the current bitmap.</para>
        public void SetBitmap(Bitmap bitmap)
        {
            if (!IsEnableSetBitMap)
            {
                return;
            }
            SetBitmap(bitmap, 255);

        }


        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = Win32.SelectObject(memDc, hBitmap);

                Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point pointSource = new Win32.Point(0, 0);
                Win32.Point topPos = new Win32.Point(Left, Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;

                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
        //        return cp;
        //    }
        //}

    }
    class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        public const Int32 ULW_COLORKEY = 0x00000001;
        public const Int32 ULW_ALPHA = 0x00000002;
        public const Int32 ULW_OPAQUE = 0x00000004;

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);
    }
}
