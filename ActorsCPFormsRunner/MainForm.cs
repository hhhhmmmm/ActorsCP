using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ActorsCP.Actors;
using ActorsCP.dotNET.Forms;
using ActorsCP.dotNET.ViewPorts;
using ActorsCP.dotNET.ViewPorts.Rtf;
using ActorsCP.dotNET.ViewPorts.Tree;
using ActorsCP.Helpers;
using ActorsCP.Tests.TestActors;

namespace ActorsCPFormsRunner
    {
    public partial class MainForm : Form
        {
        /// <summary>
        /// Вспомогательный класс для изменения/сохранения/восстановления размера окон
        /// </summary>
        private readonly Resizer _resizer;

        public MainForm()
            {
            _resizer = new Resizer(this, "MainForm");

            InitializeComponent();
            this.Load += OnLoad;
            this.FormClosing += OnFormClosing;

            OneItem[] items = new OneItem[] {
                new OneItem("Дерево", "TreeViewPort"),
                new OneItem("RTF текст","RtfTextViewPort")
            };

            ViewportTypeComboBox.DataSource = items;
            }

        /// <summary>
        /// Последний актор
        /// </summary>
        public ActorBase CreatedActor
            {
            get;
            set;
            }

        #region Вспомогательные методы

        /// <summary>
        /// Опции запуска
        /// </summary>
        /// <returns></returns>
        private RunOptions GetRunOptions()
            {
            var options = new RunOptions
                {
                sleepTime = int.Parse(SleepTimeTextBox.Text),
                QueueLegth = int.Parse(QueueLegthTextBox.Text),
                CrowdLength = int.Parse(CrowdLengthTextBox.Text)
                };
            return options;
            }

        /// <summary>
        /// Создать вьюпорт
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private void CreateAndShowViewPort(ActorBase actor, string name)
            {
            var item = (OneItem)ViewportTypeComboBox.SelectedItem;

            switch (item.Value)
                {
                case "RtfTextViewPort":
                    {
                    var form = new RtfTextViewPort(actor, name, MainProgram.MainIcon);
                    form.Show();
                    break;
                    }
                case "TreeViewPort":
                    {
                    var form = new TreeViewPort(actor, name, MainProgram.MainIcon);
                    form.Show();
                    break;
                    }
                default:
                    {
                    throw new Exception($"{item.Value} - ?");
                    }
                }
            }

        #endregion Вспомогательные методы

        /// <summary>
        /// Вызывается при загрузке формы
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
            {
            _resizer.ApplyAndSaveSizeAndPositionFromRegistry();
            OnQueueTestActor_Click(this, EventArgs.Empty);
            RunActorButtonClick(this, EventArgs.Empty);
            // Application.Exit();
            }

        /// <summary>
        /// Вызывается при попытке закрыть форму
        /// </summary>
        /// <param name="sender">Отправитель</param>
        /// <param name="e"></param>
        private void OnFormClosing(object sender, FormClosingEventArgs e)
            {
            _resizer.SaveSizeAndPositionInRegistry();
            }

        /// <summary>
        /// TestActor_Single
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSingleTestActorClick(object sender, EventArgs e)
            {
            var actor = new SimpleActor();
            //actor.SetVerbosity(ActorVerbosity.Off);
            CreatedActor = actor;
            CreateAndShowViewPort(actor, "Одиночка");
            }

        /// <summary>
        /// TestActor_Queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueTestActor_Click(object sender, EventArgs e)
            {
            var runOptions = GetRunOptions();

            var queue = new ActorsQueue("Очередь акторов");
            for (int i = 0; i < runOptions.QueueLegth; i++)
                {
                var actor = new SimpleActor();
                //actor.Interval = runOptions.sleepTime;
                queue.Add(actor);
                }
            CreatedActor = queue;

            CreateAndShowViewPort(queue, "Очередь акторов");
            }

        /// <summary>
        /// TestActor_QueueOfQueues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCombinedQueueActorClick(object sender, EventArgs e)
            {
            }

        /// <summary>
        /// TestActor_Crowd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCrowdTestActorClick(object sender, EventArgs e)
            {
            var runOptions = GetRunOptions();

            var crowd = new ActorsCrowd("Толпа акторов");
            for (int i = 0; i < runOptions.CrowdLength; i++)
                {
                var actor = new WaitActor
                    {
                    Interval = runOptions.sleepTime
                    };
                crowd.Add(actor);
                }
            CreatedActor = crowd;
            CreateAndShowViewPort(crowd, "Очередь акторов");
            }

        /// <summary>
        /// TestActor_QueueOfCrowds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueOfCrowdsClick(object sender, EventArgs e)
            {
            }

        /// <summary>
        /// Выполнить актора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RunActorButtonClick(object sender, EventArgs e)
            {
            await CreatedActor?.RunAsync();
            }

        private void OnDebugTextFormButtonClick(object sender, EventArgs e)
            {
            DebugTextForm.Show("Заголовок", "текст");
            }
        }
    }