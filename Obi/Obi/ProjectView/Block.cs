using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Block : UserControl, ISelectableInContentViewWithColors, ISearchable
    {
        protected EmptyNode mNode;                 // the corresponding node
        private bool mHighlighted;                 // selected flag
        private ISelectableInContentViewWithColors mParentContainer;  // not necessarily a strip!

        private int mBaseSpacing;
        private int mBaseHeight;
        private float mBaseFontSize;


        // Used by the designer
        public Block() { InitializeComponent(); }

        /// <summary>
        /// Create a new empty block from an empty node.
        /// </summary>
        public Block(EmptyNode node, ISelectableInContentViewWithColors parent): this()
        {
            mNode = node;
            mParentContainer = parent;
            mHighlighted = false;
            node.ChangedKind += new EmptyNode.ChangedKindEventHandler(Node_ChangedKind);
            node.ChangedPageNumber += new NodeEventHandler<EmptyNode>(Node_ChangedPageNumber);
            node.ChangedTo_DoStatus += new NodeEventHandler<EmptyNode>(Node_ChangedTo_DoStatus);
            UpdateColors();
            UpdateLabel();
            mBaseSpacing = Margin.Left;
            mBaseHeight = Height;
            mBaseFontSize = mLabel.Font.SizeInPoints;
        }


        public ColorSettings ColorSettings
        {
            get { return mParentContainer == null ? null : mParentContainer.ColorSettings; }
            set { UpdateColors(value); }
        }

        public ContentView ContentView
        {
            get { return mParentContainer == null ? null : mParentContainer.ContentView; }
        }

        /// <summary>
        /// Set the selected flag for the block.
        /// </summary>
        public virtual bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                if (value != mHighlighted)
                {
                    mHighlighted = value;
                    UpdateColors();
                }
            }
        }

        /// <summary>
        /// Get the tab index of the block.
        /// </summary>
        public int LastTabIndex { get { return TabIndex; } }

        /// <summary>
        /// The empty node for this block.
        /// </summary>
        public EmptyNode Node { get { return mNode; } }

        /// <summary>
        /// The Obi node for this block.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public virtual NodeSelection SelectionFromView { set { Highlighted = value != null; } }

        /// <summary>
        /// The strip that contains this block.
        /// </summary>
        public Strip Strip
        {
            get { return mParentContainer is Strip ? (Strip)mParentContainer : ((Block)mParentContainer).Strip; }
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors() { UpdateColors(ColorSettings); }

        public virtual void UpdateColors(ColorSettings settings)
        {
            if (mNode != null && settings != null)
            {
                BackColor =
                    mHighlighted ? settings.BlockBackColor_Selected :
                    mNode.NodeKind == EmptyNode.Kind.Silence ? settings.BlockBackColor_Silence :
                    !mNode.Used ? settings.BlockBackColor_Unused :
                    mNode.TODO ? settings.BlockBackColor_TODO :
                    mNode.NodeKind == EmptyNode.Kind.Custom ? settings.BlockBackColor_Custom :
                    mNode.NodeKind == EmptyNode.Kind.Heading ? settings.BlockBackColor_Heading :
                    mNode.NodeKind == EmptyNode.Kind.Page ? settings.BlockBackColor_Page :
                    !(mNode is PhraseNode) ? settings.BlockBackColor_Empty :
                        settings.BlockBackColor_Plain;
                ForeColor =
                    mHighlighted ? settings.BlockForeColor_Selected :
                    mNode.NodeKind == EmptyNode.Kind.Silence ? settings.BlockForeColor_Silence :
                    !mNode.Used ? settings.BlockForeColor_Unused :
                    mNode.TODO ? settings.BlockForeColor_TODO :
                    mNode.NodeKind == EmptyNode.Kind.Custom ? settings.BlockForeColor_Custom :
                    mNode.NodeKind == EmptyNode.Kind.Heading ? settings.BlockForeColor_Heading :
                    mNode.NodeKind == EmptyNode.Kind.Page ? settings.BlockForeColor_Page :
                    !(mNode is PhraseNode) ? settings.BlockForeColor_Empty :
                        settings.BlockForeColor_Plain;
            }
        }

        /// <summary>
        /// Update the tab index of the block with the new value and return the next index.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            return index + 1;
        }

        /// <summary>
        /// Set the zoom factor and the height.
        /// We cheat for the height so that it fits exactly in the parent container.
        /// </summary>
        public virtual void SetZoomFactorAndHeight(float zoom, int height)
        {
            if (zoom > 0.0f)
            {
                mLabel.Font = new Font(Font.FontFamily, zoom * mBaseFontSize);
                int margin = (int)Math.Round(zoom * mBaseSpacing);
                Margin = new Padding(margin, 0, margin, 0);
                Size = new Size(LabelFullWidth, height);
            }
        }


        #region ISearchable Members

        public string ToMatch()
        {
            return mLabel.Text.ToLowerInvariant();
        }

        #endregion


        // Width of the label (including margins)
        protected int LabelFullWidth { get { return mLabel.Width + mLabel.Margin.Horizontal; } }

        // Generate the label string for this block.
        // Since there is no content, the width is always that of the label's.
        public virtual void UpdateLabel()
        {
            UpdateLabelsText();
            Size = new Size(LabelFullWidth, Height);
        }


        private delegate void UpdateLabelsTextDelegate();

        public virtual void UpdateLabelsText()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateLabelsTextDelegate(UpdateLabelsText));
            }
            else
            {
                mLabel.Text = Node.BaseStringShort();
                mLabel.AccessibleName = Node.BaseString();
                mToolTip.SetToolTip(this, Node.BaseStringShort());
                mToolTip.SetToolTip(mLabel, Node.BaseStringShort());
                AccessibleName = mLabel.AccessibleName;
            }
        }

        // Select/deselect on click
        private void Block_Click(object sender, EventArgs e) { Strip.SelectedBlock = this; }

        // Select on tabbing
        protected void Block_Enter(object sender, EventArgs e)
        {
            if (!Strip.ParentView.Focusing)
            {
                Strip.SelectedBlock = this;
            }
        }

        // Update label when the page number changes
        private void Node_ChangedPageNumber(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }

        // Update the label when the role of the node changes
        private void Node_ChangedKind(object sender, ChangedKindEventArgs e) { UpdateLabel(); }

        // update label when to do status changes
        private void Node_ChangedTo_DoStatus(object sender, NodeEventArgs<EmptyNode> e) { UpdateLabel(); }

        private void Block_SizeChanged(object sender, EventArgs e)
        {

        }
    }
}
