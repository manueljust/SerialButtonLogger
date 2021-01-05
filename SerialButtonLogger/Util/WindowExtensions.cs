using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace SerialButtonLogger.Util
{
    internal static class WindowExtensions
    {
        private const int GWL_STYLE = -16, WS_MAXIMIZEBOX = 0x10000, WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        extern private static int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        extern private static int SetWindowLong(IntPtr hwnd, int index, int value);

        /// <summary>
        /// Hides the Minimize and Maximize buttons in a Window. Must be called in the constructor.
        /// </summary>
        /// <param name="window">The Window whose Minimize/Maximize buttons will be hidden.</param>
        public static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            window.SourceInitialized += (s, e) => {
                IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

                SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
            };
        }

        /// <summary>
        /// Hides the Minimize button in a Window. Must be called in the constructor.
        /// </summary>
        /// <param name="window">The Window whose Maximize button will be hidden.</param>
        public static void HideMinimizeButton(this Window window)
        {
            window.SourceInitialized += (s, e) => {
                IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                int currentStyle = GetWindowLong(hwnd, GWL_STYLE);

                SetWindowLong(hwnd, GWL_STYLE, currentStyle & ~WS_MINIMIZEBOX);
            };
        }
    }
}