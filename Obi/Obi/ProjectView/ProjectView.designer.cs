namespace Obi.ProjectView
{
    partial class ProjectView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mHSplitter = new System.Windows.Forms.SplitContainer();
            this.mStripsView = new Obi.ProjectView.StripsView();
            this.mTransportBar = new Obi.UserControls.TransportBar();
            this.mHSplitter.Panel2.SuspendLayout();
            this.mHSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // mHSplitter
            // 
            this.mHSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mHSplitter.Location = new System.Drawing.Point(0, 0);
            this.mHSplitter.Name = "mHSplitter";
            // 
            // mHSplitter.Panel2
            // 
            this.mHSplitter.Panel2.Controls.Add(this.mStripsView);
            this.mHSplitter.Size = new System.Drawing.Size(972, 517);
            this.mHSplitter.SplitterDistance = 226;
            this.mHSplitter.TabIndex = 1;
            // 
            // mStripsView
            // 
            this.mStripsView.BackColor = System.Drawing.Color.Red;
            this.mStripsView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mStripsView.Location = new System.Drawing.Point(0, 0);
            this.mStripsView.Name = "mStripsView";
            this.mStripsView.Size = new System.Drawing.Size(742, 517);
            this.mStripsView.TabIndex = 0;
            // 
            // mTransportBar
            // 
            this.mTransportBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            this.mTransportBar.CurrentSelectedNode = null;
            this.mTransportBar.LocalPlaylist = null;
            this.mTransportBar.Location = new System.Drawing.Point(0, 523);
            this.mTransportBar.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.PlayOnFocusEnabled = true;
            this.mTransportBar.ProjectPanel = null;
            this.mTransportBar.ProjectView = null;
            this.mTransportBar.Size = new System.Drawing.Size(960, 35);
            this.mTransportBar.TabIndex = 0;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.mHSplitter);
            this.Controls.Add(this.mTransportBar);
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(972, 558);
            this.mHSplitter.Panel2.ResumeLayout(false);
            this.mHSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Obi.UserControls.TransportBar mTransportBar;
        private System.Windows.Forms.SplitContainer mHSplitter;
        private StripsView mStripsView;

    }
}
