using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    ///
    /// </summary>
    public sealed class TextViewPort : FormsViewPortBase
        {
        private RichTextBox _control = new RichTextBox();

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="caption">Заголовок</param>
        /// <param name="icon">Иконка окна</param>
        public TextViewPort(ActorBase actor, string caption, Icon icon = null) : base(actor, caption, icon)
            {
            }

        #endregion Конструкторы

        /// <summary>
        /// Создать дочерний элемент управления
        /// </summary>
        /// <param name="rectangle">Элемент - образец</param>
        protected override Control InternalCreateChildControl(Rectangle rectangle)
            {
            _control = new RichTextBox();
            _control.Location = new Point(rectangle.X, rectangle.Y);
            _control.Multiline = true;
            _control.Name = "RichTextBox1";
            _control.Size = new Size(rectangle.Width, rectangle.Height);
            _control.TabIndex = 3;
            _control.Font = new Font("Courier New Cyr", 12);
            _control.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            _control.ScrollBars = RichTextBoxScrollBars.Both;
            _control.WordWrap = false;
            _control.HideSelection = false;//Hide selection so that AppendText will auto scroll to the end
            _control.BorderStyle = BorderStyle.FixedSingle;

            //for (int i = 0; i < 1000; i++)
            //    {
            //    AppendText(i.ToString() + "\r\n");
            //    }

            return _control;
            }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        private void ScrollToBottom()
            {
            SendMessage(_control.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
            _control.SelectionStart = _control.Text.Length;
            }

        private void AppendText(string text)
            {
            _control.AppendText(text);
            ScrollToBottom();
            }

        #region Перегружаемые методы

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            string str = null;
            var actorEventArgs = viewPortItem.ActorEventArgs;
            var actor = viewPortItem.Sender;

            switch (actorEventArgs)
                {
                case ActorExceptionEventArgs exception:
                    {
                    str = "Исключение: " + exception.Exception.ToString();
                    break;
                    }
                case ActorActionEventArgs action:
                    {
                    #region Тип события

                    switch (action.ActionEventType)
                        {
                        case ActorActionEventType.Debug:
                            {
                            str = "Отладка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Error:
                            {
                            str = "Ошибка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Exception:
                            {
                            str = "Исключение: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Neutral:
                            {
                            str = "Текст: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Warning:
                            {
                            str = "Предупреждение: " + action.MessageText;
                            break;
                            }
                        }

                    #endregion Тип события

                    break;
                    }
                default:
                    {
                    throw new Exception($"Непонятный тип объекта {actorEventArgs}");
                    }
                }

            //if (actorEventArgs is ActorActionEventArgs a)
            //    {
            //    str = a.EventDateAsString + ": ";
            //    }
            //else
            //if (actorEventArgs is ActorExceptionEventArgs ex)
            //    {
            //    }
            //else
            //if (actorEventArgs is ActorSetCountChangedEventArgs c)
            //    {
            //    }
            //else
            //if (actorEventArgs is ActorStateChangedEventArgs c2)
            //    {
            //    }
            //// else
            AppendText($"'{actor}', {str}" + Environment.NewLine);
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            var ea = viewPortItem.ActorEventArgs;
            var actor = viewPortItem.Sender;

            AppendText($"Актор: {actor}, событие: {ea}" + Environment.NewLine);
            }

        #endregion Перегружаемые методы
        }
    }