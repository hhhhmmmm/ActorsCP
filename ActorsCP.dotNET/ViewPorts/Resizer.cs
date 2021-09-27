using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Вспомогательный класс для изменения/сохранения/восстановления размера окон
    /// </summary>
    public class Resizer
        {
        #region Константы

        /// <summary>
        /// Название ключа в реестре для хранения левого края окна
        /// </summary>
        private const string CSZ_X0_POSITION = "x0";

        /// <summary>
        /// Название ключа в реестре для хранения верхнего края окна
        /// </summary>
        private const string CSZ_Y0_POSITION = "y0";

        /// <summary>
        /// Название ключа в реестре для хранения ширины окна
        /// </summary>
        private const string CSZ_WIDTH = "Width";

        /// <summary>
        /// Название ключа в реестре для хранения высоты окна
        /// </summary>
        private const string CSZ_HEIGHT = "Height";

        #endregion Константы

        /// <summary>
        /// Имя файла конфигурации сборки, например GosUslugiProxy.dll.config
        /// </summary>
        private string AssemblyConfigFileName;

        /// <summary>
        /// Полное имя файла конфигурации сборки, например D:\ГИС ЖКХ\CSharp\GosUslugi\GosUslugiApp\bin\Debug\GosUslugiProxy.dll.config
        /// </summary>
        private string FullAssemblyConfigFileName;

        /// <summary>
        /// Имя файла сборки, например GosUslugiProxy.dll
        /// </summary>
        private string AssemblyFileName;

        /// <summary>
        /// Путь к файлу сборки, например D:\ГИС ЖКХ\CSharp\GosUslugi\GosUslugiApp\bin\Debug
        /// </summary>
        private string AssemblyDirectoryName;

        /// <summary>
        /// Уникальный идентификатор экземпляра класса
        /// </summary>
        private readonly string ResizeControlUID;

        /// <summary>
        /// Имя приложения - ключ в рестре
        /// </summary>
        private string ApplicationName;

        /// <summary>
        /// Контролируемая форма
        /// </summary>
        private readonly Form ControlledForm;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="ControlledForm">Контролируемая форма</param>
        /// <param name="ResizeControlUID">Уникальный идентификатор экземпляра класса</param>
        public Resizer(Form ControlledForm, string ResizeControlUID)
            {
            EnableSizeAndPositionApplet = true;

            if (ControlledForm == null)
                {
                throw new ArgumentNullException("ControlledForm", "ControlledForm не может быть null");
                }
            if (string.IsNullOrEmpty(ResizeControlUID))
                {
                throw new ArgumentNullException("ResizeControlUID", "ResizeControlUID не может быть null");
                }

            GetAssemblyNames();
            this.ControlledForm = ControlledForm;
            this.ResizeControlUID = ResizeControlUID;
            }

        #endregion Конструкторы

        #region Импортированные функции

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(HandleRef hWnd, out Rect lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(Int32 uAction, Int32 uParam, ref Rect lpvParam, Int32 fuWinIni);

        private const Int32 SPI_GETWORKAREA = 48;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
            {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
            }

        #endregion Импортированные функции

        #region Свойства

        /// <summary>
        /// Разрешить отслеживание размера и позиции родительского окна
        /// </summary>
        public bool EnableSizeAndPositionApplet
            {
            get;
            set;
            }

        #endregion Свойства

        #region Вспомогательные функции

        /// <summary>
        /// Разобрать названия частей сборки
        /// </summary>
        private void GetAssemblyNames()
            {
            Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
            if (ExecutingAssembly == null)
                {
                throw new InvalidOperationException("Ошибка определения того, где мы находимся");
                }

            string AssemblyLocation = ExecutingAssembly.Location;

            AssemblyFileName = Path.GetFileName(AssemblyLocation);
            AssemblyDirectoryName = Path.GetDirectoryName(AssemblyLocation);
            AssemblyConfigFileName = AssemblyFileName + ".config";
            FullAssemblyConfigFileName = Path.Combine(AssemblyDirectoryName, AssemblyConfigFileName);
            ApplicationName = AssemblyFileName;
            }

        /// <summary>
        /// Получить имя ветки в реестре
        /// </summary>
        /// <returns>сформированное имя ветки</returns>
        private string GetRegistryNameBranch()
            {
            if (string.IsNullOrEmpty(ResizeControlUID))
                {
                throw new ArgumentNullException(nameof(ResizeControlUID), "ResizeControlUID не может быть null");
                }
            if (string.IsNullOrEmpty(ApplicationName))
                {
                throw new ArgumentNullException(nameof(ApplicationName), "ApplicationName не может быть null");
                }

            string s = string.Format("Software\\Inary Technologies\\{0}\\Resize control\\{1}", ApplicationName, ResizeControlUID);
            return s;
            }

        /// <summary>
        /// Получить имя ветки в реестре для хранения размера окна и его положения
        /// </summary>
        /// <returns>сформированное имя ветки</returns>
        private string GetRegistryNameBranchForSizeAndPosition()
            {
            string s = GetRegistryNameBranch();
            return s + "\\SizeAndPosition\\";
            }

        #endregion Вспомогательные функции

        /// <summary>
        /// Сохранить размер и положение окна в реестре
        /// </summary>
        /// <returns>true если все хорошо</returns>
        public bool SaveSizeAndPositionInRegistry()
            {
            if (!EnableSizeAndPositionApplet)
                {
                return false;
                }

            string Branch = GetRegistryNameBranchForSizeAndPosition();
            if (string.IsNullOrEmpty(Branch))
                {
                return false;
                }

            RegistryKey Key = Registry.CurrentUser.CreateSubKey(Branch, true, RegistryOptions.None);
            if (Key == null)
                {
                return false;
                }

            Rect WindowRect;
            if (!GetWindowRect(new HandleRef(ControlledForm, ControlledForm.Handle), out WindowRect))
                {
                return false;
                }

            int X0, Y0, Width, Height;

            X0 = WindowRect.Left;
            Y0 = WindowRect.Top;
            Width = WindowRect.Right - WindowRect.Left;
            Height = WindowRect.Bottom - WindowRect.Top;

            try
                {
                if ((X0 + Width) > 0)
                    {
                    Key.SetValue(CSZ_X0_POSITION, X0);
                    }

                if (Y0 >= 0)
                    {
                    Key.SetValue(CSZ_Y0_POSITION, Y0);
                    }

                if (Width > 0)
                    {
                    Key.SetValue(CSZ_WIDTH, Width);
                    }

                if (Height > 0)
                    {
                    Key.SetValue(CSZ_HEIGHT, Height);
                    }
                }
            catch (Exception)
                {
                return false;
                }

            return true;
            }

        /// <summary>
        /// Применить сохраненные в реестре размер и положение к окну Если онине найдены в реестре -
        /// сохранить их
        /// </summary>
        /// <returns>true если все хорошо</returns>
        public bool ApplyAndSaveSizeAndPositionFromRegistry()
            {
            bool bres;
            bres = ApplySizeAndPositionFromRegistry();
            if (bres)
                {
                return true;
                }

            bres = SaveSizeAndPositionInRegistry();
            return bres;
            }

        /// <summary>
        /// Применить сохраненные в реестре размер и положение к окну
        /// </summary>
        /// <returns>true если все хорошо</returns>
        public bool ApplySizeAndPositionFromRegistry()
            {
            if (!EnableSizeAndPositionApplet)
                {
                return false;
                }

            string Branch = GetRegistryNameBranchForSizeAndPosition();
            if (string.IsNullOrEmpty(Branch))
                {
                return false;
                }

            RegistryKey Key = Registry.CurrentUser.OpenSubKey(Branch, false);
            if (Key == null)
                {
                return false;
                }

            int? dwX0, dwY0, dwWidth, dwHeight;
            int iX0, iY0, iWidth, iHeight;

            dwX0 = (int?)Key.GetValue(CSZ_X0_POSITION);
            if (dwX0 == null)
                {
                return false;
                }

            dwY0 = (int?)Key.GetValue(CSZ_Y0_POSITION);
            if (dwY0 == null)
                {
                return false;
                }

            dwWidth = (int?)Key.GetValue(CSZ_WIDTH);
            if (dwWidth == null)
                {
                return false;
                }

            dwHeight = (int?)Key.GetValue(CSZ_HEIGHT);
            if (dwHeight == null)
                {
                return false;
                }

            iX0 = dwX0 ?? 0;
            iY0 = dwY0 ?? 0;
            iWidth = dwWidth ?? 0;
            iHeight = dwHeight ?? 0;

            int iScreenWidth, iScreenHeight;

            Rect ScreenRect = new Rect();

            SystemParametersInfo(SPI_GETWORKAREA, 0, ref ScreenRect, 0);

            iScreenWidth = ScreenRect.Right - ScreenRect.Left;
            iScreenHeight = ScreenRect.Bottom - ScreenRect.Top;

            if ((iX0 + iWidth) < 0)
                {
                return false;
                }

            if (iX0 >= iScreenWidth)
                {
                return false;
                }

            if ((iY0 + iHeight) < 0)
                {
                return false;
                }

            if (iY0 >= iScreenHeight)
                {
                return false;
                }

            bool bres = MoveWindow(ControlledForm.Handle, iX0, iY0, iWidth, iHeight, false);
            return bres;
            }
        }
    }