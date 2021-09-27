using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ActorsCP;
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
            CreatedActor = actor;
            var form = new TextViewPort(actor, "aaa", MainProgram.MainIcon);
            form.Show();
            }

        /// <summary>
        /// TestActor_Queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQueueTestActor_Click(object sender, EventArgs e)
            {
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