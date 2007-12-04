using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    /// <summary>
    /// Common interface for selection of strips and blocks
    /// </summary>
    public interface ISelectableInStripView
    {
        bool Selected { get; set; }
        ObiNode ObiNode { get; }
        NodeSelection SelectionFromView { set; }
    }

    public partial class StripsView : UserControl, IControlWithRenamableSelection
    {
        private ProjectView mView;                                   // parent project view
        private NodeSelection mSelection;                            // current selection
        private ISelectableInStripView mSelectedItem;                // the actual item for the selection
        private Dictionary<Keys, HandledShortcutKey> mShortcutKeys;  // list of all shortcuts

        // cursor stuff
        private AudioBlock mPlaybackBlock;
        private bool mFocusing;

        /// <summary>
        /// A new strips view.
        /// </summary>
        public StripsView()
        {
            InitializeComponent();
            InitializeShortcutKeys();
            mView = null;
            mSelection = null;
            mFocusing = false;
        }


        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        private bool BlockSelected { get { return mSelectedItem is Block && mSelection.Text == null; } }
        private bool StripSelected { get { return mSelectedItem is Strip && mSelection.Text == null; } }

        public bool CanAddStrip { get { return StripSelected; } }
        public bool CanRemoveBlock { get { return BlockSelected; } }
        public bool CanRemoveStrip { get { return StripSelected; } }
        public bool CanRenameStrip { get { return StripSelected; } }
        public bool CanSplitStrip { get { return BlockSelected && SelectedPhrase.Index > 0; } }

        public bool CanMergeBlockWithNext
        {
            get
            {
                EmptyNode node = mSelectedItem is Block ? ((Block)mSelectedItem).Node : null;
                return node != null && node.Index < node.ParentAs<ObiNode>().PhraseChildCount - 1;
            }
        }

        public bool CanMergeStripWithNext
        {
            get
            {
                return StripSelected &&
                    mLayoutPanel.Controls.IndexOf((Control)mSelectedItem) < mLayoutPanel.Controls.Count - 1;
            }
        }

        public bool Focusing { get { return mFocusing; } }

        public PhraseNode PlaybackBlock
        {
            get { return mPlaybackBlock.Node as PhraseNode; }
            set
            {
                mPlaybackBlock = value == null ? null : (AudioBlock)FindBlock(value);
                if (mPlaybackBlock != null) mPlaybackBlock.SetCursorTime(0.0);
            }
        }

        public void UpdateCursorPosition(double time) { mPlaybackBlock.UpdateCursorTime(time); }

        /// <summary>
        /// Create a command to delete the selected strip.
        /// </summary>
        public urakawa.undo.ICommand DeleteStripCommand()
        {
            return DeleteStripCommand(SelectedSection);
        }

        private urakawa.undo.ICommand DeleteStripCommand(SectionNode section)
        {
            Commands.Node.Delete delete = new Commands.Node.Delete(mView, section, Localizer.Message("delete_strip_command"));
            if (section.SectionChildCount > 0)
            {
                urakawa.undo.CompositeCommand command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(delete.getShortDescription());
                command.append(new Commands.TOC.MoveSectionOut(mView, section.SectionChild(0)));
                command.append(delete);
                return command;
            }
            else
            {
                return delete;
            }
        }

        /// <summary>
        /// Show the strip for this section node.
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (section != null) mLayoutPanel.ScrollControlIntoView(FindStrip(section));
        }

        /// <summary>
        /// Get a command to merge the selected strip with the next one.
        /// </summary>
        public urakawa.undo.ICommand MergeSelectedStripWithNextCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanMergeStripWithNext)
            {
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("merge_strips_command"));
                SectionNode section = SelectedSection;
                SectionNode next = section.SectionChildCount == 0 ? section.NextSibling : section.SectionChild(0);
                for (int i = 0; i < next.PhraseChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, next.PhraseChild(i), section)); 
                }
                command.append(DeleteStripCommand(next));
            }
            return command;
        }

        /// <summary>
        /// Set a new presentation for this view.
        /// </summary>
        public void NewPresentation()
        {
            mLayoutPanel.Controls.Clear();
            AddStripForSection(mView.Presentation.RootNode);
            mView.Presentation.treeNodeAdded += new TreeNodeAddedEventHandler(Presentation_treeNodeAdded);
            mView.Presentation.treeNodeRemoved += new TreeNodeRemovedEventHandler(Presentation_treeNodeRemoved);
            mView.Presentation.RenamedSectionNode += new NodeEventHandler<SectionNode>(Presentation_RenamedSectionNode);
            mView.Presentation.UsedStatusChanged += new NodeEventHandler<ObiNode>(Presentation_UsedStatusChanged);
        }

        /// <summary>
        /// Rename a strip.
        /// </summary>
        public void RenameStrip(Strip strip)
        {
            mView.RenameSectionNode(strip.Node, strip.Label);
        }

        public EmptyNode SelectedEmptyNode { get { return BlockSelected ? ((Block)mSelectedItem).Node : null; } }
        public PhraseNode SelectedPhrase { get { return BlockSelected ? ((Block)mSelectedItem).Node as PhraseNode : null; } }
        public SectionNode SelectedSection { get { return StripSelected ? ((Strip)mSelectedItem).Node : null; } }
        public ObiNode SelectedNode { set { if (mView != null) mView.Selection = new NodeSelection(value, this); } }
        public NodeSelection SelectionFromStrip { set { if (mView != null) mView.Selection = value; } }

        /// <summary>
        /// Set the selection from the parent view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                if (value != mSelection)
                {
                    ISelectableInStripView s = value == null ? null : FindSelectable(value.Node);
                    if (mSelectedItem != null) mSelectedItem.Selected = false;
                    mSelection = value;
                    mSelectedItem = s;
                    if (s != null)
                    {
                        s.SelectionFromView = mSelection;
                        mLayoutPanel.ScrollControlIntoView((Control)s);
                        SectionNode section = null;
                        if (value.Node is SectionNode) section = (SectionNode)value.Node;
                        else if (value.Node is PhraseNode)
                        {
                            //check for nodes inside a phrase container
                            if (value.Node.getParent() is PhraseNode) section = value.Node.ParentAs<PhraseNode>().ParentAs<SectionNode>();
                            else section = value.Node.ParentAs<SectionNode>();
                        }
                        else section = null;
                        mView.MakeTreeNodeVisibleForSection(section);
                        mFocusing = true;
                        if (!((Control)s).Focused) ((Control)s).Focus();
                        mFocusing = false;
                    }
                }
            }
        }

        /// <summary>
        /// Show/hide strips under the one for which the section was collapsed or expanded.
        /// </summary>
        public void SetStripsVisibilityForSection(SectionNode section, bool visible)
        {
            for (int i = 0; i < section.SectionChildCount; ++i)
            {
                Strip s;
                SectionNode child = section.SectionChild(i);
                if ((s = FindStrip(child)) != null)
                {
                    s.Visible = visible;
                    if (mSelectedItem == s && !visible) mView.Selection = null;
                    SetStripsVisibilityForSection(section.SectionChild(i), visible);
                }
            }
        }

        /// <summary>
        /// Split a strip at the given block; i.e. create a new sibling section which inherits the children of
        /// the split section except for the phrases before the selected block. Do not do anything if there are
        /// no phrases before.
        /// </summary>
        public urakawa.undo.CompositeCommand SplitStripFromSelectedBlockCommand()
        {
            urakawa.undo.CompositeCommand command = null;
            if (CanSplitStrip)
            {
                PhraseNode phrase = SelectedPhrase;
                SectionNode section = phrase.ParentAs<SectionNode>();
                command = mView.Presentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("split_strip_command"));
                SectionNode sibling = mView.Presentation.CreateSectionNode();
                sibling.Label = section.Label;
                for (int i = 0; i < section.SectionChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.SectionChild(i), sibling));
                }
                for (int i = phrase.Index; i < section.PhraseChildCount; ++i)
                {
                    command.append(new Commands.Node.ChangeParent(mView, section.PhraseChild(i), sibling, phrase.Index));
                }
                command.append(new Commands.Node.AddNode(mView, sibling, section.ParentAs<ObiNode>(),
                    section.Index + 1));
            }
            return command;
        }

        /// <summary>
        /// Views are not synchronized anymore, so make sure that all strips are visible.
        /// </summary>
        public void UnsyncViews()
        {
            foreach (Control c in mLayoutPanel.Controls) c.Visible = true;
        }


        #region Event handlers

        // Handle resizing of the layout panel: all strips are resized to be at least as wide.
        private void mLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                int w = mLayoutPanel.Width - c.Location.X - c.Margin.Right;
                c.Size = new Size(w, c.Height);
            }
        }

        // Handle section nodes renamed from the project: change the label of the corresponding strip.
        private void Presentation_RenamedSectionNode(object sender, NodeEventArgs<SectionNode> e)
        {
            Strip strip = FindStrip(e.Node);
            strip.Label = e.Node.Label;
        }

        // Handle change of used status
        private void Presentation_UsedStatusChanged(object sender, NodeEventArgs<ObiNode> e)
        {
            if (e.Node is SectionNode)
            {
                Strip strip = FindStrip((SectionNode)e.Node);
                if (strip != null) strip.UpdateColors();
            }
        }

        // Handle addition of tree nodes: add a new strip for new section nodes.
        private void Presentation_treeNodeAdded(object o, TreeNodeAddedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                SectionNode section = (SectionNode)e.getTreeNode();
                if (section.IsRooted)
                {
                    Strip strip = AddStripForSection(section);
                    mLayoutPanel.ScrollControlIntoView(strip);
                    UpdateTabIndex(strip);
                }
            }
            else if (e.getTreeNode() is EmptyNode)
            {
                EmptyNode node = (EmptyNode)e.getTreeNode();
                if (node.IsRooted)
                {
                    // TODO replace FindStrip with FindParentContainer, as it can be a strip or a block
                    Block block = FindStrip(node.ParentAs<SectionNode>()).AddBlockForPhrase(node);
                    mLayoutPanel.ScrollControlIntoView(block);
                    UpdateTabIndex(block);
                }
            }
        }

        // Add a new strip for a section and all of its subsections
        private Strip AddStripForSection(ObiNode node)
        {
            Strip strip = null;
            if (node is SectionNode)
            {
                strip = new Strip((SectionNode)node, this);
                mLayoutPanel.Controls.Add(strip);
                mLayoutPanel.Controls.SetChildIndex(strip, ((SectionNode)node).Position);
                int w = mLayoutPanel.Width - strip.Location.X - strip.Margin.Right;
                strip.Size = new Size(w, strip.Height);
            }
            for (int i = 0; i < node.SectionChildCount; ++i) AddStripForSection(node.SectionChild(i));
            for (int i = 0; i < node.PhraseChildCount; ++i) strip.AddBlockForPhrase(node.PhraseChild(i));
            return strip;
        }

        // Handle removal of tree nodes: remove a strip for a section node and all of its children.
        void Presentation_treeNodeRemoved(object o, TreeNodeRemovedEventArgs e)
        {
            if (e.getTreeNode() is SectionNode)
            {
                RemoveStripsForSection((SectionNode)e.getTreeNode());
            }
            else if (e.getTreeNode() is EmptyNode)
            {
                Strip strip = FindStrip((SectionNode)e.getFormerParent());
                if (strip != null) strip.RemoveBlock((EmptyNode)e.getTreeNode());
            }
        }

        // Remove all strips for a section and its subsections
        private void RemoveStripsForSection(SectionNode section)
        {
            for (int i = 0; i < section.SectionChildCount; ++i) RemoveStripsForSection(section.SectionChild(i));
            Strip strip = FindStrip(section);
            mLayoutPanel.Controls.Remove(strip);
        }


        // Deselect everything when clicking the panel
        private void mLayoutPanel_Click(object sender, EventArgs e)
        {
            mView.Selection = null;
        }

        #endregion


        #region Utility functions

        private Block FindBlock(PhraseNode node)
        {
            //if this node's parent is a phrase node (i.e., this node is in a container)
            if (node.getParent() is PhraseNode)
            {
                PhraseNode p = node.ParentAs<PhraseNode>();
                ContainerBlock b = (ContainerBlock)this.FindBlock(p);
                Block phraseblock = b.FindBlock(node);
                return phraseblock;
            
            }
            else
            {
                return FindStrip(node.ParentAs<SectionNode>()).FindBlock(node);
            }
        }

        private ISelectableInStripView FindSelectable(ObiNode node)
        {
            return node is SectionNode ? (ISelectableInStripView)FindStrip((SectionNode)node) :
                node is PhraseNode ? (ISelectableInStripView)FindBlock((PhraseNode)node) : null;
        }

        /// <summary>
        /// Find the strip for the given section node.
        /// The strip must be present so an exception is thrown on failure.
        /// </summary>
        private Strip FindStrip(SectionNode section)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                if (c is Strip && ((Strip)c).Node == section) return c as Strip;
            }
            //throw new Exception(String.Format("Could not find strip for section node labeled `{0}'", section.Label));
            return null;
        }

        #endregion

        #region IControlWithRenamableSelection Members

        public void SelectAndRename(ObiNode node)
        {
            DoToNewNode(node, delegate()
            {
                mView.Selection = new NodeSelection(node, this, false);
                FindStrip((SectionNode)node).StartRenaming();
            });
        }

        private delegate void DoToNewNodeDelegate();

        // Do f() to a section node that may not yet be in the view.
        private void DoToNewNode(ObiNode node, DoToNewNodeDelegate f)
        {
            if (IsInView(node))
            {
                f();
            }
            else
            {
                TreeNodeAddedEventHandler h = delegate(object o, TreeNodeAddedEventArgs e) { };
                h = delegate(object o, TreeNodeAddedEventArgs e)
                {
                    if (e.getTreeNode() == node)
                    {
                        f();
                        mView.Presentation.treeNodeAdded -= h;
                    }
                };
                mView.Presentation.treeNodeAdded += h;
            }
        }

        private bool IsInView(ObiNode node)
        {
            return node is SectionNode && FindStrip((SectionNode)node) != null;
        }

        #endregion




        // temporary for search
        public FlowLayoutPanel LayoutPanel { get { return mLayoutPanel; } }

        /// <summary>
        /// Get all the searchable items (i.e. strips; later blocks) in the control
        /// </summary>
        public List<ISearchable> Searchables
        {
            get
            {
                List<ISearchable> l = new List<ISearchable>(mLayoutPanel.Controls.Count);
                foreach (Control c in mLayoutPanel.Controls) if (c is ISearchable) l.Add((ISearchable)c);
                return l;
            }
        }

        #region tabbing

        // Update tab index for all controls after a newly added strip
        private void UpdateTabIndex(Strip strip)
        {
            int stripIndex = mLayoutPanel.Controls.IndexOf(strip);
            int tabIndex = stripIndex > 0 ? ((Strip)mLayoutPanel.Controls[stripIndex - 1]).LastTabIndex : 0;
            for (int i = stripIndex; i < mLayoutPanel.Controls.Count; ++i)
            {
                tabIndex = ((Strip)mLayoutPanel.Controls[i]).UpdateTabIndex(tabIndex);
            }
        }

        // Update tab index for all controls after a newly added block
        private void UpdateTabIndex(Block block) { UpdateTabIndex(block.Strip); }

        #endregion

        #region shortcut keys

        public delegate bool HandledShortcutKey();  // key handling delegate

        private void InitializeShortcutKeys()
        {
            mShortcutKeys = new Dictionary<Keys, HandledShortcutKey>();

            // playback shortcuts.
            // Note: if these shortcuts are to be disabled till finalisation, just comment following five lines.
            mShortcutKeys[Keys.Space] = TogglePlayPause;
            mShortcutKeys[Keys.S] = FastPlayRateStepDown;
            mShortcutKeys[Keys.F] = FastPlayRateStepUp;
            mShortcutKeys[Keys.D] = FastPlayRateNormalise;
            mShortcutKeys[Keys.E] = FastPlayNormaliseWithLapseBack;

            // Strips navigation
            mShortcutKeys[Keys.Left] = SelectPrecedingBlock;
            mShortcutKeys[Keys.Right] = SelectFollowingBlock;
            mShortcutKeys[Keys.End] = SelectLastBlockInStrip;
            mShortcutKeys[Keys.Home] = SelectFirstBlockInStrip;

            mShortcutKeys[Keys.Up] = SelectPreviousStrip;
            mShortcutKeys[Keys.Down] = SelectNextStrip;
            mShortcutKeys[Keys.Control | Keys.Home] = SelectFirstStrip;
            mShortcutKeys[Keys.Control | Keys.End] = SelectLastStrip;
        }

        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly int WM_SYSKEYDOWN = 0x104;

        private bool CanUseKeys { get { return mSelection == null || mSelection.Text == null; } }

        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            if (CanUseKeys &&
                ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) &&
                mShortcutKeys.ContainsKey(key) && mShortcutKeys[key]()) return true;
            return base.ProcessCmdKey(ref msg, key);
        }

        // Get the strip for the currently selected component (i.e. the strip itself, or the parent strip
        // for a block.)
        private Strip StripFor(ISelectableInStripView item)
        {
            return item is Strip ? (Strip)item : item is Block ? ((Block)item).Strip : null;
        }

        private delegate Block SelectBlockFunction(Strip strip, ISelectableInStripView item);

        private bool SelectBlockFor(SelectBlockFunction f)
        {
            Strip strip = StripFor(mSelectedItem);
            if (strip != null)
            {
                Block block = f(strip, mSelectedItem);
                if (block != null)
                {
                    mView.Selection = new NodeSelection(block.Node, this);
                    return true;
                }
            }
            return false;
        }

        private bool SelectPrecedingBlock()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.BlockBefore(item); });
        }

        private bool SelectFollowingBlock()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.BlockAfter(item); });
        }

        private bool SelectLastBlockInStrip()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.LastBlock; });
        }

        private bool SelectFirstBlockInStrip()
        {
            return SelectBlockFor(delegate(Strip strip, ISelectableInStripView item) { return strip.FirstBlock; });
        }

        private delegate Strip SelectStripFunction(Strip strip);

        private bool SelectStripFor(SelectStripFunction f)
        {
            Strip strip = f(StripFor(mSelectedItem) as Strip);
            if (strip != null)
            {
                mView.Selection = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool SelectPreviousStrip()
        {
            Strip strip = mSelectedItem is Block ? StripFor(mSelectedItem) : StripBefore(StripFor(mSelectedItem));
            if (strip != null)
            {
                mView.Selection = new NodeSelection(strip.Node, this);
                return true;
            }
            return false;
        }

        private bool SelectNextStrip()
        {
            return SelectStripFor(StripAfter);
        }

        private bool SelectFirstStrip()
        {
            return SelectStripFor(delegate(Strip strip)
            {
                return mLayoutPanel.Controls.Count > 0 ? (Strip)mLayoutPanel.Controls[0] : null;
            });
        }

        private bool SelectLastStrip()
        {
            return SelectStripFor(delegate(Strip strip)
            {
                return mLayoutPanel.Controls.Count > 0 ? (Strip)mLayoutPanel.Controls[mLayoutPanel.Controls.Count - 1] :
                    null;
            });
        }

        private Strip StripAfter(Strip strip)
        {
            if (strip != null)
            {
                int count = mLayoutPanel.Controls.Count;
                int index = 1 + mLayoutPanel.Controls.IndexOf(strip);
                return index < count ? (Strip)mLayoutPanel.Controls[index] : null;
            }
            return null;
        }

        public Strip StripBefore(Strip strip)
        {
            if (strip != null)
            {
                int index = mLayoutPanel.Controls.IndexOf(strip);
                return index > 0 ? (Strip)mLayoutPanel.Controls[index - 1] : null;
            }
            return null;
        }


        /// <summary>
        /// Toggles play selection and pause with spacebar
        /// In this function Pause works both for play selection and Play all
        /// <see cref=""/>
        /// </summary>
        /// <returns></returns>
        private bool TogglePlayPause()
        {
            if (mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped
                || mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
            {
                mView.TransportBar.Play(mView.Selection.Node);
                return true;
            }
            else if (mView.TransportBar.CurrentPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
            {
                mView.TransportBar.Pause();
                return true;
            }
            return false;
        }


        private bool FastPlayRateStepDown()
        {
            return mView.TransportBar.FastPlayRateStepDown();
        }

        private bool FastPlayRateStepUp()
        {
            return mView.TransportBar.FastPlayRateStepUp();
        }

        private bool FastPlayRateNormalise()
        {
            return mView.TransportBar.FastPlayRateNormalise();
        }

        private bool FastPlayNormaliseWithLapseBack()
        {
            return mView.TransportBar.FastPlayNormaliseWithLapseBack();
        }

        #endregion

        public void SelectAtCurrentTime() { mPlaybackBlock.SelectAtCurrentTime(); }

        /// <summary>
        /// Get the parent for adding a new block depending on the current selection.
        /// </summary>
        public ObiNode ParentForNewBlock()
        {
            return mSelection == null ? null :
                mSelection.Node is SectionNode ? mSelection.Node : mSelection.Node.ParentAs<ObiNode>();
        }

        /// <summary>
        /// Get the index at which to insert a new block depending on the current selection.
        /// </summary>
        /// <returns></returns>
        public int IndexForNewBlock()
        {
            return mSelection == null ? -1 :
                mSelection.Node is SectionNode ? mSelection.Node.PhraseChildCount : (mSelection.Node.Index + 1);
        }
    }
}