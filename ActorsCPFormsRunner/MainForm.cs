using System;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ActorsCP.Actors;
using ActorsCP.dotNET.ViewPorts;
using ActorsCP.Tests.TestActors;

namespace ActorsCPFormsRunner
    {
    public partial class MainForm : Form
        {
        public MainForm()
            {
            InitializeComponent();
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
            var options = new RunOptions();
            options.sleepTime = int.Parse(SleepTimeTextBox.Text);
            options.QueueLegth = int.Parse(QueueLegthTextBox.Text);
            options.CrowdLength = int.Parse(CrowdLengthTextBox.Text);
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
            var form = new TextViewPort(actor, name, MainProgram.MainIcon);
            form.Show();
            }

        #endregion Вспомогательные методы

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
                var actor = new WaitActor();
                actor.Interval = runOptions.sleepTime;
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
                var actor = new WaitActor();
                actor.Interval = runOptions.sleepTime;
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
        }
    }