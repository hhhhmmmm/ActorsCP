using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using ActorsCP.Actors;

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
        private Resizer _resizer;

        /// <summary>
        /// Контрол в котором происходит отображение информации
        /// </summary>
        private Control _childControl;

        /// <summary>
        /// Базовая строка заголовка
        /// </summary>
        private string _baseCaption;

        #endregion Приватные мемберы

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
                throw new ArgumentNullException(nameof(actor), "actor не может быть null");
                }

            _actor = actor;
            _baseCaption = caption;
            NormalIcon = icon;

            InitializeComponent();

            var rectangle = ComputeChildControlSize();
            _childControl = CreateChildControl(rectangle);

            Controls.Add(_childControl);
            _childControl?.Show();

            _resizer = new Resizer(this, "ViewPortBase");

            SetTitle();
            SetActorTimer();

            if (NormalIcon != null)
                {
                InvokeSetIcon(NormalIcon);
                }
            }

        #endregion Конструкторы

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
            string title = _baseCaption;
            switch (_actor.State)
                {
                case ActorState.Pending:
                    {
                    title = _baseCaption + "- ожидание";
                    break;
                    }
                case ActorState.Running:
                    {
                    title = _baseCaption + "- работает";
                    break;
                    }
                case ActorState.Started:
                    {
                    title = _baseCaption + "- запущен";
                    break;
                    }
                case ActorState.Stopped:
                    {
                    title = _baseCaption + "- остановлен";
                    break;
                    }
                case ActorState.Terminated:
                    {
                    title = _baseCaption + "- завершен";
                    break;
                    }
                }

            if (!Text.Equals(title))
                {
                InvokeSetText(title);
                }
            }

        #region Реализация интерфейса IGuCancellationSource

        /// <summary>
        /// Источник отмены задачи
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Источник отмены выполнения
        /// </summary>
        public CancellationTokenSource TokenSource
            {
            get
                {
                return _cancellationTokenSource;
                }
            }

        #endregion Реализация интерфейса IGuCancellationSource

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

                _cancellationTokenSource?.Dispose();
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
            InternalOnLoad(sender, e);
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
        protected void OnFormClosing(object sender, FormClosingEventArgs e)
            {
            if (Actor.State != ActorState.Terminated)
            //if (Actor.State == ActorState.Started || Actor.State == ActorState.Running || Actor.State == ActorState.Stopped)
                {
                Actor.CancelAsync().Wait();
                e.Cancel = true;
                return;
                }

            //TokenSource.Cancel();
            _resizer.SaveSizeAndPositionInRegistry();
            KillStateTimer();
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
            return InternalCreateChildControl(rectangle);
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

        /// <summary>
        /// Статистика выполнения
        /// </summary>
        //private ExecutionStatistics CurrentExecutionStatistics;

        /// <summary>
        /// Последняя сохраненная статистика выполнения
        /// </summary>
        //private ExecutionStatistics LastSavedExecutionStatistics;

        ///// <summary>
        ///// Мигающая иконка
        ///// </summary>
        //private Icon BlinkingIcon;

        ///// <summary>
        ///// Время выполнения
        ///// </summary>
        //private ActorTime ExecutionTime = default;

        /// <summary>
        /// Актер начал работу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnActorBeginWorking(object sender, EventArgs e)
        //    {
        //    // ExecutionTime.SetStartDate();
        //    SetImageList();
        //    //    SetTitle();
        //    SetIcon();
        //    }

        /// <summary>
        /// Актер завершил выполнение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnActorTerminated(object sender, EventArgs e)
        //    {
        //    //IGuActor impl = (IGuActor)Actor;
        //    //impl.ActorBeginWorking -= OnActorBeginWorking;
        //    //impl.ActorTerminated -= OnActorTerminated;

        //    //this.m_ActionQueue.Enqueue(new ViewPortBaseQueuedAction(QueuedActions.Main_ActorTerminated, sender, e));
        //    }

        /// <summary>
        /// Список изображений который, возможно, будет использоваться для отображения
        /// </summary>
        //protected ImageList TmpTreeImageList
        //    {
        //    get
        //        {
        //        return null;//
        //                    //    TreeImageList;
        //        }
        //    }

        /// <summary>
        /// Прокачать сообщения если поток выполнения - UI
        /// </summary>
        /// <returns>true если поток выполнения - UI</returns>
        //protected static bool DoEvents()
        //    {
        //    //       return MessagePumpHelper.DoEvents();
        //    return true;
        //    }

        /// <summary>
        /// Установить заголовок
        /// </summary>
        //private void SetTitle()
        //    {
        //    string PercentCompleted = this.LastSavedExecutionStatistics.PercentCompleted;

        //    if (MainActorTerminated) // выполнение основного актора завершено
        //        {
        //        if (this.LastSavedExecutionStatistics.PercentCompleted100) // завершено строго 100%
        //            {
        //            InvokeSetText(PercentCompleted + m_BaseCaption + " - завершено, время выполнения: " + ExecutionTime.ShortTimeInterval);
        //            }
        //        else
        //            {
        //            InvokeSetText(m_BaseCaption + " - завершено, время выполнения: " + ExecutionTime.ShortTimeInterval);
        //            }
        //        return;
        //        }

        //    if (m_Actor.IsCanceled)
        //        {
        //        InvokeSetText(PercentCompleted + m_BaseCaption + " - отменен");
        //        return;
        //        }
        //    if (m_Actor.IsInErrorState)
        //        {
        //        InvokeSetText(PercentCompleted + m_BaseCaption + " - ошибка, время выполнения: " + ExecutionTime.ShortTimeInterval);
        //        return;
        //        }
        //    if (m_Actor.IsCompletedSuccessfully)
        //        {
        //        InvokeSetText(PercentCompleted + m_BaseCaption + " - завершено, время выполнения: " + ExecutionTime.ShortTimeInterval);
        //        return;
        //        }

        //    switch (m_Actor.Status)
        //        {
        //        case Actors.GuActorExecutionStatus.Pending:
        //            {
        //            this.Text = PercentCompleted + m_BaseCaption;
        //            break;
        //            }
        //        case Actors.GuActorExecutionStatus.Initialized:
        //        case Actors.GuActorExecutionStatus.Running:
        //            {
        //            if (!ExecutionTime.HasStartDate)
        //                {
        //                ExecutionTime.SetStartDate();
        //                }
        //            ExecutionTime.SetEndDate();
        //            InvokeSetText(PercentCompleted + m_BaseCaption + " - выполняется, время: " + ExecutionTime.ShortTimeInterval);
        //            break;
        //            }

        //        default:
        //            {
        //            InvokeSetText(PercentCompleted + m_BaseCaption);
        //            return;
        //            }
        //        }
        //    }

        #region Добавление/удаление в словарь

        ///// <summary>
        ///// Ожидание при блокировке бага
        ///// </summary>
        //private const int BAG_LOCK_SLEEP = 0;

        ///// <summary>
        ///// Количество попыток доступа к m_Bag
        ///// </summary>
        //private const int BAG_ATTEMPTS = 100;

        ///// <summary>
        ///// Добавить узел в дерево
        ///// </summary>
        ///// <param name="ActorUID">Уникальный идентификатор актора</param>
        ///// <param name="o"></param>
        //protected bool AddToBag(Guid ActorUID, IViewPortBaseObject o)
        //    {
        //    bool bres = false;
        //    for (int i = 0; i < BAG_ATTEMPTS; i++)
        //        {
        //        bres = m_Bag.TryAdd(ActorUID, o);
        //        if (bres)
        //            {
        //            break;
        //            }
        //        Thread.Sleep(BAG_LOCK_SLEEP);
        //        }
        //    return bres;
        //    }

        ///// <summary>
        ///// Найти узел в дереве по объекту Actors.GuActor
        ///// </summary>
        ///// <param name="sender">Отправитель</param>
        ///// <returns></returns>
        //protected IViewPortBaseObject FindInTheBag(object sender)
        //    {
        //    bool bres = false;
        //    IViewPortBaseObject tvan = null;

        //    if (sender == null)
        //        {
        //        return null;
        //        }

        //    GuActor actor = sender as GuActor;
        //    if (actor == null)
        //        {
        //        return null;
        //        }

        //    for (int i = 0; i < BAG_ATTEMPTS; i++)
        //        {
        //        bres = m_Bag.TryGetValue(actor.ActorUID, out tvan);
        //        if (bres)
        //            {
        //            break;
        //            }
        //        if (!m_Bag.IsEmpty)
        //            {
        //            Thread.Sleep(BAG_LOCK_SLEEP);
        //            }
        //        }

        //    // if (!bres) { MessageBox.Show("aa"); }
        //    return bres ? tvan : null;
        //    }

        #endregion Добавление/удаление в словарь

        /// <summary>
        /// Получить дополнительный текст для статистики
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAdditionalStatisticsText()
            {
            return null;
            }

        /// <summary>
        /// Обновить статистику
        /// </summary>
        private void SetStatistics()
            {
            //if (CurrentExecutionStatistics != LastSavedExecutionStatistics)

            //    {
            //    LastSavedExecutionStatistics = CurrentExecutionStatistics;
            //    string Statistics = LastSavedExecutionStatistics.GetStatistics();

            //    string AdditionalStatisticsText = GetAdditionalStatisticsText();
            //    if (!string.IsNullOrEmpty(AdditionalStatisticsText))
            //        {
            //        Statistics = Statistics + ", " + AdditionalStatisticsText;
            //        }

            //    // System.Diagnostics.Debug.WriteLine(Statistics); UpdatingLabel(Statistics);

            //    if (!StatisticsLabel.Text.Equals(Statistics, StringComparison.OrdinalIgnoreCase))
            //        {
            //        StatisticsLabel.Text = Statistics;
            //        }
            //    }
            }
        }
    }