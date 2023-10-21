using APP_POSITION.Config;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace APP_POSITION.Positioning
{
    internal class WindowHandling
    {
        #region --- Win32 API ---
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", EntryPoint = "GetWindowPos")]
        public static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        #endregion --- Win32 API ---

        private readonly AppConfig appConfiguration;
        private readonly string machineName;

        public WindowHandling(AppConfig appConfiguration, string machineName)
        {
            this.appConfiguration = appConfiguration;
            this.machineName = machineName;
        }

        private void RestoreWindowPosition(IntPtr windowPtr, WindowPosition windowPosition)
        {
            Rect parentWindowRectangle = new Rect()
            {
                Top = Convert.ToInt16(windowPosition.Top),
                Left = Convert.ToInt16(windowPosition.Left),
                Right = Convert.ToInt16(windowPosition.Width),
                Bottom = Convert.ToInt16(windowPosition.Height),
            };

            // int X, int Y, int nWidth, int nHeight
            MoveWindow(windowPtr,
                       parentWindowRectangle.Left, parentWindowRectangle.Top,
                       parentWindowRectangle.Right, parentWindowRectangle.Bottom,
                       true);
        }

        private void SaveWindowPosition(IntPtr windowPtr, WindowPosition windowPosition)
        {
            Rect parentWindowRectangle = new Rect();
            GetWindowRect(windowPtr, ref parentWindowRectangle);

            windowPosition.Top = Convert.ToString(parentWindowRectangle.Top);
            windowPosition.Left = Convert.ToString(parentWindowRectangle.Left);
            windowPosition.Height = Convert.ToString(parentWindowRectangle.Bottom - parentWindowRectangle.Top);
            windowPosition.Width = Convert.ToString(parentWindowRectangle.Right - parentWindowRectangle.Left);

            WindowSettings.AppSettingsUpdate(appConfiguration);
        }

        public bool SetWindowPositioning()
        {
            // Main Window
            //RestoreWindowPosition(GetConsoleWindow(),
            //    appConfiguration.Application.WindowPosition.Top,
            //    appConfiguration.Application.WindowPosition.Left,
            //    appConfiguration.Application.WindowPosition.Width,
            //    appConfiguration.Application.WindowPosition.Height);

            bool escapeKeyPressed = false;

            Process[] localAll = Process.GetProcesses();

            foreach (ProcessesList processItem in appConfiguration.Processes)
            {
                Process[] processes = Process.GetProcessesByName(processItem.Name, machineName);

                foreach (Process process in processes)
                {
                    IntPtr windowHandle = process.MainWindowHandle;
                    if (windowHandle != IntPtr.Zero) 
                    {
                        RestoreWindowPosition(windowHandle, processItem.WindowPosition);
                    }
                }
            }

            return escapeKeyPressed;
        }

        public void StoreWindowPositioning()
        {
            // Main Window
            //SaveWindowPosition(GetConsoleWindow(), appConfiguration.Application.WindowPosition);

            Process[] localAll = Process.GetProcesses();

            foreach (ProcessesList processItem in appConfiguration.Processes)
            {
                Process[] processes = Process.GetProcessesByName(processItem.Name, machineName);

                foreach (Process process in processes)
                {
                    IntPtr windowHandle = process.MainWindowHandle;
                    if (windowHandle != IntPtr.Zero)
                    {
                        SaveWindowPosition(windowHandle, processItem.WindowPosition);
                    }
                }
            }
        }
    }
}
