namespace ActorsCP.dotNET.Forms
	{
	partial class DebugTextForm
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
            this.DataTextBox = new System.Windows.Forms.TextBox();
            this.WordWrapCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // DataTextBox
            // 
            this.DataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DataTextBox.HideSelection = false;
            this.DataTextBox.Location = new System.Drawing.Point(4, 29);
            this.DataTextBox.Multiline = true;
            this.DataTextBox.Name = "DataTextBox";
            this.DataTextBox.ReadOnly = true;
            this.DataTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.DataTextBox.Size = new System.Drawing.Size(1433, 549);
            this.DataTextBox.TabIndex = 0;
            this.DataTextBox.WordWrap = false;
            // 
            // WordWrapCheckBox
            // 
            this.WordWrapCheckBox.AutoSize = true;
            this.WordWrapCheckBox.Location = new System.Drawing.Point(4, 6);
            this.WordWrapCheckBox.Name = "WordWrapCheckBox";
            this.WordWrapCheckBox.Size = new System.Drawing.Size(97, 17);
            this.WordWrapCheckBox.TabIndex = 1;
            this.WordWrapCheckBox.Text = "Перенос слов";
            this.WordWrapCheckBox.UseVisualStyleBackColor = true;
            this.WordWrapCheckBox.CheckedChanged += new System.EventHandler(this.OnWordWrapCheckedChanged);
            // 
            // DebugTextForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1439, 582);
            this.Controls.Add(this.WordWrapCheckBox);
            this.Controls.Add(this.DataTextBox);
            this.Name = "DebugTextForm";
            this.Text = "Окно отладчика - показывает переданный текст";
            this.ResumeLayout(false);
            this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.TextBox DataTextBox;
		private System.Windows.Forms.CheckBox WordWrapCheckBox;
		}
	}