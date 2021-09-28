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
            var form = new TextViewPort(actor, "Одиночка", MainProgram.MainIcon);
            form.Show();
            }

        /// <summary>
        /// TestActor_Queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueTestActor_Click(object sender, EventArgs e)
            {
            int sleepTime = int.Parse(SleepTimeTextBox.Text);
            int N = int.Parse(QueueLegthTextBox.Text);
            var queue = new ActorsQueue("Очередь акторов");
            for (int i = 0; i < N; i++)
                {
                var actor = new WaitActor();
                actor.Interval = sleepTime;
                queue.Add(actor);
                }
            CreatedActor = queue;
            var form = new TextViewPort(queue, "Очередь акторов", MainProgram.MainIcon);
            form.Show();
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
            int sleepTime = int.Parse(SleepTimeTextBox.Text);
            int N = int.Parse(CrowdLengthTextBox.Text);
            var crowd = new ActorsCrowd("Толпа акторов");
            for (int i = 0; i < N; i++)
                {
                var actor = new WaitActor();
                actor.Interval = sleepTime;
                crowd.Add(actor);
                }
            CreatedActor = crowd;
            var form = new TextViewPort(crowd, "Очередь акторов", MainProgram.MainIcon);
            form.Show();
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