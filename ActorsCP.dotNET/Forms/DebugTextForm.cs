using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ActorsCP.Helpers.Data;

namespace ActorsCP.dotNET.Forms
    {
    /// <summary>
    /// Показать отладочное окно с моноширинным текстом
    /// </summary>
    public partial class DebugTextForm : Form
        {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="context">Содержимое окна</param>
        public DebugTextForm(string caption, string context)
            {
            InitializeComponent();
            if (Globals.ApplicationIcon != null)
                {
                this.Icon = Globals.ApplicationIcon;
                }

            if (caption != null)
                {
                this.Text = caption;
                }

            if (context != null)
                {
                this.DataTextBox.Text = context;
                this.DataTextBox.Select(0, 0);
                }

            WordWrapCheckBox.Checked = this.DataTextBox.WordWrap;
            Focus();
            }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWordWrapCheckedChanged(object sender, EventArgs e)
            {
            this.DataTextBox.WordWrap = WordWrapCheckBox.Checked;
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="context">Содержимое окна</param>
        /// <param name="bModal">Модальное окно</param>
        public static void InternalShowText(string caption, string context, bool bModal)
            {
            DebugTextForm dtf = null;

            var t = new Thread(() =>
            {
                Application.Run(dtf = new DebugTextForm(caption, context));
            });
            t.Start();

            if (dtf != null)
                {
                if (bModal)
                    {
                    dtf.ShowDialog();
                    }
                else
                    {
                    dtf.Show();
                    }
                }
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом, содержимое таблицы
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="table">Таблица с данными</param>
        /// <param name="bModal">Модальное окно</param>
        public static void InternalShowTable(string caption, DataTable table, bool bModal)
            {
            if (table == null)
                {
                throw new ArgumentNullException(nameof(table), "Table не может быть null");
                }

            var tf = new TableFormatter(table);
            InternalShowText(caption, tf.ToString(), bModal);
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом, содержимое таблицы
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="tables">Таблицы с данными</param>
        /// <param name="bModal">Модальное окно</param>
        public static void InternalShowDataSet(string caption, DataSet tables, bool bModal)
            {
            if (tables == null)
                {
                throw new ArgumentNullException(nameof(tables), "Tables не может быть null");
                }

            var tf = new TableFormatter(tables);
            InternalShowText(caption, tf.ToString(), bModal);
            }

        #region Функции показа - текст

        /// <summary>
        /// Показать модальное отладочное окно с моноширинным текстом - текст
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="context">Содержимое окна</param>
        public static void ShowModal(string caption, string context)
            {
            InternalShowText(caption, context, true);
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом - текст
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="context">Содержимое окна</param>
        public static void Show(string caption, string context)
            {
            InternalShowText(caption, context, false);
            }

        #endregion Функции показа - текст

        #region Функции показа - таблица

        /// <summary>
        /// Показать модальное отладочное окно с моноширинным текстом - содержимое таблицы
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="table">Таблица с данными</param>
        public static void ShowModal(string caption, DataTable table)
            {
            InternalShowTable(caption, table, true);
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом - содержимое таблицы
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="Table">Таблица с данными</param>
        public static void Show(string caption, DataTable table)
            {
            InternalShowTable(caption, table, false);
            }

        /// <summary>
        /// Показать отладочное окно с моноширинным текстом - содержимое таблиц
        /// </summary>
        /// <param name="caption">Заголовок окна</param>
        /// <param name="tables">Таблицы с данными</param>
        public static void Show(string caption, DataSet tables)
            {
            InternalShowDataSet(caption, tables, false);
            }

        #endregion Функции показа - таблица
        }
    }