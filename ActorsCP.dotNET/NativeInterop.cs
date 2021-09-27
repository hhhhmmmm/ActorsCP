using System;
using System.Runtime.InteropServices;

namespace ActorsCP.dotNET
    {
    /// <summary>
    /// Вспомогательный класс для вызова функций операционной системы, не включенных в состав .Net Framework
    /// </summary>
    internal class NativeInterop
        {
        private NativeInterop()
            {
            }

        public const int WM_PRINTCLIENT = 0x0318;
        public const int PRF_CLIENT = 0x00000004;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Это Windows XP или выше
        /// </summary>
        public static bool IsWinXpOrAbove
            {
            get
                {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) &&
                    ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
                }
            }

        /// <summary>
        /// Это виста или выше
        /// </summary>
        public static bool IsWinVistaOrAbove
            {
            get
                {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
                }
            }

        internal static readonly int GWL_EXSTYLE = -20;
        internal static readonly int WS_EX_COMPOSITED = 0x02000000;

        [DllImport("user32")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        }
    }