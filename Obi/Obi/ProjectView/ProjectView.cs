using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class ProjectView : UserControl
    {
        private bool mEnableTooltips;            // tooltips flag
        private Presentation mPresentation;      // presentation
        private NodeSelection mSelection;        // currently selected node
        private Clipboard mClipboard;            // the clipboard
        private bool mSynchronizeViews;          // synchronize views flag
        private ObiForm mForm;                   // parent form

        public event EventHandler SelectionChanged;             // triggered when the selection changes
        public event EventHandler FindInTextVisibilityChanged;  // triggered when the search bar is shown or hidden
        public event ImportingFileEventHandler ImportingFile;   // triggered when a file is being imported
        public event EventHandler FinishedImportingFiles;       // triggered when all files were imported


        /// <summary>
        /// Create a new project view with no project yet.
        /// </summary>
        public ProjectView()
        {
            InitializeComponent();
            mTOCView.ProjectView = this;
            mStripsView.ProjectView = this;
            mFindInText.ProjectView = this;
            mTransportBar.ProjectView = this;
            mTransportBar.Enabled = false;
            mTOCViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel1Collapsed;
            mMetadataViewVisible = !mTOCSplitter.Panel1Collapsed && !mMetadataSplitter.Panel2Collapsed;
            mPresentation = null;
            mSelection = null;
            mForm = null;
            mClipboard = null;
        }


        /// <summary>
        /// Contents of the clipboard
        /// </summary>
        public Clipboard Clipboard
        {
            get { return mClipboard; }
            set { mClipboard = value; }
        }

        /// <summary>
        /// Enable/disable tooltips in the view (currently mostly disabled.)
        /// </summary>
        public bool EnableTooltips
        {
            get { return mEnableTooltips; }
            set
            {
                mEnableTooltips = value;
                // mStripManagerPanel.EnableTooltips = value;
                // mTOCPanel.EnableTooltips = value;
                // mTransportBar.EnableTooltips = value;
            }
        }

        /// <summary>
        /// Get the next page number for the selected block.
        /// </summary>
        public int NextPageNumber { get { return mPresentation.PageNumberFor(mSelection.Node); }  }

        /// <summary>
        /// The parent form as an Obi form.
        /// </summary>
        public ObiForm ObiForm
        {
            get { return mForm; }
            set { mForm = value; }
        }

        /// <summary>
        /// Set the presentation that the project view displays.
        /// </summary>
        public Presentation Presentation
        {
            get { return mPresentation; }
            set
            {
                ProjectVisible = value != null;
                if (mPresentation != value)
                {
                    mPresentation = value;
                    mTransportBar.Enabled = mPresentation != null;
                    if (mPresentation != null)
                    {
                        mTOCView.SetNewPresentation();
                        mStripsView.NewPresentation();
                        mTransportBar.NewPresentation();
                    }
                }
            }
        }




        /// <summary>
        /// Show or hide the project display.
        /// </summary>
        private bool ProjectVisible
        {
            set
            {
                mTransportBarSplitter.Visible = value;
                mNoProjectLabel.Visible = !value;
            }
        }

        /// <summary>
        /// If the current selection is a section, return it, otherwise return null.
        /// </summary>
        public SectionNode SelectedSection
        {
            get
            {
                return mSelection == null ? // || mSelection.Control != mTOCPanel ?
                    null : mSelection.Node as SectionNode;
            }
        }

        /// <summary>
        /// The current selection in the view.
        /// </summary>
        public NodeSelection Selection
        {
            get { return mSelection; }
            set
            {
                System.Diagnostics.Debug.Print("Selection: `{0}' >>> `{1}'", mSelection, value);
                if (mSelection != value)
                {
                    // deselect if there was a selection in a different control
                    if (mSelection != null && (value == null || mSelection.Control != value.Control))
                    {
                        mSelection.Control.Selection = null;
                    }
                    // select in the control
                    mSelection = value;
                    if (mSelection != null)
                    {
                        if (mSelection.Control == mTOCView) TOCViewVisible = true;
                        else if (mSelection.Control == mMetadataView) MetadataViewVisible = true;
                        mSelection.Control.Selection = value;
                    }
                    if (SelectionChanged != null) SelectionChanged(this, new EventArgs());
                }
            }
        }


        /// <summary>
        /// Currently selected node, regardless of its type or where it is selected.
        /// Null if nothing is selected.
        /// </summary>
        public ObiNode SelectionNode
        {
            get { return mSelection == null ? null : mSelection.Node; }
        }

        /// <summary>
        /// Set the synchronize views flag for this view and resynchronize the views if necessary.
        /// </summary>
        public bool SynchronizeViews
        {
            set
            {
                mSynchronizeViews = value;
                if (mSynchronizeViews)
                {
                    // TODO depending on where the focus is, resync TOC-wise or strips-wise
                    // (now it is TOC-wise...)
                    mTOCView.ResyncViews();
                    if (mSelection != null && mSelection.Control == mTOCView)
                    {
                        mStripsView.MakeStripVisibleForSection(SelectedSection);
                    }
                }
                else
                {
                    mStripsView.UnsyncViews();
                }
            }
        }

        public TransportBar TransportBar { get { return mTransportBar; } }


        #region Strips

        /// <summary>
        /// True if a block is selected (and not its contents.)
        /// </summary>
        public bool IsBlockSelected
        {
            get
            {
                return mSelection != null && mSelection.GetType() == typeof(NodeSelection) && mSelection.Node is EmptyNode;
            }
        }

        /// <summary>
        /// The phrase node for the block selected in the strips view.
        /// Null if no strip is selected.
        /// TODO: we need a compound node kind for container blocks.
        /// </summary>
        public EmptyNode SelectedBlockNode
        {
            get { return mSelection == null ? null : mSelection.Node as EmptyNode; }
            set { Selection = value == null ? null : new NodeSelection(value, mStripsView); }
        }

        public PhraseNode SelectedPhraseNode
        {
            get { return mSelection == null ? null : mSelection.Node as PhraseNode; }
        }

        public PhraseNode PlaybackBlock { set { mStripsView.PlaybackBlock = value; } }

        /// <summary>
        /// The section node for the strip selected in the strips view.
        /// Null if no strip is selected.
        /// </summary>
        public SectionNode SelectedStripNode
        {
            get { return mSelection == null ? null : mSelection.Node as SectionNode; }
            set { Selection = value == null ? null : new NodeSelection(value, mStripsView); }
        }

        #endregion


        /// <summary>
        /// Add a new section node to the project.
        /// </summary>
        public void AddNewSection()
        {
            if (CanAddSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.AddNewSection(this, mTOCView.Selection == null ?
                    new DummySelection(mPresentation.RootNode, mTOCView) : mTOCView.Selection));
            }
        }

        /// <summary>
        /// Insert a new subsection in the book as the last child of the selected section node in the TOC view.
        /// </summary>
        public void AddNewSubSection()
        {
            if (CanAddSubSection)
            {
                // hack to simulate the dummy node
                SectionNode section = (SectionNode)mTOCView.Selection.Node;
                mPresentation.UndoRedoManager.execute(new Commands.TOC.AddNewSection(this,
                    section.SectionChildCount > 0 ?
                    new NodeSelection(section.SectionChild(section.SectionChildCount - 1), mTOCView) :
                    new DummySelection(mTOCView.Selection.Node, mTOCView)));
            }
        }

        /// <summary>
        /// Add a new strip after, and at the same level as, the selected strip
        /// </summary>
        public void AddNewStrip()
        {
            if (CanAddStrip) { mPresentation.UndoRedoManager.execute(new Commands.Strips.AddNewStrip(this)); }
        }

        /// <summary>
        /// Select the name field of the selected section and start editing it.
        /// </summary>
        public void StartRenamingSelectedSection()
        {
            if (CanRenameSection) mTOCView.SelectAndRename(mTOCView.Selection.Section);
        }

        /// <summary>
        /// Select the label of the strip and start editing it.
        /// </summary>
        public void StartRenamingSelectedStrip()
        {
            if (CanRenameStrip) mStripsView.SelectAndRename(mStripsView.Selection.Section);
        }

        /// <summary>
        /// Split a strip at the selected position.
        /// </summary>
        public void SplitStrip()
        {
            if (CanSplitStrip) mPresentation.UndoRedoManager.execute(mStripsView.SplitStripFromSelectedBlockCommand());
        }

        /// <summary>
        /// Merge the selection strip with the next one, i.e. either its first sibling or its first child.
        /// </summary>
        public void MergeStrips()
        {
            if (CanMergeStripWithNext) mPresentation.UndoRedoManager.execute(mStripsView.MergeSelectedStripWithNextCommand());
        }


        /// <summary>
        /// Move the selected section node out.
        /// </summary>
        public void MoveSelectedSectionOut()
        {
            if (CanMoveSectionOut)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.MoveSectionOut(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Move the selected section node in.
        /// </summary>
        public void MoveSelectedSectionIn()
        {
            if (CanMoveSectionIn)
            {
                mPresentation.UndoRedoManager.execute(new Commands.TOC.MoveSectionIn(this, mTOCView.Selection.Section));
            }
        }

        /// <summary>
        /// Change the used status of the selected section, and of all its subsections.
        /// </summary>
        public void MarkSectionUsed(bool used)
        {
            if (CanToggleSectionUsed)
            {
                SectionNode section = mTOCView.Selection.Section;
                if (section.Used != used) mPresentation.UndoRedoManager.execute(new Commands.TOC.ToggleSectionUsed(this, section));
            }
        }

        /// <summary>
        /// Cut (delete) the selection and store it in the clipboard.
        /// </summary>
        public void Cut()
        {
            if (CanRemoveSection || CanRemoveStrip)
            {
                bool isSection = mSelection.Control is TOCView;
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message(isSection ? "cut_section_command" : "cut_strip_command"));
                command.append(new Commands.Node.Copy(this, isSection));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.UndoRedoManager.execute(command);
            }
            else if (CanRemoveBlock)
            {
                urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                command.setShortDescription(Localizer.Message("cut_block_command"));
                command.append(new Commands.Node.Copy(this, true));
                command.append(new Commands.Node.Delete(this, mSelection.Node));
                mPresentation.UndoRedoManager.execute(command);
            }
        }

        /// <summary>
        /// Copy the current selection into the clipboard. Noop if there is no selection.
        /// </summary>
        public void Copy()
        {
            if (CanCopySection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_section_command")));
            }
            else if (CanCopyStrip)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, false, Localizer.Message("copy_strip_command")));
            }
            else if (CanCopyBlock)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Copy(this, true, Localizer.Message("copy_block_command")));
            }
        }

        /// <summary>
        /// Paste the contents of the clipboard in the current context. Noop if the clipboard is empty.
        /// </summary>
        public void Paste()
        {
            if (CanPaste)
            {
                Commands.Node.Paste paste = new Commands.Node.Paste(this);
                if (mSelection.Control is StripsView && mSelection.Node.SectionChildCount > 0)
                {
                    urakawa.undo.CompositeCommand command = mPresentation.getCommandFactory().createCompositeCommand();
                    for (int i = 0; i < mSelection.Node.SectionChildCount; ++i)
                    {
                        command.append(new Commands.Node.ChangeParent(this, mSelection.Node.SectionChild(i), paste.Copy));
                    }
                    command.append(paste);
                    command.setShortDescription(paste.getShortDescription());
                    mPresentation.UndoRedoManager.execute(command);
                }
                else
                {
                    mPresentation.UndoRedoManager.execute(paste);
                }
            }
        }

        /// <summary>
        /// Delete the current selection. Noop if there is no selection.
        /// </summary>
        public void Delete()
        {
            if (CanRemoveSection)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Delete(this, mTOCView.Selection.Section,
                    Localizer.Message("delete_section_command")));
            }
            else if (CanRemoveStrip)
            {
                mPresentation.UndoRedoManager.execute(mStripsView.DeleteStripCommand());
            }
            else if (CanRemoveBlock)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.Delete(this, mStripsView.Selection.Phrase,
                    Localizer.Message("delete_block_command")));
            }
        }

        /// <summary>
        /// Select a section node in the TOC view.
        /// </summary>
        public void SelectInTOCView(SectionNode section) { Selection = new NodeSelection(section, mTOCView); }

        /// <summary>
        /// Select a section or strip and start renaming it.
        /// </summary>
        public void SelectAndRenameSelection(NodeSelection selection)
        {
            if (selection.Control is IControlWithRenamableSelection)
            {
                ((IControlWithRenamableSelection)selection.Control).SelectAndRename(selection.Node);
            }
        }

        public void RenameSectionNode(SectionNode section, string label)
        {
            mPresentation.UndoRedoManager.execute(new Commands.Node.RenameSection(this, section, label));
        }

        private bool mTOCViewVisible;  // keep track of the TOC view visibility (don't reopen it accidentally)

        /// <summary>
        /// Show or hide the TOC view.
        /// </summary>
        public bool TOCViewVisible
        {
            get { return mTOCViewVisible; }
            set
            {
                mTOCViewVisible = value;
                if (value)
                {
                    mTOCSplitter.Panel1Collapsed = false;
                    mMetadataSplitter.Panel2Collapsed = !MetadataViewVisible;
                }
                else
                {
                    if (mSelection != null && mSelection.Control == mTOCView) Selection = null;
                    if (!MetadataViewVisible) mTOCSplitter.Panel1Collapsed = true;
                }
                mMetadataSplitter.Panel1Collapsed = !value;
            }
        }

        private bool mMetadataViewVisible;  // keep track of the Metadata view visibility

        /// <summary>
        /// Show or hide the Metadata view.
        /// </summary>
        public bool MetadataViewVisible
        {
            get { return mMetadataViewVisible; }
            set
            {
                mMetadataViewVisible = value;
                if (value)
                {
                    mTOCSplitter.Panel1Collapsed = false;
                    mMetadataSplitter.Panel1Collapsed = !TOCViewVisible;
                }
                else if (!value && !TOCViewVisible) mTOCSplitter.Panel1Collapsed = true;
                mMetadataSplitter.Panel2Collapsed = !value;
            }
        }

        /// <summary>
        /// Show or hide the search bar.
        /// </summary>
        public bool FindInTextVisible
        {
            get { return !mFindInTextSplitter.Panel2Collapsed; }
            set 
            {
                bool isVisible = !mFindInTextSplitter.Panel2Collapsed;
                if (isVisible != value) mFindInTextSplitter.Panel2Collapsed = !value;
                if (FindInTextVisibilityChanged != null) FindInTextVisibilityChanged(this, null);
            }
        }

        /// <summary>
        /// Show or hide the transport bar.
        /// </summary>
        public bool TransportBarVisible
        {
            get { return !mTransportBarSplitter.Panel2Collapsed; }
            set
            {
                bool isVisible = !mTransportBarSplitter.Panel2Collapsed;
                if (isVisible != value) mTransportBarSplitter.Panel2Collapsed = !value;
            }
        }


        public bool CanCut { get { return CanDelete; } }
        public bool CanCopy { get { return CanCopySection || CanCopyStrip || CanCopyBlock; } }
        public bool CanDelete { get { return CanRemoveSection || CanRemoveStrip || CanRemoveBlock; } }
        public bool CanPaste { get { return mSelection != null && mSelection.CanPaste(mClipboard); } }
        public bool CanDeselect { get { return mSelection != null; } }

        public bool CanShowInStripsView { get { return SelectedSection != null && mSelection.Control == mTOCView; } }
        public bool CanShowInTOCView { get { return SelectedSection != null && mSelection.Control == mStripsView; } }

        public bool CanAddEmptyBlock { get { return mStripsView.Selection != null; } }
        public bool CanAddSection { get { return mTOCView.CanAddSection && !mStripsView.CanAddStrip; } }
        public bool CanAddStrip { get { return mStripsView.CanAddStrip; } }
        public bool CanAddSubSection { get { return mTOCView.CanAddSection && mTOCView.Selection != null; } }
        public bool CanCopySection { get { return mTOCView.CanCopySection; } }
        public bool CanCopyStrip { get { return mStripsView.CanCopyStrip; } }
        public bool CanCopyBlock { get { return mStripsView.CanCopyBlock; } }
        public bool CanMarkSectionUnused { get { return mTOCView.CanToggleSectionUsed && mTOCView.Selection.Node.Used; } }
        public bool CanMergeBlockWithNext { get { return mStripsView.CanMergeBlockWithNext; } }
        public bool CanMergeStripWithNext { get { return mStripsView.CanMergeStripWithNext; } }
        public bool CanMoveSectionIn { get { return mTOCView.CanMoveSectionIn; } }
        public bool CanMoveSectionOut { get { return mTOCView.CanMoveSectionOut; } }
        public bool CanRemoveBlock { get { return mStripsView.CanRemoveBlock; } }
        public bool CanRemoveSection { get { return mTOCView.CanRemoveSection; } }
        public bool CanRemoveStrip { get { return mStripsView.CanRemoveStrip; } }
        public bool CanRenameSection { get { return mTOCView.CanRenameSection; } }
        public bool CanRenameStrip { get { return mStripsView.CanRenameStrip; } }
        public bool CanSplitBlock { get { return mSelection is AudioSelection; } }
        public bool CanSplitStrip { get { return mStripsView.CanSplitStrip; } }
        public bool CanToggleSectionUsed { get { return mTOCView.CanToggleSectionUsed; } }

        /// <summary>
        /// Show the strip for the given section
        /// </summary>
        public void MakeStripVisibleForSection(SectionNode section)
        {
            if (mSynchronizeViews) mStripsView.MakeStripVisibleForSection(section);
        }

        /// <summary>
        /// Show the tree node in the TOC view for the given section
        /// </summary>
        public void MakeTreeNodeVisibleForSection(SectionNode section)
        {
            if (mSynchronizeViews) mTOCView.MakeTreeNodeVisibleForSection(section);
        }

        /// <summary>
        /// Show/hide strips for nodes that were collapsed/expanded when the views are synchronized
        /// </summary>
        public void SetStripsVisibilityForSection(SectionNode section, bool visible)
        {
            if (mSynchronizeViews) mStripsView.SetStripsVisibilityForSection(section, visible);
        }

        public void SetStripVisibilityForSection(SectionNode section, bool visible)
        {
            if (mSynchronizeViews) mStripsView.SetStripVisibilityForSection(section, visible);
        }

        /// <summary>
        /// Show (select) the section node for the current selection
        /// </summary>
        public void ShowSelectedSectionInTOCView()
        {
            if (CanShowInTOCView)
            {
                Selection = new NodeSelection(mSelection.Node, mTOCView);
            }
            else
            {
                TOCViewVisible = true;
                mTOCView.Focus();
            }
        }

        /// <summary>
        /// Show (select) the strip for the current selection
        /// </summary>
        public void ShowSelectedSectionInStripsView()
        {
            if (CanShowInStripsView)
            {
                Selection = new NodeSelection(mSelection.Node, mStripsView);
            }
            else
            {
                mStripsView.GetFocus();
            }
        }

        #region Find in Text

        public void FindInText()
        {
            //show the form if it's not already shown
            if (mFindInTextSplitter.Panel2Collapsed == true) mFindInTextSplitter.Panel2Collapsed = false;
            FindInTextVisible = true;
            //iterating over the layout panel seems to be the way to search the sections 
            mFindInText.StartNewSearch(mStripsView.LayoutPanel);
        }

        public void FindNextInText()
        {
            if (FindInTextVisible) mFindInText.FindNext();
        }

        public void FindPreviousInText()
        {
            if (FindInTextVisible) mFindInText.FindPrevious();
        }

        public bool CanFindNextPreviousText
        {
            get { return mFindInText.CanFindNextPreviousText; }
        }
        #endregion


        public void ListenToSelection() { }
        public bool CanListenToSection { get { return mTransportBar.Enabled && mTOCView.Selection != null; } }
        public bool CanListenToStrip { get { return mTransportBar.Enabled && mStripsView.SelectedSection != null; } }
        public bool CanListenToBlock { get { return mTransportBar.Enabled && mStripsView.SelectedPhrase != null; } }

        // Blocks


        /// <summary>
        /// Add a new empty block.
        /// </summary>
        public void AddEmptyBlock()
        {
            if (CanAddEmptyBlock)
            {
                EmptyNode node = new EmptyNode(mPresentation);
                mPresentation.UndoRedoManager.execute(new Commands.Node.AddEmptyNode(this, node,
                    mStripsView.Selection.ParentForNewNode(node), mStripsView.Selection.IndexForNewNode(node)));
            }
        }

        /// <summary>
        /// Import new phrases in the strip, one block per file.
        /// </summary>
        public void ImportPhrases()
        {
            if (CanImportPhrases)
            {
                string[] paths = SelectFilesToImport();
                List<PhraseNode> phrases = new List<PhraseNode>(paths.Length);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(delegate(object sender, DoWorkEventArgs e)
                {
                    CreatePhrasesForFiles(phrases, paths);
                });
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(
                    delegate(object sender, RunWorkerCompletedEventArgs e)
                    {
                        if (phrases.Count > 0) mPresentation.UndoRedoManager.execute(new Commands.Strips.ImportPhrases(this, phrases));
                    });
                worker.RunWorkerAsync();
            }
        }

        public bool CanImportPhrases { get { return mStripsView.Selection != null; } }

        /// <summary>
        /// Bring up the file chooser to select audio files to import and return new phrase nodes for the selected files,
        /// or null if nothing was selected.
        /// </summary>
        private string[] SelectFilesToImport()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message("audio_file_filter");
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : new string[0];
        }

        private void CreatePhrasesForFiles(List<PhraseNode> phrases, string[] paths)
        {
            foreach (string path in paths)
            {
                if (ImportingFile != null) ImportingFile(this, new ImportingFileEventArgs(path));
                try
                {
                    phrases.Add(mPresentation.CreatePhraseNode(path));
                }
                catch (Exception)
                {
                    MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                        Localizer.Message("import_phrase_error_caption"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            if (FinishedImportingFiles != null) FinishedImportingFiles(this, null);
        }

        public void SelectNothing() { Selection = null; }

        public void SetCustomTypeForSelectedBlock(EmptyNode.Kind kind, string custom)
        {
            if (IsBlockSelected)
            {
                mPresentation.UndoRedoManager.execute(new Commands.Node.ChangeCustomType(this, SelectedBlockNode, kind, custom));
            }
        }

        public void SplitBlock()
        {
            if (CanSplitBlock) mPresentation.UndoRedoManager.execute(new Commands.Node.SplitAudio(this));
        }

        public void MergeBlockWithNext()
        {
            if (CanMergeBlockWithNext) mPresentation.UndoRedoManager.execute(new Commands.Node.MergeAudio(this));
        }

        public void MakeBlockIntoContainer()
        {
            //there is a problem with this because the treenode_added event gets thrown for each step
            //and Obi isn't used to dealing with empty nodes yet (which happens after cmd1 below gets executed)
            if (SelectedBlockNode != null)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();
                Commands.Node.AddContainer cmd1 = new Obi.Commands.Node.AddContainer(this, SelectedBlockNode.ParentAs<ObiNode>(), SelectedBlockNode.Index);
                Commands.Node.ChangeParent cmd2 = new Obi.Commands.Node.ChangeParent(this, SelectedBlockNode, cmd1.Container);
                command.append(cmd1);
                command.append(cmd2);
                mPresentation.UndoRedoManager.execute(command);
            }
        }

        //TODO: put this code into a command
        public void RemoveContainer()
        {
            if (SelectedBlockNode != null)
            {
                //here, selected block node is the container itself
                int index = SelectedBlockNode.Index;
                ObiNode parentNode = SelectedBlockNode.ParentAs<ObiNode>();
                for (int i = 0; i < SelectedBlockNode.PhraseChildCount; i++)
                {
                    PhraseNode node = (PhraseNode)SelectedBlockNode.PhraseChild(0).Detach();
                    parentNode.Insert(node, index + i);
                }
                parentNode.RemoveChild(SelectedBlockNode);
            }
        }

        public void MakeSelectedBlockIntoSilencePhrase()
        {
            EmptyNode node = SelectedBlockNode;
            if (node != null)
            {
                urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();
                Commands.Node.ChangeCustomType silence = new Commands.Node.ChangeCustomType(this, node, EmptyNode.Kind.Silence);
                command.append(silence);
                command.setShortDescription(silence.getShortDescription());
                if (node.Used) command.append(new Commands.Node.ToggleNodeUsed(this, node));
                Presentation.UndoRedoManager.execute(command);
            }
        }

        public void MakeSelectedBlockIntoHeadingPhrase()
        {
            if (SelectedBlockNode != null)
            {
                if (SelectedBlockNode != null)
                {
                    urakawa.undo.CompositeCommand command = Presentation.getCommandFactory().createCompositeCommand();

                    //1. clear existing custom type
                    Commands.Node.ChangeCustomType cmd1 = new Obi.Commands.Node.ChangeCustomType(this, SelectedBlockNode, EmptyNode.Kind.Plain);
                    //2. unset existing heading on section
                    EmptyNode node = SelectedBlockNode.AncestorAs<SectionNode>().Heading;
                    Commands.Node.UnsetNodeAsHeadingPhrase cmd2 = new Obi.Commands.Node.UnsetNodeAsHeadingPhrase(this, node);
                    //3. set new heading
                    Commands.Node.SetNodeAsHeadingPhrase cmd3 = new Obi.Commands.Node.SetNodeAsHeadingPhrase(this, SelectedPhraseNode);
                    //4. assign new custom type as "heading"
                    Commands.Node.ChangeCustomType cmd4 = new Obi.Commands.Node.ChangeCustomType(this, SelectedBlockNode, EmptyNode.Kind.Heading, null);
                    command.setShortDescription(cmd4.getShortDescription());
                    command.append(cmd1);
                    command.append(cmd2);
                    command.append(cmd3);
                    command.append(cmd4);
                    mPresentation.UndoRedoManager.execute(command);
                 }     
            }
        }
        public void UpdateCursorPosition(double time) { mStripsView.UpdateCursorPosition(time); }
        public void SelectAtCurrentTime() { mStripsView.SelectAtCurrentTime(); }


        /// <summary>
        /// Used for adding custom types on the fly: add it to the presentation and also set it on the block
        /// </summary>
        /// <param name="customName"></param>
        /// <param name="kind"></param>
        public void AddCustomTypeAndSetOnBlock(EmptyNode.Kind nodeKind, string customClass)
        {
            if (IsBlockSelected)
            {
                EmptyNode node = SelectedBlockNode;
                if (node.NodeKind != nodeKind || node.CustomClass != customClass)
                {
                    mPresentation.UndoRedoManager.execute(new Obi.Commands.Node.ChangeCustomType(this, node, customClass));
                }
            }
        }

        public bool CanSetPageNumber { get { return IsBlockSelected; } } 

        public void SetPageNumberOnSelectedBock(int number, bool renumber)
        {
            if (CanSetPageNumber)
            {
                urakawa.undo.ICommand cmd = new Commands.Node.SetPageNumber(this, SelectedBlockNode, number);
                if (renumber)
                {
                    urakawa.undo.CompositeCommand k = Presentation.getCommandFactory().createCompositeCommand();
                    k.setShortDescription(cmd.getShortDescription());
                    for (ObiNode n = SelectedBlockNode.FollowingNode; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                        {
                            k.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, ++number));
                        }
                    }
                    k.append(cmd);
                    cmd = k;
                }
                mPresentation.UndoRedoManager.execute(cmd);
            }
        }

        public void AddPageRange(int number, int count, bool renumber)
        {
            if (CanAddEmptyBlock)
            {
                urakawa.undo.CompositeCommand cmd = Presentation.getCommandFactory().createCompositeCommand();
                cmd.setShortDescription(Localizer.Message("add_empty_page_blocks"));
                ObiNode parent = mSelection.ParentForNewBlock();
                int index = mSelection.IndexForNewBlock();
                for (int i = 0; i < count; ++i)
                {
                    EmptyNode node = new EmptyNode(Presentation);
                    cmd.append(new Commands.Node.AddEmptyNode(this, node, parent, index + i));
                    cmd.append(new Commands.Node.SetPageNumber(this, node, number++));
                }
                if (renumber)
                {
                    ObiNode from = index < parent.getChildCount() ? (ObiNode)parent.getChild(index) : parent;
                    for (ObiNode n = from; n != null; n = n.FollowingNode)
                    {
                        if (n is EmptyNode && ((EmptyNode)n).NodeKind == EmptyNode.Kind.Page)
                        {
                            cmd.append(new Commands.Node.SetPageNumber(this, (EmptyNode)n, ++number));
                        }
                    }
                }
                mPresentation.UndoRedoManager.execute(cmd);
            }
        }
    }

    public class ImportingFileEventArgs
    {
        public string Path;  // path of the file being imported
        public ImportingFileEventArgs(string path) { Path = path; }
    }

    public delegate void ImportingFileEventHandler(object sender, ImportingFileEventArgs e);

}