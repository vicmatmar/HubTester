﻿namespace HubTester
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
            this.RunButton = new System.Windows.Forms.Button();
            this.runTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // RunButton
            // 
            this.RunButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RunButton.Location = new System.Drawing.Point(756, 12);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(75, 23);
            this.RunButton.TabIndex = 0;
            this.RunButton.Text = "&Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // runTextBox
            // 
            this.runTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.runTextBox.Font = new System.Drawing.Font("Microsoft YaHei", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runTextBox.Location = new System.Drawing.Point(11, 52);
            this.runTextBox.Multiline = true;
            this.runTextBox.Name = "runTextBox";
            this.runTextBox.ReadOnly = true;
            this.runTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.runTextBox.Size = new System.Drawing.Size(820, 416);
            this.runTextBox.TabIndex = 3;
            this.runTextBox.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 480);
            this.Controls.Add(this.runTextBox);
            this.Controls.Add(this.RunButton);
            this.Name = "MainForm";
            this.Text = "HubTester";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TextBox runTextBox;
    }
}

