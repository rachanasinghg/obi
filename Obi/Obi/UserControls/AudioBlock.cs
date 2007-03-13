using System;
using System.Drawing;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media;

namespace Obi.UserControls
{
    /// <summary>
    /// The audio block is used to display a phrase.
    /// </summary>
    public partial class AudioBlock : AbstractBlock
    {
        private AnnotationBlock mAnnotationBlock;  // the annotation is taken out of the block

        public event SectionStrip.ChangedMinimumSizeHandler ChangedMinimumSize;

        #region properties

        /// <summary>
        /// Phrase node for this block.
        /// </summary>
        public override PhraseNode Node
        {
            get { return mNode; }
            set
            {
                mNode = value;
                if (mAnnotationBlock != null)
                {
                    mAnnotationBlock.Used = mNode != null ? mNode.Used : false;
                }
                if (mNode != null) RefreshDisplay();
            }
        }

        public override bool Selected
        {
            get { return base.Selected; }
            set
            {
                if (mSelected != value)
                {
                    mSelected = value;
                    if (value) Focus();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Used in the project or excluded from it.
        /// </summary>
        public override bool Used
        {
            set { RefreshUsed(); }
        }

        /// <summary>
        /// Strip manager panel in which this block is displayed.
        /// </summary>
        public override StripManagerPanel Manager
        {
            get { return mManager; }
            set
            {
                mManager = value;
                mAnnotationBlock.Manager = value;
            }
        }

        /// <summary>
        /// Annotation corresponding to this block.
        /// </summary>
        public AnnotationBlock AnnotationBlock
        {
            get { return mAnnotationBlock; }
            set
            {
                mAnnotationBlock = value;
                mAnnotationBlock.AudioBlock = this;
            }
        }

        #endregion

        #region instantiators

        public AudioBlock()
            : base()
        {
            InitializeComponent();
            mAnnotationBlock = new AnnotationBlock();
            mAnnotationBlock.AudioBlock = this;
            this.mToolTip.SetToolTip(this, Localizer.Message("audio_block_tooltip"));
            this.mToolTip.SetToolTip(this.mTimeLabel, Localizer.Message("audio_block_duration_tooltip"));
        }

        #endregion

        #region AudioBlock (this)

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            mManager.SelectedNode = mNode;
        }

        private void AudioBlock_DoubleClick(object sender, EventArgs e)
        {
            mManager.SelectedNode = mNode;
            // should be handled by the project panel's transport bar
            ((ObiForm)mManager.ParentForm).Play(mNode);
        }

        #endregion

        /// <summary>
        /// Contents size changed, so update the minimum width.
        /// </summary>
        private void ContentsSizeChanged(object sender, EventArgs e)
        {
            RefreshWidth();
        }

        /// <summary>
        /// Paint borders on the block if selected. In thick red pen, no less.
        /// </summary>
        /// <remarks>Let's look at ControlPaint for better results.</remarks>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (mSelected)
            {
                Pen pen = new Pen(Colors.SelectionColor, Padding.All);
                e.Graphics.DrawRectangle(pen, new Rectangle(Padding.All / 2, Padding.All / 2,
                    Width - Padding.All, Height - Padding.All));
                pen.Dispose();
            }
        }

        /// <summary>
        /// Refresh the display when the node underneath has been modified.
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshUsed();
            RefreshLabels();
            RefreshWidth();
        }

        public void RefreshDisplay(double time)
        {
            RefreshUsed(time);
            RefreshLabels(time);
            RefreshWidth();
        }

        /// <summary>
        /// Refresh the display of the block to show its used state.
        /// </summary>
        private void RefreshUsed()
        {
            BackColor = mNode != null && mNode.Used ?
                mNode.Asset.LengthInMilliseconds == 0.0 ? Colors.AudioBlockEmpty : Colors.AudioBlockUsed :
                Colors.AudioBlockUnused;
            if (mAnnotationBlock != null)
            {
                mAnnotationBlock.Used = mNode.Used;
            }
        }

        private void RefreshUsed(double time)
        {
            BackColor = mNode != null && mNode.Used ?
                time == 0.0 ? Colors.AudioBlockEmpty : Colors.AudioBlockUsed :
                Colors.AudioBlockUnused;
            if (mAnnotationBlock != null)
            {
                mAnnotationBlock.Used = mNode.Used;
            }
        }

        /// <summary>
        /// Refresh the labels of the block (including accessible labels.)
        /// </summary>
        public void RefreshLabels()
        {
            // Set the label and accessible description
            if (mNode.PageProperty != null)
            {
                mLabel.Text = String.Format(Localizer.Message("page_number"), mNode.PageProperty.PageNumber);
            }
            else
            {
                int index = 0;
                int outof = 0;
                if (Parent != null && Parent.Controls != null)
                {
                    index = Parent.Controls.IndexOf(this) + 1;
                    outof = Parent.Controls.Count;
                }
                mLabel.Text = String.Format(Localizer.Message("audio_block_default_label"), index, outof);
            }
            AccessibleName  = mLabel.Text;
            // Set the time display
            mTimeLabel.Text = Assets.MediaAsset.FormatTime(mNode.Asset.LengthInMilliseconds);
        }

        private void RefreshLabels(double time)
        {
            RefreshLabels();
            mTimeLabel.Text = Assets.MediaAsset.FormatTime(time);
        }

        /// <summary>
        /// Refresh the width of the block and its annotation.
        /// </summary>
        private void RefreshWidth()
        {
            int wlabel = mLabel.Width + mLabel.Location.X + mLabel.Margin.Right;
            int wtime = mTimeLabel.Width + mTimeLabel.Location.X + mTimeLabel.Margin.Right;
            int widest = wlabel > wtime ? wlabel : wtime;
            MinimumSize = new Size(widest, Height);
            if (ChangedMinimumSize != null) ChangedMinimumSize(this, new EventArgs());
        }

        /// <summary>
        /// Select the block when it is entered.
        /// </summary>
        private void AudioBlock_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Print("Entering <{0}> from {1} ({2})", AccessibleDescription, sender, e);
            if (mSectionStrip.ChildCanFocus)
            {
                mManager.SelectedNode = mNode;
            }
            else
            {
                mSectionStrip.Focus();
            }
        }
    }
}