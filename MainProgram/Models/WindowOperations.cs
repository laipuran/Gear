using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ProngedGear.Models
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
                (int)window.Top, (int)window.Width, (int)window.Height, 0x0003);
        }
    }
}
