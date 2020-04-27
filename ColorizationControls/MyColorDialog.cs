using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorizationControls
{
    public class MyColorDialog : ColorDialog
    {

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, uint wFlags);

        private const int WM_INITDIALOG = 0x0110;
        private static readonly IntPtr HWND_TOP = new IntPtr(0);

        private int x = 500;
        private int y = 100;

        public MyColorDialog()
            : base()
        { }

        public void SetPos(Point p)
        {
            x = p.X;
            y = p.Y;
        }

        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            // Defines the constants for Windows messages.
            const int WM_INITDIALOG = 0x0110;

            //uFlag Constants
            const uint SWP_NOSIZE = 0x0001;
            const uint SWP_SHOWWINDOW = 0x0040;
            const uint SWP_NOZORDER = 0x0004;
            const uint UFLAGS = SWP_NOSIZE | SWP_NOZORDER | SWP_SHOWWINDOW;

            //We do the base initialization
            IntPtr hookProc = base.HookProc(hWnd, msg, wparam, lparam);

            //Then we init the dialog
            if (msg == WM_INITDIALOG)
            {
                //We move the position
                SetWindowPos(hWnd, HWND_TOP, x, y, 0, 0, UFLAGS);
            }
            return hookProc;
        }
    }
}
