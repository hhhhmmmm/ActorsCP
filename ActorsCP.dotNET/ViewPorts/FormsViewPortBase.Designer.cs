namespace ActorsCP.dotNET.ViewPorts
    {
    public partial class FormsViewPortBase
        {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
            {
            this.StatisticsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // StatisticsLabel
            this.StatisticsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatisticsLabel.BackColor = System.Drawing.Color.Yellow;
            this.StatisticsLabel.Location = new System.Drawing.Point(1, 3);
            this.StatisticsLabel.Name = "StatisticsLabel";
            this.StatisticsLabel.Size = new System.Drawing.Size(622, 18);
            this.StatisticsLabel.TabIndex = 1;
            this.StatisticsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // ViewPortBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(624, 481);
            this.Controls.Add(this.StatisticsLabel);
 this.Name = "ViewPortBase";
            this.Text = "Дерево событий актеров";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private System.Windows.Forms.Label StatisticsLabel;
        }
    }
