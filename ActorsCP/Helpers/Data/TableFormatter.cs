﻿using System;
using System.Data;
using System.Text;

namespace ActorsCP.Helpers.Data
    {
    /// <summary>
    /// Класс предназначен для представления данных таблицы в виде текста
    /// </summary>
    public sealed class TableFormatter
        {
        #region Символы форматирования

        /// <summary>
        /// Пробельный разделитель
        /// </summary>
        private const char SPACE = ' ';

        /// <summary>
        /// Разделитель колонок
        /// </summary>
        private const char COLUMN_SEPARATOR = ' ';

        /// <summary>
        /// Разделитель заголовка и данных
        /// </summary>
        private const char DATA_SEPARATOR = '-';

        #endregion Символы форматирования

        /// <summary>
        /// Данные слева
        /// </summary>
        private readonly bool m_FormatLeft = true;

        /// <summary>
        /// Данные
        /// </summary>
        private readonly DataTable m_Table;

        /// <summary>
        /// Данные
        /// </summary>
        private readonly DataSet m_Tables;

        /// <summary>
        /// Данные в виде строки
        /// </summary>
        private readonly string m_DataString;

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Table">Таблица с данными</param>
        public TableFormatter(DataTable Table)
            {
            m_Table = Table;
            m_DataString = FormatTableAsString(m_Table, m_FormatLeft);
            }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Tables"></param>
        public TableFormatter(DataSet Tables)
            {
            m_Tables = Tables;
            m_DataString = FormatTablesAsString(m_Tables, m_FormatLeft);
            }

        #endregion Конструкторы

        /// <summary>
        /// Пробелы
        /// </summary>
        /// <param name="len">желаемая ширина</param>
        /// <returns></returns>
        private static string Spaces(int len)
            {
            if (len <= 0)
                {
                return string.Empty;
                }
            return new string(SPACE, len);
            }

        /// <summary>
        /// Маркеры разделителя
        /// </summary>
        /// <param name="len">желаемая ширина</param>
        /// <returns></returns>
        private static string Marks(int len)
            {
            return new string(DATA_SEPARATOR, len);
            }

        /// <summary>
        /// Отформатировать значение строки
        /// </summary>
        /// <param name="Value">значение строки</param>
        /// <param name="MaxLength">желаемая ширина</param>
        /// <param name="bFormatLeft">Данные слева</param>
        /// <returns>отформатированная строка</returns>
        private static string Format(string Value, int MaxLength, bool bFormatLeft)
            {
            if (bFormatLeft)
                {
                return Value + Spaces(MaxLength - Value.Length);
                }
            else
                {
                return Spaces(MaxLength - Value.Length) + Value;
                }
            }

        /// <summary>
        /// Создать данные в виде строки-таблицы
        /// </summary>
        /// <param name="Table">таблица</param>
        /// <param name="bFormatLeft">форматировать влево</param>
        /// <returns>данные в виде строки-таблицы</returns>
        private static string FormatTableAsString(DataTable Table, bool bFormatLeft)
            {
            StringBuilder sb = new StringBuilder();

            int[] ColumnsWidth = new int[Table.Columns.Count];

            int TotalWidth = 0;

            #region Подсчет ширины

            // колонки
            for (int i = 0; i < Table.Columns.Count; i++)
                {
                DataColumn Column = Table.Columns[i];
                ColumnsWidth[i] = Column.ColumnName.Length;
                }

            // строки
            for (int j = 0; j < Table.Rows.Count; j++)
                {
                // колонки
                for (int i = 0; i < Table.Columns.Count; i++)
                    {
                    string Value = Table.Rows[j][i].ToString();
                    ColumnsWidth[i] = Math.Max(ColumnsWidth[i], Value.Length);
                    }
                }

            for (int i = 0; i < ColumnsWidth.Length; i++)
                {
                TotalWidth += ColumnsWidth[i];
                }

            TotalWidth += ColumnsWidth.Length - 1;

            #endregion Подсчет ширины

            string TableName = string.Format("Имя таблицы: '{0}'\r\n", Table.TableName);

            string sep = Marks(Math.Max(TotalWidth, TableName.Length)) + "\r\n";
            sb.Append(sep);
            sb.Append(TableName);
            sb.Append(sep);

            #region Заголовок

            for (int i = 0; i < Table.Columns.Count; i++)
                {
                DataColumn Column = Table.Columns[i];
                string Value = Column.ColumnName;

                sb.Append(Format(Value, ColumnsWidth[i], bFormatLeft));
                if (i != (ColumnsWidth.Length - 1))
                    {
                    sb.Append(COLUMN_SEPARATOR);
                    }
                }
            sb.Append("\r\n");

            #endregion Заголовок

            #region Разделитель

            for (int i = 0; i < Table.Columns.Count; i++)
                {
                sb.Append(Marks(ColumnsWidth[i]));
                if (i != (ColumnsWidth.Length - 1))
                    {
                    sb.Append(COLUMN_SEPARATOR);
                    }
                }
            sb.Append("\r\n");

            #endregion Разделитель

            #region Данные

            // строки
            for (int j = 0; j < Table.Rows.Count; j++)
                {
                // колонки
                for (int i = 0; i < Table.Columns.Count; i++)
                    {
                    string Value;
                    if (Table.Rows[j][i] == DBNull.Value)
                        {
                        Value = "NULL";
                        }
                    else
                        {
                        Value = Table.Rows[j][i].ToString();
                        }
                    sb.Append(Format(Value, ColumnsWidth[i], bFormatLeft));

                    if (i != (ColumnsWidth.Length - 1))
                        {
                        sb.Append(COLUMN_SEPARATOR);
                        }
                    }
                sb.Append("\r\n");
                }

            #endregion Данные

            return sb.ToString();
            }

        /// <summary>
        /// Создать данные в виде строки - набора таблиц
        /// </summary>
        /// <param name="Tables">набора таблиц</param>
        /// <param name="bFormatLeft">форматировать влево</param>
        /// <returns>данные в виде строки - набора таблиц</returns>
        private static string FormatTablesAsString(DataSet Tables, bool bFormatLeft)
            {
            StringBuilder sb = new StringBuilder();

            foreach (DataTable t in Tables.Tables)
                {
                string s = FormatTableAsString(t, bFormatLeft);
                sb.Append(s);
                sb.AppendLine();
                }

            return sb.ToString();
            }

        /// <summary>
        /// Вернуть данные таблицы в виде строки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            {
            return m_DataString;
            }
        }
    }