using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ToolKit.Classes
{
    public enum WindowZIndex
    {
        Top = 0,
        Bottom = 1,
        TopMost = -1,
        NoTopMost = -2,
    }

    public static class Operations
    {
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndlnsertAfter, int X, int Y, int cx, int cy, uint Flags);
        public static void ToBottom(this Window window)
        {
            SetWindowPos(new WindowInteropHelper(window).Handle,
                (IntPtr)WindowZIndex.Bottom, (int)window.Left,
                (int)window.Top, (int)window.Width, (int)window.Height, 0);
        }
    }
}
