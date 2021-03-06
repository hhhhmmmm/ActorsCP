using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.Actors.Events;
using ActorsCP.ViewPorts;

namespace ActorsCP.dotNET.ViewPorts.Rtf
    {
    /// <summary>
    /// Текстовый вьюпорт
    /// </summary>
    public sealed class RtfTextViewPort : FormsViewPortBase
        {
        #region Приватные мемберы

        /// <summary>
        ///
        /// </summary>
        private RichTextBox _control;

        /// <summary>
        /// Нормальный шрифт
        /// </summary>
        private readonly Font NormalFont = new Font("Courier New Cyr", 12);

        /// <summary>
        ///
        /// </summary>
        private readonly Font SmallFont = new Font("Courier New Cyr", 10);

        #endregion Приватные мемберы

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="caption">Заголовок</param>
        /// <param name="icon">Иконка окна</param>
        public RtfTextViewPort(ActorBase actor, string caption, Icon icon = null) : base(actor, caption, icon)
            {
            }

        #endregion Конструкторы

        #region ScrollToBottom

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        /// <summary>
        /// Прокрутить окно до низу
        /// </summary>
        private void ScrollToBottom()
            {
            SendMessage(_control.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
            _control.SelectionStart = _control.Text.Length;
            }

        #endregion ScrollToBottom

        // private object Locker = new object();

        /// <summary>
        /// Добавить текст в контрол
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="color">цвет текста</param>
        /// <param name="font">шрифт</param>
        private void AppendText(string text, Color color, Font font = null)
            {
            if (!_control.IsHandleCreated)
                {
                }

            Invoke(new MethodInvoker(delegate
            {
                if (_control.Handle == IntPtr.Zero)
                    {
                    return;
                    }
                //  lock (Locker)
                //      {
                _control.SelectionStart = _control.TextLength;
                _control.SelectionLength = 0;
                _control.SelectionFont = font ?? NormalFont;
                _control.SelectionColor = color;
                _control.AppendText(text);
                _control.SelectionColor = _control.ForeColor;
                ScrollToBottom();
                //     }
                }));
            }

        #region Перегружаемые методы

        /// <summary>
        /// Создать дочерний элемент управления
        /// </summary>
        /// <param name="rectangle">Элемент - образец</param>
        protected override Control InternalCreateChildControl(Rectangle rectangle)
            {
            _control = new RichTextBox
                {
                Location = new Point(rectangle.X, rectangle.Y),
                Multiline = true,
                Name = "RichTextBox1",
                Size = new Size(rectangle.Width, rectangle.Height),
                TabIndex = 3,
                Font = new Font("Courier New Cyr", 12),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                HideSelection = false,//Hide selection so that AppendText will auto scroll to the end
                BorderStyle = BorderStyle.FixedSingle
                };

            return _control;
            }

        /// <summary>
        /// Обработать как ActorViewPortBoundEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorViewPortBoundEventArgs(ViewPortItem viewPortItem)
            {
            Color color = Color.Orchid;
            var actorEventArgs = viewPortItem.ActorEventArgs as ActorViewPortBoundEventArgs;
            var actor = viewPortItem.Sender;
            var str = actorEventArgs.EventDateAsString + " ";
            if (actorEventArgs.Bound)
                {
                str += $"Объект '{actor.Name}' привязан к вьюпорту";
                }
            else
                {
                str += $"Объект '{actor.Name}' отвязан от вьюпорта";
                }
            AppendText(str + Environment.NewLine, color);
            }

        /// <summary>
        /// Обработать как ActorEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorEventArgs(ViewPortItem viewPortItem)
            {
            Color color = Color.Black;

            string str;
            var actorEventArgs = viewPortItem.ActorEventArgs;
            //var actor = viewPortItem.Sender;

            str = actorEventArgs.EventDateAsString + " ";
            switch (actorEventArgs)
                {
                case ActorExceptionEventArgs exception:
                    {
                    color = Color.Red;
                    str = str + "Исключение: " + exception.Exception.ToString();
                    break;
                    }
                case ActorActionEventArgs action:
                    {
                    #region Тип события

                    switch (action.ActionEventType)
                        {
                        case ActorActionEventType.Debug:
                            {
                            color = Color.Gray;
                            str = str + "Отладка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Error:
                            {
                            color = Color.Red;
                            str = str + "Ошибка: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Exception:
                            {
                            color = Color.Red;
                            str = str + "Исключение: " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Neutral:
                            {
                            color = Color.Green;
                            str = str + " " + action.MessageText;
                            break;
                            }
                        case ActorActionEventType.Warning:
                            {
                            color = Color.Orange;
                            str = str + "Предупреждение: " + action.MessageText;
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
            // int AEA = viewPortItem.ActorEventArgs.AEA;
            // AppendText($" AEA_{AEA}, " + str + Environment.NewLine, color);
            AppendText(str + Environment.NewLine, color);
            }

        /// <summary>
        /// Обработать как ActorStateChangedEventArgs
        /// </summary>
        /// <param name="viewPortItem">Данные</param>
        protected override void InternalProcessAsActorStateChangedEventArgs(ViewPortItem viewPortItem)
            {
            Color color = Color.Blue;

            string str;
            var actorEventArgs = viewPortItem.ActorEventArgs;
            var actor = viewPortItem.Sender;

            str = actorEventArgs.EventDateAsString + " ";

            switch (actorEventArgs)
                {
                case ActorSetCountChangedEventArgs c:
                    {
                    str = str + " " + $"W: {c.WaitingCount} R: {c.RunningCount} C: {c.CompletedCount} + T {c.TotalCount}";
                    break;
                    }
                case ActorStateChangedEventArgs e:
                    {
                    str = str + " " + e.State;
                    break;
                    }
                default:
                    {
                    throw new Exception($"Непонятный тип объекта {actorEventArgs}");
                    }
                }
            // int AEA = viewPortItem.ActorEventArgs.AEA;
            // AppendText($" AEA_{AEA},  {actorEventArgs.EventDateAsString}  stateChanged: '{actor}', событие: {str}" + Environment.NewLine, color, SmallFont);
            AppendText($"{actorEventArgs.EventDateAsString}  stateChanged: '{actor}', событие: {str}" + Environment.NewLine, color, SmallFont);
            }

        #endregion Перегружаемые методы
        }
    }