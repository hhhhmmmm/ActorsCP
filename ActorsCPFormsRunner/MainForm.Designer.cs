
namespace ActorsCPFormsRunner
    {
    partial class MainForm
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null))
                {
                components.Dispose();
                }
            base.Dispose(disposing);
            }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.CrowdLengthTextBox = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.QueueLegthTextBox = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.SleepTimeTextBox = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.SingleTestActorButton = new System.Windows.Forms.Button();
            this.QueueTestActorButton = new System.Windows.Forms.Button();
            this.CombinedQueueActorButton = new System.Windows.Forms.Button();
            this.CrowdTestActorButton = new System.Windows.Forms.Button();
            this.TestActor_QueueOfCrowdsButton = new System.Windows.Forms.Button();
            this.RunActorButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ViewportTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // CrowdLengthTextBox
            // 
            this.CrowdLengthTextBox.Location = new System.Drawing.Point(147, 117);
            this.CrowdLengthTextBox.Name = "CrowdLengthTextBox";
            this.CrowdLengthTextBox.Size = new System.Drawing.Size(77, 20);
            this.CrowdLengthTextBox.TabIndex = 42;
            this.CrowdLengthTextBox.Text = "10";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(53, 117);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(77, 13);
            this.label24.TabIndex = 41;
            this.label24.Text = "Длина толпы:";
            // 
            // QueueLegthTextBox
            // 
            this.QueueLegthTextBox.Location = new System.Drawing.Point(142, 55);
            this.QueueLegthTextBox.Name = "QueueLegthTextBox";
            this.QueueLegthTextBox.Size = new System.Drawing.Size(77, 20);
            this.QueueLegthTextBox.TabIndex = 40;
            this.QueueLegthTextBox.Text = "10";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(48, 55);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(87, 13);
            this.label23.TabIndex = 39;
            this.label23.Text = "Длина очереди:";
            // 
            // SleepTimeTextBox
            // 
            this.SleepTimeTextBox.Location = new System.Drawing.Point(147, 150);
            this.SleepTimeTextBox.Name = "SleepTimeTextBox";
            this.SleepTimeTextBox.Size = new System.Drawing.Size(77, 20);
            this.SleepTimeTextBox.TabIndex = 38;
            this.SleepTimeTextBox.Text = "100";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(53, 152);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(82, 13);
            this.label22.TabIndex = 37;
            this.label22.Text = "SleepTime (ms):";
            // 
            // SingleTestActorButton
            // 
            this.SingleTestActorButton.Location = new System.Drawing.Point(284, 55);
            this.SingleTestActorButton.Name = "SingleTestActorButton";
            this.SingleTestActorButton.Size = new System.Drawing.Size(194, 23);
            this.SingleTestActorButton.TabIndex = 43;
            this.SingleTestActorButton.Text = "TestActor_Single";
            this.SingleTestActorButton.UseVisualStyleBackColor = true;
            this.SingleTestActorButton.Click += new System.EventHandler(this.OnSingleTestActorClick);
            // 
            // QueueTestActorButton
            // 
            this.QueueTestActorButton.Location = new System.Drawing.Point(284, 89);
            this.QueueTestActorButton.Name = "QueueTestActorButton";
            this.QueueTestActorButton.Size = new System.Drawing.Size(194, 23);
            this.QueueTestActorButton.TabIndex = 44;
            this.QueueTestActorButton.Text = "TestActor_Queue";
            this.QueueTestActorButton.UseVisualStyleBackColor = true;
            this.QueueTestActorButton.Click += new System.EventHandler(this.OnQueueTestActor_Click);
            // 
            // CombinedQueueActorButton
            // 
            this.CombinedQueueActorButton.Location = new System.Drawing.Point(284, 118);
            this.CombinedQueueActorButton.Name = "CombinedQueueActorButton";
            this.CombinedQueueActorButton.Size = new System.Drawing.Size(194, 23);
            this.CombinedQueueActorButton.TabIndex = 45;
            this.CombinedQueueActorButton.Text = "TestActor_QueueOfQueues";
            this.CombinedQueueActorButton.UseVisualStyleBackColor = true;
            this.CombinedQueueActorButton.Click += new System.EventHandler(this.OnCombinedQueueActorClick);
            // 
            // CrowdTestActorButton
            // 
            this.CrowdTestActorButton.Location = new System.Drawing.Point(284, 147);
            this.CrowdTestActorButton.Name = "CrowdTestActorButton";
            this.CrowdTestActorButton.Size = new System.Drawing.Size(194, 23);
            this.CrowdTestActorButton.TabIndex = 46;
            this.CrowdTestActorButton.Text = "TestActor_Crowd";
            this.CrowdTestActorButton.UseVisualStyleBackColor = true;
            this.CrowdTestActorButton.Click += new System.EventHandler(this.OnCrowdTestActorClick);
            // 
            // TestActor_QueueOfCrowdsButton
            // 
            this.TestActor_QueueOfCrowdsButton.Location = new System.Drawing.Point(286, 177);
            this.TestActor_QueueOfCrowdsButton.Name = "TestActor_QueueOfCrowdsButton";
            this.TestActor_QueueOfCrowdsButton.Size = new System.Drawing.Size(192, 23);
            this.TestActor_QueueOfCrowdsButton.TabIndex = 47;
            this.TestActor_QueueOfCrowdsButton.Text = "TestActor_QueueOfCrowds";
            this.TestActor_QueueOfCrowdsButton.UseVisualStyleBackColor = true;
            this.TestActor_QueueOfCrowdsButton.Click += new System.EventHandler(this.OnQueueOfCrowdsClick);
            // 
            // RunActorButton
            // 
            this.RunActorButton.Location = new System.Drawing.Point(551, 53);
            this.RunActorButton.Name = "RunActorButton";
            this.RunActorButton.Size = new System.Drawing.Size(159, 23);
            this.RunActorButton.TabIndex = 48;
            this.RunActorButton.Text = "Выполнить актора";
            this.RunActorButton.UseVisualStyleBackColor = true;
            this.RunActorButton.Click += new System.EventHandler(this.RunActorButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 49;
            this.label1.Text = "Тип вьюпорта:";
            // 
            // ViewportTypeComboBox
            // 
            this.ViewportTypeComboBox.FormattingEnabled = true;
            this.ViewportTypeComboBox.Location = new System.Drawing.Point(147, 11);
            this.ViewportTypeComboBox.Name = "ViewportTypeComboBox";
            this.ViewportTypeComboBox.Size = new System.Drawing.Size(331, 21);
            this.ViewportTypeComboBox.TabIndex = 50;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ViewportTypeComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RunActorButton);
            this.Controls.Add(this.QueueTestActorButton);
            this.Controls.Add(this.CombinedQueueActorButton);
            this.Controls.Add(this.CrowdTestActorButton);
            this.Controls.Add(this.TestActor_QueueOfCrowdsButton);
            this.Controls.Add(this.SingleTestActorButton);
            this.Controls.Add(this.CrowdLengthTextBox);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.QueueLegthTextBox);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.SleepTimeTextBox);
            this.Controls.Add(this.label22);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Отладчик ActorsCP";
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion
        private System.Windows.Forms.TextBox CrowdLengthTextBox;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox QueueLegthTextBox;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox SleepTimeTextBox;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button SingleTestActorButton;
        private System.Windows.Forms.Button QueueTestActorButton;
        private System.Windows.Forms.Button CombinedQueueActorButton;
        private System.Windows.Forms.Button CrowdTestActorButton;
        private System.Windows.Forms.Button TestActor_QueueOfCrowdsButton;
        private System.Windows.Forms.Button RunActorButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ViewportTypeComboBox;
        }
    }

