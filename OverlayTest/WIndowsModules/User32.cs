using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Windows.Documents;

namespace OverlayTest.WIndowsModules
{
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;
    using System.Windows.Shapes;

    public class User32
    {
        public static double DpiScalingX = 1.0, DpiScalingY = 1.0;

        [Flags]
        public enum MouseEventFlags : uint
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            Wheel = 0x00000800
        }

        public const int WsExTransparent = 0x00000020;
        public const int WsExToolWindow = 0x00000080;
        private const int GwlExstyle = (-20);
        private const int GwlStyle = -16;
        private const int WsMinimize = 0x20000000;
        private const int WsMaximize = 0x1000000;
        public const int SwRestore = 9;
        public const int SwShow = 5;
        private const int Alt = 0xA4;
        private const int ExtendedKey = 0x1;
        private const int KeyUp = 0x2;
        private static DateTime _lastCheck;
        private static IntPtr _hsWindow;

        

        [DllImport("user32.dll")]
        public static extern IntPtr GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx",
            CharSet = CharSet.Auto)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent,
            IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
            int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowText",
            CharSet = CharSet.Auto)]
        static extern IntPtr GetWindowCaption(IntPtr hwnd,
            StringBuilder lpString, int maxCount);

        [DllImport("user32.dll", EntryPoint = "SendMessage",
            CharSet = CharSet.Auto)]
        static extern int SendMessage3(IntPtr hwndControl, uint Msg,
            int wParam, StringBuilder strBuffer); // get text

        [DllImport("user32.dll", EntryPoint = "SendMessage",
            CharSet = CharSet.Auto)]
        static extern int SendMessage4(IntPtr hwndControl, uint Msg,
            int wParam, int lParam);  // text length

        public static void SetWindowExStyle(IntPtr hwnd, int style) => SetWindowLong(hwnd, GwlExstyle, GetWindowLong(hwnd, GwlExstyle) | style);

        //public static bool IsHearthstoneInForeground() => GetForegroundWindow() == GetWindowHandle();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpPoint);

        const UInt32 WM_GETTEXT = 0x000D;
        const UInt32 WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hWnd, string text);

        public static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate string length 
            int length = (int)SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            StringBuilder sb = new StringBuilder(length + 1);
            // Get window text
            SendMessage(hwnd, WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }

        public static Point GetMousePos()
        {
            MousePoint p;
            GetCursorPos(out p);
            return new Point(p.X, p.Y);
        }
        public static int GetTextBoxTextLength(IntPtr hTextBox)
        {
            // helper for GetTextBoxText
            uint WM_GETTEXTLENGTH = 0x000E;
            int result = SendMessage4(hTextBox, WM_GETTEXTLENGTH,
                0, 0);
            return result;
        }


        public static string GetTextBoxText(IntPtr hTextBox)
        {
            uint WM_GETTEXT = 0x000D;
            int len = GetTextBoxTextLength(hTextBox);
            if (len <= 0) return null;  // no text
            StringBuilder sb = new StringBuilder(len + 1);
            SendMessage3(hTextBox, WM_GETTEXT, len + 1, sb);
            return sb.ToString();
        }

        public static WindowState GetWindowState(IntPtr hsWindows)
        {
            var hsWindow = hsWindows;
            var state = GetWindowLong(hsWindow, GwlStyle);
            if ((state & WsMaximize) == WsMaximize)
                return WindowState.Maximized;
            if ((state & WsMinimize) == WsMinimize)
                return WindowState.Minimized;
            return WindowState.Normal;
        }

        public static bool WindowViible(IntPtr windowHandle)
        {
            return IsWindowVisible(windowHandle);
        }

        public static void SetWindowOwner(IntPtr windowHandle)
        {

            Application.Current.Dispatcher.Invoke(new Action(() =>
            { new WindowInteropHelper(Application.Current.MainWindow).Owner = windowHandle; }));
        }

        public static string GetWindowText(IntPtr hwind)
        {
           StringBuilder sb = new StringBuilder(256);
            GetWindowCaption(hwind, sb, 256);
            return sb.ToString();


           
        }
        public static List<IntPtr> GetAllChildrenWindowHandles(IntPtr hParent,
            int maxCount)
        {
            List<IntPtr> result = new List<IntPtr>();
            int ct = 0;
            IntPtr prevChild = IntPtr.Zero;
            IntPtr currChild = IntPtr.Zero;
            while (true && ct < maxCount)
            {
                currChild = FindWindowEx(hParent, prevChild, null, null);
                if (currChild == IntPtr.Zero) break;
                result.Add(currChild);
                prevChild = currChild;
                ++ct;
            }
            return result;
        }

        public static IntPtr? GetWindowHandle(string Name)
        {

            Process[] processes = Process.GetProcessesByName(Name);
            if (processes.Length == 0)
            {
                return new IntPtr?();
            }
            return processes.FirstOrDefault().MainWindowHandle;
            //if (DateTime.Now - _lastCheck < new TimeSpan(0, 0, 5) && _hsWindow == IntPtr.Zero)
            //    return _hsWindow;
            //if (_hsWindow != IntPtr.Zero && IsWindow(_hsWindow))
            //    return _hsWindow;
            //if (false)
            //{
            //    foreach (var process in Process.GetProcesses())
            //    {
            //        var sb = new StringBuilder(200);
            //        GetClassName(process.MainWindowHandle, sb, 200);
            //        if (!sb.ToString().Equals("UnityWndClass", StringComparison.InvariantCultureIgnoreCase))
            //            continue;
            //        _hsWindow = process.MainWindowHandle;
            //        _lastCheck = DateTime.Now;
            //        return _hsWindow;
            //    }

            //    _lastCheck = DateTime.Now;
            //    return IntPtr.Zero;
            //}
            //_hsWindow = FindWindow("Notepad","Untitled - Notepad");//Config.Instance.HearthstoneWindowName);
            //if (_hsWindow != IntPtr.Zero)
            //    return _hsWindow;
            //foreach (var windowName in WindowNames)
            //{
            //    _hsWindow = FindWindow("UnityWndClass", windowName);
            //    if (_hsWindow == IntPtr.Zero)
            //        continue;

            //    break;
            //}
            //_lastCheck = DateTime.Now;
            //return _hsWindow;
        }

        public static Process GetWindowProcessId()
        {
            if (_hsWindow == IntPtr.Zero)
                return null;
            try
            {
                uint procId;
                GetWindowThreadProcessId(_hsWindow, out procId);
                return Process.GetProcessById((int)procId);
            }
            catch
            {
                return null;
            }
        }

        public static Rect GetWindowRect(IntPtr WindowHandle, bool dpiScaling = true)
        {
            // Returns the co-ordinates of Hearthstone's client area in screen co-ordinates
            var hsHandle = WindowHandle;
            var rect = new Rect();
            var ptUL = new Point();
            var ptLR = new Point();

            GetClientRect(hsHandle, ref rect);

            ptUL.X = rect.left;
            ptUL.Y = rect.top;

            ptLR.X = rect.right;
            ptLR.Y = rect.bottom;

            ClientToScreen(hsHandle, ref ptUL);
            ClientToScreen(hsHandle, ref ptLR);

            if (dpiScaling)
            {
                ptUL.X = (int)(ptUL.X / DpiScalingX);
                ptUL.Y = (int)(ptUL.Y / DpiScalingY);
                ptLR.X = (int)(ptLR.X / DpiScalingX);
                ptLR.Y = (int)(ptLR.Y / DpiScalingY);
            }

            return new Rect() { bottom = ptUL.X, left = ptUL.Y, right = ptLR.X - ptUL.X, top = ptLR.Y - ptUL.Y};
        }

        public static void BringWindowToForeground(IntPtr windowHandle)
        {
            var hsHandle = windowHandle;
            if (hsHandle == IntPtr.Zero)
                return;
            ActivateWindow(hsHandle);
            SetForegroundWindow(hsHandle);
        }

        //public static void FlashHs() => FlashWindow(GetWindowHandle(), false);

        //http://www.roelvanlisdonk.nl/?p=4032
        public static void ActivateWindow(IntPtr mainWindowHandle)
        {
            // Guard: check if window already has focus.
            if (mainWindowHandle == GetForegroundWindow())
                return;

            // Show window maximized.
            ShowWindow(mainWindowHandle, GetWindowState(mainWindowHandle) == WindowState.Minimized ? SwRestore : SwShow);

            // Simulate an "ALT" key press.
            keybd_event(Alt, 0x45, ExtendedKey | 0, 0);

            // Simulate an "ALT" key release.
            keybd_event(Alt, 0x45, ExtendedKey | KeyUp, 0);

            // Show window in forground.
            SetForegroundWindow(mainWindowHandle);
        }


        //http://joelabrahamsson.com/detecting-mouse-and-keyboard-input-with-net/
        public class MouseInput : IDisposable
        {
            private const int WH_MOUSE_LL = 14;
            private const int WM_LBUTTONDOWN = 0x201;
            private const int WM_LBUTTONUP = 0x0202;
            private readonly WindowsHookHelper.HookDelegate _mouseDelegate;
            private readonly IntPtr _mouseHandle;
            private bool _disposed;

            public MouseInput()
            {
                _mouseDelegate = MouseHookDelegate; //crashes application if directly used for some reason
                _mouseHandle = WindowsHookHelper.SetWindowsHookEx(WH_MOUSE_LL, _mouseDelegate, IntPtr.Zero, 0);
            }

            public void Dispose() => Dispose(true);

            public event EventHandler<EventArgs> LmbDown;
            public event EventHandler<EventArgs> LmbUp;
            public event EventHandler<EventArgs> MouseMoved;

            private IntPtr MouseHookDelegate(int code, IntPtr wParam, IntPtr lParam)
            {
                if (code < 0)
                    return WindowsHookHelper.CallNextHookEx(_mouseHandle, code, wParam, lParam);


                switch (wParam.ToInt32())
                {
                    case WM_LBUTTONDOWN:
                        LmbDown?.Invoke(this, new EventArgs());
                        break;
                    case WM_LBUTTONUP:
                        LmbUp?.Invoke(this, new EventArgs());
                        break;
                    default:
                        MouseMoved?.Invoke(this, new EventArgs());
                        break;
                }

                return WindowsHookHelper.CallNextHookEx(_mouseHandle, code, wParam, lParam);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                    return;
                if (_mouseHandle != IntPtr.Zero)
                    WindowsHookHelper.UnhookWindowsHookEx(_mouseHandle);
                _disposed = true;
            }

            ~MouseInput()
            {
                Dispose(false);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MousePoint
        {
            public readonly int X;
            public readonly int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public double left;
            public double top;
            public double right;
            public double bottom;
        }

        public class WindowsHookHelper
        {
            public delegate IntPtr HookDelegate(int code, IntPtr wParam, IntPtr lParam);

            [DllImport("User32.dll")]
            public static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("User32.dll")]
            public static extern IntPtr UnhookWindowsHookEx(IntPtr hHook);

            [DllImport("User32.dll")]
            public static extern IntPtr SetWindowsHookEx(int idHook, HookDelegate lpfn, IntPtr hmod, int dwThreadId);
        }
    }
}