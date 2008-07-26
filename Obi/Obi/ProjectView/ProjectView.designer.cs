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
            this.mTransportBarSplitter = new System.Windows.Forms.SplitContainer();
            this.mFindInTextSplitter = new System.Windows.Forms.SplitContainer();
            this.mTOCSplitter = new System.Windows.Forms.SplitContainer();
            this.mMetadataSplitter = new System.Windows.Forms.SplitContainer();
            this.mPanelInfoLabelButton = new System.Windows.Forms.Button();
            this.mNoProjectLabel = new System.Windows.Forms.Label();
            this.mTOCView = new Obi.ProjectView.InheritedTOCView();
            this.mMetadataView = new Obi.ProjectView.MetadataView();
            this.mContentView = new Obi.ProjectView.ContentView();
            this.mFindInText = new Obi.ProjectView.FindInText();
            this.mTransportBar = new Obi.ProjectView.TransportBar();
            this.mTransportBarSplitter.Panel1.SuspendLayout();
            this.mTransportBarSplitter.Panel2.SuspendLayout();
            this.mTransportBarSplitter.SuspendLayout();
            this.mFindInTextSplitter.Panel1.SuspendLayout();
            this.mFindInTextSplitter.Panel2.SuspendLayout();
            this.mFindInTextSplitter.SuspendLayout();
            this.mTOCSplitter.Panel1.SuspendLayout();
            this.mTOCSplitter.Panel2.SuspendLayout();
            this.mTOCSplitter.SuspendLayout();
            this.mMetadataSplitter.Panel1.SuspendLayout();
            this.mMetadataSplitter.Panel2.SuspendLayout();
            this.mMetadataSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTransportBarSplitter
            // 
            this.mTransportBarSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.mTransportBarSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTransportBarSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mTransportBarSplitter.IsSplitterFixed = true;
            this.mTransportBarSplitter.Location = new System.Drawing.Point(0, 0);
            this.mTransportBarSplitter.Name = "mTransportBarSplitter";
            this.mTransportBarSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mTransportBarSplitter.Panel1
            // 
            this.mTransportBarSplitter.Panel1.Controls.Add(this.mFindInTextSplitter);
            // 
            // mTransportBarSplitter.Panel2
            // 
            this.mTransportBarSplitter.Panel2.Controls.Add(this.mTransportBar);
            this.mTransportBarSplitter.Panel2MinSize = 35;
            this.mTransportBarSplitter.Size = new System.Drawing.Size(996, 700);
            this.mTransportBarSplitter.SplitterDistance = 660;
            this.mTransportBarSplitter.TabIndex = 0;
            // 
            // mFindInTextSplitter
            // 
            this.mFindInTextSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.mFindInTextSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFindInTextSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mFindInTextSplitter.IsSplitterFixed = true;
            this.mFindInTextSplitter.Location = new System.Drawing.Point(0, 0);
            this.mFindInTextSplitter.Name = "mFindInTextSplitter";
            this.mFindInTextSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mFindInTextSplitter.Panel1
            // 
            this.mFindInTextSplitter.Panel1.Controls.Add(this.mTOCSplitter);
            // 
            // mFindInTextSplitter.Panel2
            // 
            this.mFindInTextSplitter.Panel2.Controls.Add(this.mFindInText);
            this.mFindInTextSplitter.Panel2MinSize = 29;
            this.mFindInTextSplitter.Size = new System.Drawing.Size(996, 660);
            this.mFindInTextSplitter.SplitterDistance = 627;
            this.mFindInTextSplitter.TabIndex = 0;
            // 
            // mTOCSplitter
            // 
            this.mTOCSplitter.AccessibleName = "Panel Splitter";
            this.mTOCSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.mTOCSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCSplitter.Location = new System.Drawing.Point(0, 0);
            this.mTOCSplitter.Name = "mTOCSplitter";
            // 
            // mTOCSplitter.Panel1
            // 
            this.mTOCSplitter.Panel1.Controls.Add(this.mMetadataSplitter);
            this.mTOCSplitter.Panel1.Controls.Add(this.mPanelInfoLabelButton);
            // 
            // mTOCSplitter.Panel2
            // 
            this.mTOCSplitter.Panel2.Controls.Add(this.mContentView);
            this.mTOCSplitter.Size = new System.Drawing.Size(996, 627);
            this.mTOCSplitter.SplitterDistance = 302;
            this.mTOCSplitter.TabIndex = 0;
            // 
            // mMetadataSplitter
            // 
            this.mMetadataSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMetadataSplitter.Location = new System.Drawing.Point(0, 0);
            this.mMetadataSplitter.Name = "mMetadataSplitter";
            this.mMetadataSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mMetadataSplitter.Panel1
            // 
            this.mMetadataSplitter.Panel1.Controls.Add(this.mTOCView);
            // 
            // mMetadataSplitter.Panel2
            // 
            this.mMetadataSplitter.Panel2.Controls.Add(this.mMetadataView);
            this.mMetadataSplitter.Size = new System.Drawing.Size(302, 627);
            this.mMetadataSplitter.SplitterDistance = 349;
            this.mMetadataSplitter.TabIndex = 0;
            // 
            // mPanelInfoLabelButton
            // 
            this.mPanelInfoLabelButton.AccessibleName = "Panel Splitter";
            this.mPanelInfoLabelButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPanelInfoLabelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mPanelInfoLabelButton.AutoSize = true;
            this.mPanelInfoLabelButton.BackColor = System.Drawing.Color.Transparent;
            this.mPanelInfoLabelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mPanelInfoLabelButton.Location = new System.Drawing.Point(-132, -1);
            this.mPanelInfoLabelButton.Name = "mPanelInfoLabelButton";
            this.mPanelInfoLabelButton.Size = new System.Drawing.Size(6, 6);
            this.mPanelInfoLabelButton.TabIndex = 2;
            this.mPanelInfoLabelButton.UseVisualStyleBackColor = false;
            this.mPanelInfoLabelButton.Leave += new System.EventHandler(this.mPanelInfoLabelButton_Leave);
            this.mPanelInfoLabelButton.Enter += new System.EventHandler(this.mPanelInfoLabelButton_Enter);
            // 
            // mNoProjectLabel
            // 
            this.mNoProjectLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.mNoProjectLabel.AutoSize = true;
            this.mNoProjectLabel.BackColor = System.Drawing.SystemColors.Control;
            this.mNoProjectLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mNoProjectLabel.Location = new System.Drawing.Point(458, 340);
            this.mNoProjectLabel.Name = "mNoProjectLabel";
            this.mNoProjectLabel.Size = new System.Drawing.Size(81, 20);
            this.mNoProjectLabel.TabIndex = 1;
            this.mNoProjectLabel.Text = "No project";
            // 
            // mTOCView
            // 
            this.mTOCView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTOCView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTOCView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTOCView.FullRowSelect = true;
            this.mTOCView.LabelEdit = true;
            this.mTOCView.Location = new System.Drawing.Point(0, 0);
            this.mTOCView.Name = "mTOCView";
            this.mTOCView.Selection = null;
            this.mTOCView.Size = new System.Drawing.Size(302, 349);
            this.mTOCView.TabIndex = 0;
            // 
            // mMetadataView
            // 
            this.mMetadataView.BackColor = System.Drawing.Color.Transparent;
            this.mMetadataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mMetadataView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mMetadataView.Location = new System.Drawing.Point(0, 0);
            this.mMetadataView.Margin = new System.Windows.Forms.Padding(0);
            this.mMetadataView.Name = "mMetadataView";
            this.mMetadataView.Selection = null;
            this.mMetadataView.Size = new System.Drawing.Size(302, 274);
            this.mMetadataView.TabIndex = 0;
            // 
            // mContentView
            // 
            this.mContentView.AudioScale = 0.01F;
            this.mContentView.AutoScroll = true;
            this.mContentView.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.mContentView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mContentView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mContentView.Location = new System.Drawing.Point(0, 0);
            this.mContentView.Name = "mContentView";
            this.mContentView.PlaybackPhrase = null;
            this.mContentView.Selection = null;
            this.mContentView.Size = new System.Drawing.Size(690, 627);
            this.mContentView.TabIndex = 0;
            this.mContentView.ZoomFactor = 1F;
            // 
            // mFindInText
            // 
            this.mFindInText.BackColor = System.Drawing.SystemColors.Control;
            this.mFindInText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mFindInText.Location = new System.Drawing.Point(0, 0);
            this.mFindInText.Name = "mFindInText";
            this.mFindInText.Size = new System.Drawing.Size(996, 29);
            this.mFindInText.TabIndex = 0;
            // 
            // mTransportBar
            // 
            this.mTransportBar.BackColor = System.Drawing.Color.White;
            this.mTransportBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTransportBar.LocalPlaylist = null;
            this.mTransportBar.Location = new System.Drawing.Point(0, 0);
            this.mTransportBar.Name = "mTransportBar";
            this.mTransportBar.SelectionChangedPlaybackEnabled = true;
            this.mTransportBar.Size = new System.Drawing.Size(1060, 35);
            this.mTransportBar.TabIndex = 0;
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.Controls.Add(this.mNoProjectLabel);
            this.Controls.Add(this.mTransportBarSplitter);
            this.DoubleBuffered = true;
            this.Name = "ProjectView";
            this.Size = new System.Drawing.Size(996, 700);
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ProjectView_Layout);
            this.mTransportBarSplitter.Panel1.ResumeLayout(false);
            this.mTransportBarSplitter.Panel2.ResumeLayout(false);
            this.mTransportBarSplitter.ResumeLayout(false);
            this.mFindInTextSplitter.Panel1.ResumeLayout(false);
            this.mFindInTextSplitter.Panel2.ResumeLayout(false);
            this.mFindInTextSplitter.ResumeLayout(false);
            this.mTOCSplitter.Panel1.ResumeLayout(false);
            this.mTOCSplitter.Panel1.PerformLayout();
            this.mTOCSplitter.Panel2.ResumeLayout(false);
            this.mTOCSplitter.ResumeLayout(false);
            this.mMetadataSplitter.Panel1.ResumeLayout(false);
            this.mMetadataSplitter.Panel2.ResumeLayout(false);
            this.mMetadataSplitter.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer mTransportBarSplitter;
        private TransportBar mTransportBar;
        private System.Windows.Forms.SplitContainer mFindInTextSplitter;
        private System.Windows.Forms.SplitContainer mTOCSplitter;
        private FindInText mFindInText;
        private System.Windows.Forms.SplitContainer mMetadataSplitter;
        private MetadataView mMetadataView;
        private System.Windows.Forms.Label mNoProjectLabel;
        private System.Windows.Forms.Button mPanelInfoLabelButton ;
        private InheritedTOCView mTOCView;
        private ContentView mContentView;


    }
}
