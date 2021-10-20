using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.Logger;

namespace ActorsCP.dotNET.ViewPorts
    {
    /// <summary>
    /// Форма для представления событий - базовый класс
    /// </summary>
    public partial class FormsViewPortBase : Form
        {
        #region Приватные мемберы

        /// <summary>
        /// Объект для которого показываются события
        /// </summary>
        private readonly ActorBase _actor;

        /// <summary>
        /// Вспомогательный класс для изменения/сохранения/восстановления размера окон
        /// </summary>
        private readonly Resizer _resizer;

        /// <summary>
        /// Контрол в котором происходит отображение информации
        /// </summary>
        private readonly Control _childControl;

        /// <summary>
        /// Базовая строка заголовка
        /// </summary>
        private readonly string _baseCaption;

        #endregion Приватные мемберы

        #region Свойства

        /// <summary>
        /// Логгер
        /// </summary>
        protected IActorLogger Logger
            {
            get
                {
                return GlobalActorLogger.GetInstance();
                }
            }

        /// <summary>
        /// Возвращает объект
        /// </summary>
        public ActorBase Actor
            {
            get
                {
                return _actor;
                }
            }

        /// <summary>
        /// Нормальная иконка
        /// </summary>
        public Icon NormalIcon
            {
            get;
            private set;
            }

        #endregion Свойства

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        #region Конструкторы

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="actor">Объект</param>
        /// <param name="caption">Заголовок</param>
        /// <param name="icon">Иконка окна</param>
        public FormsViewPortBase(ActorBase actor, string caption, Icon icon = null)
            {
            if (actor == null)
                {
                throw new ArgumentNullException($"{nameof(actor)} не может быть null");
                }

            _actor = actor;
            _baseCaption = caption;
            NormalIcon = icon;

            if (NormalIcon == null)
                {
                NormalIcon = Globals.ApplicationIcon;
                }

            InitializeComponent();

            var rectangle = ComputeChildControlSize();
            _childControl = CreateChildControl(rectangle);

            Controls.Add(_childControl);
            _childControl?.Show();

            //while (!_childControl.IsHandleCreated)
            //    {
            //    Thread.Sleep(100);
            //    }

            _resizer = new Resizer(this, "ViewPortBase");

            SetTitle();
            SetActorTimer();

            if (NormalIcon != null)
                {
                InvokeSetIcon(NormalIcon);
                }

            //BeginInvoke(new Action(AfterConsructor));
            }

        #endregion Конструкторы

        //private void AfterConsructor()
        //    {
        //
        //    }

        /// <summary>
        /// Посчитать размеры дочернего контрола
        /// </summary>
        /// <returns></returns>
        private Rectangle ComputeChildControlSize()
            {
            var loc0 = StatisticsLabel.Location;
            var x0_Offset = loc0.X;
            var y0_Offset = 2 * loc0.Y + StatisticsLabel.Height;

            var rectangle = new Rectangle(x0_Offset, y0_Offset, ClientSize.Width - 2 * x0_Offset, ClientSize.Height - y0_Offset);
            return rectangle;
            }

        /// <summary>
        /// Установить заголовок
        /// </summary>
        private void SetTitle()
            {
            var percentCompleted = _lastSavedExecutionStatistics.PercentCompleted;

            #region Отменен

            if (Actor.IsCancellationRequested)
                {
                InvokeSetText(percentCompleted + _baseCaption + " - отменен");
                return;
                }

            #endregion Отменен

            #region Ошибка

            if (Actor.AnErrorOccurred)
                {
                InvokeSetText(percentCompleted + _baseCaption + " - ошибка, время: " + Actor.ExecutionTime.ShortTimeInterval);
                return;
                }

            #endregion Ошибка

            #region Завершен

            if (Actor.IsTerminated)
                {
                string title1;
                if (_lastSavedExecutionStatistics.PercentCompleted100) // завершено строго 100%
                    {
                    title1 = percentCompleted + _baseCaption + " - завершено, время: " + Actor.ExecutionTime.ShortTimeInterval;
                    }
                else
                    {
                    title1 = _baseCaption + " - завершено, время: " + Actor.ExecutionTime.ShortTimeInterval;
                    }

                InvokeSetText(title1);

                return;
                }

            #endregion Завершен

            string title2;
            switch (_actor.State)
                {
                case ActorState.Pending:
                    {
                    title2 = percentCompleted + _baseCaption + " - ожидание";
                    break;
                    }
                case ActorState.Started:
                    {
                    if (!_executionTime.HasStartDate)
                        {
                        _executionTime.SetStartDate();
                        }
                    _executionTime.SetEndDate();
                    title2 = percentCompleted + _baseCaption + " - запущен";
                    break;
                    }
                case ActorState.Running:
                    {
                    if (!_executionTime.HasStartDate)
                        {
                        _executionTime.SetStartDate();
                        }
                    _executionTime.SetEndDate();
                    title2 = percentCompleted + _baseCaption + " - работает, время: " + _executionTime.ShortTimeInterval;
                    break;
                    }
                case ActorState.Stopped:
                    {
                    if (!_executionTime.HasStartDate)
                        {
                        _executionTime.SetStartDate();
                        }
                    _executionTime.SetEndDate();
                    title2 = _baseCaption + " - остановлен";
                    break;
                    }

                default:
                    {
                    InvokeSetText(percentCompleted + _baseCaption);
                    return;
                    }
                }

            InvokeSetText(title2);
            }

        #region Реализация интерфейса IDisposable

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();

                _childControl?.Dispose();
                _actor?.Dispose();

                KillStateTimer();
                }
            base.Dispose(disposing);
            }

        #endregion Реализация интерфейса IDisposable

        #region Обработчики событий формы

        /// <summary>
        /// Вызывается при загрузке формы
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        protected virtual void InternalOnLoad(object sender, EventArgs e)
            {
            }

        /// <summary>
        /// Вызывается при загрузке формы
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
            {
            _resizer.ApplyAndSaveSizeAndPositionFromRegistry();
            SetStatistics(); // OnLoad
            InternalOnLoad(sender, e);
            CreateViewPort();
            }

        /// <summary>
        /// Вызывается при закрытии формы
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        protected virtual void InternalOnFormClosing(object sender, EventArgs e)
            {
            }

        /// <summary>
        /// Вызывается при попытке закрыть форму
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        private void OnFormClosing(object sender, FormClosingEventArgs e)
            {
            if (Actor.State != ActorState.Terminated)
            //if (Actor.State == ActorState.Started || Actor.State == ActorState.Running || Actor.State == ActorState.Stopped)
                {
                Actor.CancelAsync().Wait();
                e.Cancel = true;
                return;
                }

            _resizer.SaveSizeAndPositionInRegistry();
            KillStateTimer();
            TerminateViewPort();
            SetTitle();
            InternalOnFormClosing(sender, e);
            }

        #endregion Обработчики событий формы

        #region Создание контрола отображения

        /// <summary>
        /// Создать дочерний элемент управления
        /// </summary>
        /// <param name="rectangle">Прямоугольник - место для элемента</param>
        private Control CreateChildControl(Rectangle rectangle)
            {
            var control = InternalCreateChildControl(rectangle);
            control.Show();
            control.Update();
            return control;
            }

        /// <summary>
        /// Создать дочерний элемент управления
        /// </summary>
        /// <param name="rectangle">Прямоугольник - место для элемента</param>
        protected virtual Control InternalCreateChildControl(Rectangle rectangle)
            {
            return null;
            }

        #endregion Создание контрола отображения

        #region Функции Invoke

        /// <summary>
        /// Установить иконку окна
        /// </summary>
        /// <param name="icon">Иконка</param>
        private void InvokeSetIcon(Icon icon)
            {
            if (icon == null)
                {
                return;
                }

            if (InvokeRequired)
                {
                Invoke(new MethodInvoker(delegate
                    {
                        Icon = icon;
                        }));
                }
            else
                {
                this.Icon = icon;
                }
            }

        /// <summary>
        /// Вызвать SetText
        /// </summary>
        /// <param name="text"></param>
        private void InvokeSetText(string text)
            {
            if (Text.Equals(text))
                {
                return;
                }
            if (InvokeRequired)
                {
                Invoke(new MethodInvoker(delegate
                    {
                        Text = text;
                        }));
                }
            else
                {
                Text = text;
                }
            }

        #endregion Функции Invoke
        }
    }