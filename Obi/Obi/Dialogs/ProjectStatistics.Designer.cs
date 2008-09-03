namespace Obi.Dialogs
{
    partial class ProjectStatistics
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( ProjectStatistics ) );
        this.m_lblDuration = new System.Windows.Forms.Label ();
        this.m_txtDuration = new System.Windows.Forms.TextBox ();
        this.m_lblSectionCount = new System.Windows.Forms.Label ();
        this.m_txtSectionsCount = new System.Windows.Forms.TextBox ();
        this.m_lblPhraseCount = new System.Windows.Forms.Label ();
        this.m_txtPhraseCount = new System.Windows.Forms.TextBox ();
        this.m_lblPageCount = new System.Windows.Forms.Label ();
        this.m_txtPageCount = new System.Windows.Forms.TextBox ();
        this.m_btnOk = new System.Windows.Forms.Button ();
        this.m_lblTitle = new System.Windows.Forms.Label ();
        this.m_txtTitle = new System.Windows.Forms.TextBox ();
        this.mCancelButton = new System.Windows.Forms.Button ();
        this.SuspendLayout ();
        // 
        // m_lblDuration
        // 
        resources.ApplyResources ( this.m_lblDuration, "m_lblDuration" );
        this.m_lblDuration.Name = "m_lblDuration";
        // 
        // m_txtDuration
        // 
        resources.ApplyResources ( this.m_txtDuration, "m_txtDuration" );
        this.m_txtDuration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.m_txtDuration.Name = "m_txtDuration";
        this.m_txtDuration.ReadOnly = true;
        // 
        // m_lblSectionCount
        // 
        resources.ApplyResources ( this.m_lblSectionCount, "m_lblSectionCount" );
        this.m_lblSectionCount.Name = "m_lblSectionCount";
        // 
        // m_txtSectionsCount
        // 
        resources.ApplyResources ( this.m_txtSectionsCount, "m_txtSectionsCount" );
        this.m_txtSectionsCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.m_txtSectionsCount.Name = "m_txtSectionsCount";
        this.m_txtSectionsCount.ReadOnly = true;
        // 
        // m_lblPhraseCount
        // 
        resources.ApplyResources ( this.m_lblPhraseCount, "m_lblPhraseCount" );
        this.m_lblPhraseCount.Name = "m_lblPhraseCount";
        // 
        // m_txtPhraseCount
        // 
        resources.ApplyResources ( this.m_txtPhraseCount, "m_txtPhraseCount" );
        this.m_txtPhraseCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.m_txtPhraseCount.Name = "m_txtPhraseCount";
        this.m_txtPhraseCount.ReadOnly = true;
        // 
        // m_lblPageCount
        // 
        resources.ApplyResources ( this.m_lblPageCount, "m_lblPageCount" );
        this.m_lblPageCount.Name = "m_lblPageCount";
        // 
        // m_txtPageCount
        // 
        resources.ApplyResources ( this.m_txtPageCount, "m_txtPageCount" );
        this.m_txtPageCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.m_txtPageCount.Name = "m_txtPageCount";
        this.m_txtPageCount.ReadOnly = true;
        // 
        // m_btnOk
        // 
        resources.ApplyResources ( this.m_btnOk, "m_btnOk" );
        this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.m_btnOk.Name = "m_btnOk";
        this.m_btnOk.UseVisualStyleBackColor = true;
        // 
        // m_lblTitle
        // 
        resources.ApplyResources ( this.m_lblTitle, "m_lblTitle" );
        this.m_lblTitle.Name = "m_lblTitle";
        // 
        // m_txtTitle
        // 
        resources.ApplyResources ( this.m_txtTitle, "m_txtTitle" );
        this.m_txtTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.m_txtTitle.Name = "m_txtTitle";
        // 
        // mCancelButton
        // 
        resources.ApplyResources ( this.mCancelButton, "mCancelButton" );
        this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.mCancelButton.Name = "mCancelButton";
        this.mCancelButton.UseVisualStyleBackColor = true;
        // 
        // ProjectStatistics
        // 
        this.AcceptButton = this.m_btnOk;
        resources.ApplyResources ( this, "$this" );
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.mCancelButton;
        this.Controls.Add ( this.mCancelButton );
        this.Controls.Add ( this.m_txtTitle );
        this.Controls.Add ( this.m_lblTitle );
        this.Controls.Add ( this.m_btnOk );
        this.Controls.Add ( this.m_txtPageCount );
        this.Controls.Add ( this.m_lblPageCount );
        this.Controls.Add ( this.m_txtPhraseCount );
        this.Controls.Add ( this.m_lblPhraseCount );
        this.Controls.Add ( this.m_txtSectionsCount );
        this.Controls.Add ( this.m_lblSectionCount );
        this.Controls.Add ( this.m_txtDuration );
        this.Controls.Add ( this.m_lblDuration );
        this.Name = "ProjectStatistics";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.Load += new System.EventHandler ( this.ProjectStatistics_Load );
        this.ResumeLayout ( false );
        this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDuration;
        private System.Windows.Forms.TextBox m_txtDuration;
        private System.Windows.Forms.Label m_lblSectionCount;
        private System.Windows.Forms.TextBox m_txtSectionsCount;
        private System.Windows.Forms.Label m_lblPhraseCount;
        private System.Windows.Forms.TextBox m_txtPhraseCount;
        private System.Windows.Forms.Label m_lblPageCount;
        private System.Windows.Forms.TextBox m_txtPageCount;
        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Label m_lblTitle;
        private System.Windows.Forms.TextBox m_txtTitle;
        private System.Windows.Forms.Button mCancelButton;
    }
}