﻿namespace FirstTutorial
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.m_IncreseAmplitudeComboBox = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.FadeOutDurationTextBox = new System.Windows.Forms.TextBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.FadeInDurationTextBox = new System.Windows.Forms.TextBox();
            this.FadeOutDurationButton = new System.Windows.Forms.Button();
            this.FadeInDurationButton = new System.Windows.Forms.Button();
            this.FadeOutStartingPointTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.m_IncreaseAmplitude = new System.Windows.Forms.Button();
            this.m_DecreaseAmplitudeComboBox = new System.Windows.Forms.ComboBox();
            this.m_DecreaseAmplitudeButton = new System.Windows.Forms.Button();
            this.m_NoiseReductionButton = new System.Windows.Forms.Button();
            this.m_LowCutOffFrequencyTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_HighCutoffFrequencyTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(434, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open WAV File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(13, 56);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(434, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Normalize";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(13, 176);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(434, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "Fade Out Whole Audio";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // m_IncreseAmplitudeComboBox
            // 
            this.m_IncreseAmplitudeComboBox.FormattingEnabled = true;
            this.m_IncreseAmplitudeComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.m_IncreseAmplitudeComboBox.Location = new System.Drawing.Point(13, 93);
            this.m_IncreseAmplitudeComboBox.Name = "m_IncreseAmplitudeComboBox";
            this.m_IncreseAmplitudeComboBox.Size = new System.Drawing.Size(121, 21);
            this.m_IncreseAmplitudeComboBox.TabIndex = 4;
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(13, 216);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(434, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Fade In Whole Audio";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // FadeOutDurationTextBox
            // 
            this.FadeOutDurationTextBox.Location = new System.Drawing.Point(347, 263);
            this.FadeOutDurationTextBox.Name = "FadeOutDurationTextBox";
            this.FadeOutDurationTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeOutDurationTextBox.TabIndex = 7;
            this.FadeOutDurationTextBox.Text = "0";
            // 
            // FadeInDurationTextBox
            // 
            this.FadeInDurationTextBox.Location = new System.Drawing.Point(204, 326);
            this.FadeInDurationTextBox.Name = "FadeInDurationTextBox";
            this.FadeInDurationTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeInDurationTextBox.TabIndex = 8;
            this.FadeInDurationTextBox.Text = "0";
            // 
            // FadeOutDurationButton
            // 
            this.FadeOutDurationButton.Enabled = false;
            this.FadeOutDurationButton.Location = new System.Drawing.Point(474, 260);
            this.FadeOutDurationButton.Name = "FadeOutDurationButton";
            this.FadeOutDurationButton.Size = new System.Drawing.Size(134, 23);
            this.FadeOutDurationButton.TabIndex = 9;
            this.FadeOutDurationButton.Text = "Fade Out Duration";
            this.FadeOutDurationButton.UseVisualStyleBackColor = true;
            this.FadeOutDurationButton.Click += new System.EventHandler(this.FadeOutDurationButton_Click);
            // 
            // FadeInDurationButton
            // 
            this.FadeInDurationButton.Enabled = false;
            this.FadeInDurationButton.Location = new System.Drawing.Point(347, 324);
            this.FadeInDurationButton.Name = "FadeInDurationButton";
            this.FadeInDurationButton.Size = new System.Drawing.Size(135, 23);
            this.FadeInDurationButton.TabIndex = 10;
            this.FadeInDurationButton.Text = "Fade In Duration";
            this.FadeInDurationButton.UseVisualStyleBackColor = true;
            this.FadeInDurationButton.Click += new System.EventHandler(this.FadeInDurationButton_Click);
            // 
            // FadeOutStartingPointTextBox
            // 
            this.FadeOutStartingPointTextBox.Location = new System.Drawing.Point(141, 263);
            this.FadeOutStartingPointTextBox.Name = "FadeOutStartingPointTextBox";
            this.FadeOutStartingPointTextBox.Size = new System.Drawing.Size(100, 20);
            this.FadeOutStartingPointTextBox.TabIndex = 11;
            this.FadeOutStartingPointTextBox.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Fade Out Duration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 266);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Fade Out Starting Point";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 326);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Fade In Duration";
            // 
            // m_IncreaseAmplitude
            // 
            this.m_IncreaseAmplitude.Enabled = false;
            this.m_IncreaseAmplitude.Location = new System.Drawing.Point(166, 91);
            this.m_IncreaseAmplitude.Name = "m_IncreaseAmplitude";
            this.m_IncreaseAmplitude.Size = new System.Drawing.Size(281, 23);
            this.m_IncreaseAmplitude.TabIndex = 15;
            this.m_IncreaseAmplitude.Text = "Increase Amplitude";
            this.m_IncreaseAmplitude.UseVisualStyleBackColor = true;
            this.m_IncreaseAmplitude.Click += new System.EventHandler(this.m_IncreaseAmplitude_Click);
            // 
            // m_DecreaseAmplitudeComboBox
            // 
            this.m_DecreaseAmplitudeComboBox.FormattingEnabled = true;
            this.m_DecreaseAmplitudeComboBox.Items.AddRange(new object[] {
            "0.25",
            "0.5",
            "0.75"});
            this.m_DecreaseAmplitudeComboBox.Location = new System.Drawing.Point(13, 137);
            this.m_DecreaseAmplitudeComboBox.Name = "m_DecreaseAmplitudeComboBox";
            this.m_DecreaseAmplitudeComboBox.Size = new System.Drawing.Size(121, 21);
            this.m_DecreaseAmplitudeComboBox.TabIndex = 16;
            // 
            // m_DecreaseAmplitudeButton
            // 
            this.m_DecreaseAmplitudeButton.Enabled = false;
            this.m_DecreaseAmplitudeButton.Location = new System.Drawing.Point(166, 135);
            this.m_DecreaseAmplitudeButton.Name = "m_DecreaseAmplitudeButton";
            this.m_DecreaseAmplitudeButton.Size = new System.Drawing.Size(281, 23);
            this.m_DecreaseAmplitudeButton.TabIndex = 17;
            this.m_DecreaseAmplitudeButton.Text = "Decrease Amplitude";
            this.m_DecreaseAmplitudeButton.UseVisualStyleBackColor = true;
            this.m_DecreaseAmplitudeButton.Click += new System.EventHandler(this.m_DecreaseAmplitudeButton_Click);
            // 
            // m_NoiseReductionButton
            // 
            this.m_NoiseReductionButton.Enabled = false;
            this.m_NoiseReductionButton.Location = new System.Drawing.Point(460, 382);
            this.m_NoiseReductionButton.Name = "m_NoiseReductionButton";
            this.m_NoiseReductionButton.Size = new System.Drawing.Size(157, 23);
            this.m_NoiseReductionButton.TabIndex = 18;
            this.m_NoiseReductionButton.Text = "Noise Reduction";
            this.m_NoiseReductionButton.UseVisualStyleBackColor = true;
            this.m_NoiseReductionButton.Click += new System.EventHandler(this.m_NoiseReductionButton_Click);
            // 
            // m_LowCutOffFrequencyTextBox
            // 
            this.m_LowCutOffFrequencyTextBox.Location = new System.Drawing.Point(115, 385);
            this.m_LowCutOffFrequencyTextBox.Name = "m_LowCutOffFrequencyTextBox";
            this.m_LowCutOffFrequencyTextBox.Size = new System.Drawing.Size(100, 20);
            this.m_LowCutOffFrequencyTextBox.TabIndex = 19;
            this.m_LowCutOffFrequencyTextBox.Text = "1500";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1, 387);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Low  Cutoff Frequency";
            // 
            // m_HighCutoffFrequencyTextBox
            // 
            this.m_HighCutoffFrequencyTextBox.Location = new System.Drawing.Point(350, 387);
            this.m_HighCutoffFrequencyTextBox.Name = "m_HighCutoffFrequencyTextBox";
            this.m_HighCutoffFrequencyTextBox.Size = new System.Drawing.Size(100, 20);
            this.m_HighCutoffFrequencyTextBox.TabIndex = 21;
            this.m_HighCutoffFrequencyTextBox.Text = "3000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(234, 392);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "High Cutoff Frequency";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 498);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.m_HighCutoffFrequencyTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_LowCutOffFrequencyTextBox);
            this.Controls.Add(this.m_NoiseReductionButton);
            this.Controls.Add(this.m_DecreaseAmplitudeButton);
            this.Controls.Add(this.m_DecreaseAmplitudeComboBox);
            this.Controls.Add(this.m_IncreaseAmplitude);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FadeOutStartingPointTextBox);
            this.Controls.Add(this.FadeInDurationButton);
            this.Controls.Add(this.FadeOutDurationButton);
            this.Controls.Add(this.FadeInDurationTextBox);
            this.Controls.Add(this.FadeOutDurationTextBox);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.m_IncreseAmplitudeComboBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "First Tutorial";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox m_IncreseAmplitudeComboBox;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox FadeOutDurationTextBox;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.TextBox FadeInDurationTextBox;
        private System.Windows.Forms.Button FadeOutDurationButton;
        private System.Windows.Forms.Button FadeInDurationButton;
        private System.Windows.Forms.TextBox FadeOutStartingPointTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button m_IncreaseAmplitude;
        private System.Windows.Forms.ComboBox m_DecreaseAmplitudeComboBox;
        private System.Windows.Forms.Button m_DecreaseAmplitudeButton;
        private System.Windows.Forms.Button m_NoiseReductionButton;
        private System.Windows.Forms.TextBox m_LowCutOffFrequencyTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox m_HighCutoffFrequencyTextBox;
        private System.Windows.Forms.Label label5;
    }
}

