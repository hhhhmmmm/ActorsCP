using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ActorsCP.dotNET.ViewPorts.TreeViewViewPort
    {
    /// <summary>
    /// Пользовательский контрол - дерево с ускоренной перерисовкой взято отсюда: http://dev.nomad-net.info/articles/double-buffered-tree-and-list-views
    /// </summary>
    internal sealed partial class FastTreeView : ActorsTreeView
        {
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Конструктор
        /// </summary>
        public FastTreeView()
            {
            InitializeComponent();

            // Enable default double buffering processing
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            }

        /// <summary>
        /// Вспомогательная функция
        /// </summary>
        private void UpdateExtendedStyles()
            {
            int Style = 0;

            if (DoubleBuffered)
                {
                Style |= TVS_EX_DOUBLEBUFFER;
                }

            if (Style != 0)
                {
                NativeInterop.SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)Style);
                }

            int style = NativeInterop.GetWindowLong(this.Handle, NativeInterop.GWL_EXSTYLE);
            style |= NativeInterop.WS_EX_COMPOSITED;
            NativeInterop.SetWindowLong(this.Handle, NativeInterop.GWL_EXSTYLE, style);
            }

        /// <summary>
        /// Перегруженная функция OnHandleCreated. Вызывается один раз при создании дерева
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
            {
            base.OnHandleCreated(e);
            UpdateExtendedStyles();
            }
        }
    }