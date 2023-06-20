using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IntegrateApp
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pnlCode.Size = new Size(Width, Height);
            pnlCode.Location = new Point(0, 0);
        }

        [DllImport("user32.dll")]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        const int GWL_STYLE = -16;
        const int WS_SYSMENU = 0x80000;
        const int WS_CAPTION = 0xC00000;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const int WS_SIZEBOX = 0x00040000;

        private Process p;

        private void Form1_Shown(object sender, EventArgs e)
        {
            // open notepad.exe maximized in pnlCode
            p = Process.Start("notepad.exe");
            p.WaitForInputIdle();
            SetParent(p.MainWindowHandle, pnlCode.Handle);
            MoveWindow(p.MainWindowHandle, 0, 0, pnlCode.Width, pnlCode.Height, true);

            // remove buttons in window title
            int style = GetWindowLong(p.MainWindowHandle, GWL_STYLE);
            style = style & ~WS_SYSMENU & ~WS_CAPTION;
            SetWindowLong(p.MainWindowHandle, GWL_STYLE, style);

            // set fixed borders for notepad.exe
            pnlCode.BorderStyle = BorderStyle.FixedSingle;

            // disable resizing of notepad.exe window
            SetWindowLong(p.MainWindowHandle, GWL_STYLE, GetWindowLong(p.MainWindowHandle, GWL_STYLE) & ~WS_SIZEBOX);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pnlCode.Size = new Size(Width, Height);

            // resize notepad.exe
            MoveWindow(p.MainWindowHandle, 0, 0, pnlCode.Width, pnlCode.Height, true);
        }
    }
}
