using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.UserControls
{
    /// <summary>
    /// Top level panel that displays the current project, using a splitter (TOC on the left, strips on the right.)
    /// </summary>
    public partial class ProjectPanel : UserControl
    {
        private Project mProject;  // the project to display

        /// <summary>
        /// Set the project for the panel, as well as all the correct handlers.
        /// </summary>
        public Project Project
        {
            get { return mProject; }
            set
            {
                if (mProject != null) UnsetEventHandlers();
                if (value != null) SetEventHandlers(value);
                mProject = value;
                mSplitContainer.Visible = mProject != null;
                mSplitContainer.Panel1Collapsed = false;
                mNoProjectLabel.Text = mProject == null ? Localizer.Message("no_project") : "";
                mTransportBar.Visible = mProject != null;
            }
        }

        /// <summary>
        /// Set event handlers for the new project.
        /// </summary>
        /// <param name="project">The new project.</param>
        private void SetEventHandlers(Project project)
        {
            //md test 20061207
            mStripManagerPanel.SelectionChanged += new Events.SelectedHandler(this.test_WidgetSelect);
            mTOCPanel.SelectionChanged += new Events.SelectedHandler(this.test_WidgetSelect);

            mTOCPanel.AddSiblingSectionRequested += new Events.SectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSectionRequested += new Events.SectionNodeHandler(project.CreateSiblingSectionNodeRequested);
            project.AddedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            project.AddedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

            mTOCPanel.AddChildSectionNodeRequested += new Events.SectionNodeHandler(project.CreateChildSectionNodeRequested);

            project.MovedSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.MovedSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);

            project.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            project.UndidMoveSectionNode += new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);

            mTOCPanel.IncreaseSectionNodeLevelRequested += new Events.SectionNodeHandler(project.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.DecreaseSectionNodeLevelRequested += new Events.SectionNodeHandler(project.DecreaseSectionNodeLevelRequested);
            project.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
            project.DecreasedSectionNodeLevel += new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);

            mTOCPanel.RenameSectionNodeRequested += new Events.RenameSectionNodeHandler(project.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSectionRequested += new Events.RenameSectionNodeHandler(project.RenameSectionNodeRequested);
            project.RenamedSectionNode += new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            project.RenamedSectionNode += new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);

            mTOCPanel.DeleteSectionNodeRequested += new Events.SectionNodeHandler(project.RemoveSectionNodeRequested);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            project.DeletedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);

            // Block events

            mStripManagerPanel.RequestToCutSectionNode += new Events.SectionNodeHandler(project.CutSectionNodeRequested);
            mStripManagerPanel.RequestToCutPhraseNode += new Events.PhraseNodeHandler(project.CutPhraseNode);
            mStripManagerPanel.RequestToCopyPhraseNode += new Events.PhraseNodeHandler(project.CopyPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode += new Events.NodeEventHandler(project.PastePhraseNode);

            mStripManagerPanel.ImportAudioAssetRequested += new Events.RequestToImportAssetHandler(project.ImportAssetRequested);
            mStripManagerPanel.DeleteBlockRequested += new Events.PhraseNodeHandler(project.DeletePhraseNodeRequested);
            mStripManagerPanel.MoveAudioBlockForwardRequested += new Events.PhraseNodeHandler(project.MovePhraseNodeForwardRequested);
            mStripManagerPanel.MoveAudioBlockBackwardRequested += new Events.PhraseNodeHandler(project.MovePhraseNodeBackwardRequested);
            mStripManagerPanel.SetMediaRequested += new Events.SetMediaHandler(project.SetMediaRequested);
            mStripManagerPanel.SplitAudioBlockRequested += new Events.SplitPhraseNodeHandler(project.SplitAudioBlockRequested);
            mStripManagerPanel.RequestToApplyPhraseDetection += new Events.RequestToApplyPhraseDetectionHandler(project.ApplyPhraseDetection);

            project.AddedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);
            project.DeletedPhraseNode += new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);
            project.MediaSet += new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);
            project.TouchedNode += new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            project.UpdateTime += new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            mStripManagerPanel.MergeNodes += new Events.MergePhraseNodesHandler(project.MergeNodesRequested);

            //md: clipboard in the TOC
            mTOCPanel.CutSectionNodeRequested += new Events.SectionNodeHandler(project.CutSectionNodeRequested);
            project.CutSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            project.CutSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);

            mTOCPanel.CopySectionNodeRequested += new Events.SectionNodeHandler(project.CopySectionNodeRequested);
            project.CopiedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            project.CopiedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            project.UndidCopySectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            project.UndidCopySectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

            mTOCPanel.PasteSectionNodeRequested += new Events.SectionNodeHandler(project.PasteSectionNodeRequested);
            project.PastedSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            project.PastedSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            project.UndidPasteSectionNode += new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);


            //md 20060812
            mStripManagerPanel.RequestToShallowDeleteSectionNode += new Events.SectionNodeHandler(project.ShallowDeleteSectionNodeRequested);

            mStripManagerPanel.RequestToSetPageNumber += new Events.RequestToSetPageNumberHandler(project.SetPageRequested);
            mStripManagerPanel.RequestToRemovePageNumber += new Events.PhraseNodeHandler(project.RemovePageRequested);
            project.RemovedPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            project.SetPageNumber += new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            mTOCPanel.ToggleSectionNodeUsedRequested += new Obi.Events.SectionNodeHandler(project.ToggleSectionUsedStateRequested);
            project.ToggledSectionUsedState += new Obi.Events.SectionNodeHandler(mTOCPanel.ToggledSectionUsedState);
        }

        /// <summary>
        /// Unset event handlers from the old project (still set.)
        /// </summary>
        private void UnsetEventHandlers()
        {
            //md test 20061207
            mStripManagerPanel.SelectionChanged -= new Events.SelectedHandler(this.test_WidgetSelect);
            mTOCPanel.SelectionChanged -= new Events.SelectedHandler(this.test_WidgetSelect);

            mTOCPanel.AddSiblingSectionRequested -= new Events.SectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);
            mStripManagerPanel.AddSiblingSectionRequested -=
                new Events.SectionNodeHandler(mProject.CreateSiblingSectionNodeRequested);

            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncAddedSectionNode);
            mProject.AddedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncAddedSectionNode);

            mTOCPanel.AddChildSectionNodeRequested -= new Events.SectionNodeHandler(mProject.CreateChildSectionNodeRequested);

            mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.MovedSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);
            mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mTOCPanel.SyncMovedSectionNode);
            mProject.UndidMoveSectionNode -= new Events.MovedSectionNodeHandler(mStripManagerPanel.SyncMovedSectionNode);

            mTOCPanel.IncreaseSectionNodeLevelRequested -= new Events.SectionNodeHandler(mProject.IncreaseSectionNodeLevelRequested);
            //marisa: the former "mProject.IncreasedSectionLevel" event is now handled by MovedNode

            mTOCPanel.DecreaseSectionNodeLevelRequested -= new Events.SectionNodeHandler(mProject.DecreaseSectionNodeLevelRequested);
            mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mTOCPanel.SyncDecreasedSectionNodeLevel);
            mProject.DecreasedSectionNodeLevel -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDecreaseSectionNodeLevel);

            mTOCPanel.RenameSectionNodeRequested -= new Events.RenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mStripManagerPanel.RenameSectionRequested -= new Events.RenameSectionNodeHandler(mProject.RenameSectionNodeRequested);
            mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mTOCPanel.SyncRenamedSectionNode);
            mProject.RenamedSectionNode -= new Events.RenameSectionNodeHandler(mStripManagerPanel.SyncRenamedNode);

            mTOCPanel.DeleteSectionNodeRequested -= new Events.SectionNodeHandler(mProject.RemoveSectionNodeRequested);
            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncDeletedSectionNode);
            mProject.DeletedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncDeletedSectionNode);

            mStripManagerPanel.ImportAudioAssetRequested -= new Events.RequestToImportAssetHandler(mProject.ImportAssetRequested);
            //mProject.ImportedAsset -= new Events.Node.ImportedAssetHandler(mStripManagerPanel.SyncCreateNewAudioBlock);
            mProject.AddedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncAddedPhraseNode);

            mStripManagerPanel.SetMediaRequested -= new Events.SetMediaHandler(mProject.SetMediaRequested);
            mProject.MediaSet -= new Events.SetMediaHandler(mStripManagerPanel.SyncMediaSet);

            mStripManagerPanel.SplitAudioBlockRequested -= new Events.SplitPhraseNodeHandler(mProject.SplitAudioBlockRequested);
            mStripManagerPanel.RequestToApplyPhraseDetection -=
                new Events.RequestToApplyPhraseDetectionHandler(mProject.ApplyPhraseDetection);

            mStripManagerPanel.MergeNodes -= new Events.MergePhraseNodesHandler(mProject.MergeNodesRequested);

            mStripManagerPanel.DeleteBlockRequested -=
                new Events.PhraseNodeHandler(mProject.DeletePhraseNodeRequested);
            mProject.DeletedPhraseNode -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncDeleteAudioBlock);

            mTOCPanel.CutSectionNodeRequested -= new Events.SectionNodeHandler(mProject.CutSectionNodeRequested);
            mProject.CutSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncCutSectionNode);
            mProject.CutSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncCutSectionNode);

            mTOCPanel.CopySectionNodeRequested -= new Events.SectionNodeHandler(mProject.CopySectionNodeRequested);
            mProject.CopiedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncCopiedSectionNode);
            mProject.CopiedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncCopiedSectionNode);
            mProject.UndidCopySectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidCopySectionNode);
            mProject.UndidCopySectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidCopySectionNode);

            mTOCPanel.PasteSectionNodeRequested -= new Events.SectionNodeHandler(mProject.PasteSectionNodeRequested);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncPastedSectionNode);
            mProject.PastedSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncPastedSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mTOCPanel.SyncUndidPasteSectionNode);
            mProject.UndidPasteSectionNode -= new Events.SectionNodeHandler(mStripManagerPanel.SyncUndidPasteSectionNode);

            mProject.TouchedNode -= new Events.NodeEventHandler(mStripManagerPanel.SyncTouchedNode);
            mProject.UpdateTime -= new Events.UpdateTimeHandler(mStripManagerPanel.SyncUpdateAudioBlockTime);

            //md 20060812
            mStripManagerPanel.RequestToShallowDeleteSectionNode -= new Events.SectionNodeHandler(mProject.ShallowDeleteSectionNodeRequested);

            mStripManagerPanel.RequestToCutSectionNode -=
                new Events.SectionNodeHandler(mProject.CutSectionNodeRequested);
            mStripManagerPanel.RequestToCutPhraseNode -=
                new Events.PhraseNodeHandler(mProject.CutPhraseNode);
            mStripManagerPanel.RequestToCopyPhraseNode -=
                new Events.PhraseNodeHandler(mProject.CopyPhraseNode);
            mStripManagerPanel.RequestToPastePhraseNode -=
                new Events.NodeEventHandler(mProject.PastePhraseNode);
            mStripManagerPanel.RequestToSetPageNumber -= new Events.RequestToSetPageNumberHandler(mProject.SetPageRequested);
            mStripManagerPanel.RequestToRemovePageNumber -=
                new Events.PhraseNodeHandler(mProject.RemovePageRequested);
            mProject.RemovedPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncRemovedPageNumber);
            mProject.SetPageNumber -= new Events.PhraseNodeHandler(mStripManagerPanel.SyncSetPageNumber);

            mTOCPanel.ToggleSectionNodeUsedRequested -= new Obi.Events.SectionNodeHandler(mProject.ToggleSectionUsedStateRequested);
            mProject.ToggledSectionUsedState -= new Obi.Events.SectionNodeHandler(mTOCPanel.ToggledSectionUsedState);
        }

        /// <summary>
        /// TOC panel can be visible (true) or hidden (false).
        /// </summary>
        public Boolean TOCPanelVisible
        {
            get { return mProject != null && !mSplitContainer.Panel1Collapsed; }
        }

        /// <summary>
        /// The strip manager for this project.
        /// </summary>
        public StripManagerPanel StripManager
        {
            get { return mStripManagerPanel; }
        }

        /// <summary>
        /// The TOC panel for this project.
        /// </summary>
        public TOCPanel TOCPanel
        {
            get { return mTOCPanel; }
        }

        /// <summary>
        /// The transport bar for the project.
        /// </summary>
        public TransportBar TransportBar
        {
            get { return mTransportBar; }
        }

        /// <summary>
        /// Return the node that is selected in either view, if any.
        /// </summary>
        public ObiNode SelectedNode
        {
            get
            {
                return mStripManagerPanel.SelectedNode != null ?
                        mStripManagerPanel.SelectedNode :
                    mTOCPanel.IsNodeSelected ?
                        mTOCPanel.SelectedSection : null;
            }
        }

        /// <summary>
        /// Create a new project panel with currently no project.
        /// </summary>
        public ProjectPanel()
        {
            InitializeComponent();
            mTOCPanel.ProjectPanel = this;
            mStripManagerPanel.ProjectPanel = this;
            Project = null;
        }

        /// <summary>
        /// Hide the TOC panel.
        /// </summary>
        public void HideTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = true;
        }

        /// <summary>
        /// Show the TOC panel.
        /// </summary>
        public void ShowTOCPanel()
        {
            mSplitContainer.Panel1Collapsed = false;
        }

        /// <summary>
        /// Synchronize the project views with the core tree and initialize the playlist for the transport bar.
        /// Used when opening a XUK file or touching the project.
        /// </summary>
        public void SynchronizeWithCoreTree()
        {
            mTOCPanel.SynchronizeWithCoreTree(mProject.RootNode);
            mStripManagerPanel.SynchronizeWithCoreTree(mProject.RootNode);
            mTransportBar.Playlist = new Playlist(mProject, Audio.AudioPlayer.Instance);
        }

        //things to deselect:
        /*StripManagerPanel.mPhraseNodeMap[mSelectedPhrase] (audio blocks)
	    StripManagerPanel.mSectionNodeMap[mSelectedSection] (section strip)
	    annotation block (future)
	    page block (future)
	    TOC panel node	*/
        //IMPORTANT! don't raise anything like "SelectionChanged" events
        internal void DeselectEverything()
        {
            mTOCPanel.SelectedSection = null;
            mStripManagerPanel.SelectedPhraseNode = null;// SelectedBlock = null;
            mStripManagerPanel.SelectedSectionNode = null;// SelectedSectionStrip = null;
        }
        
        internal void test_WidgetSelect(object sender, Obi.Events.Node.SelectedEventArgs e)
        {
            CoreNode target = null;

            if (e.Widget is Obi.UserControls.SectionStrip)
            {
                System.Diagnostics.Debug.Write("SectionStrip - ");
                Obi.UserControls.SectionStrip strip = (Obi.UserControls.SectionStrip)e.Widget;
                target = strip.Node;
            }
            else if (e.Widget is Obi.UserControls.AudioBlock)
            {
                System.Diagnostics.Debug.Write("AudioBlock - ");
                Obi.UserControls.AudioBlock block = (Obi.UserControls.AudioBlock)e.Widget;
                target = block.Node;
            }
            else if (e.Widget is System.Windows.Forms.TreeNode)
            {
                System.Diagnostics.Debug.Write("TOC.TreeNode - ");
                System.Windows.Forms.TreeNode treenode = (System.Windows.Forms.TreeNode)e.Widget;
                target = this.mTOCPanel.SelectedSection;
            }

            if (target != null) System.Diagnostics.Debug.Write(target.GetType().ToString() + ": ");
            else System.Diagnostics.Debug.Write("!target node is null");

            string text = "";
            if (target is SectionNode) text = Project.GetTextMedia((CoreNode)target).getText();
            if (target is PhraseNode) text = ((urakawa.media.TextMedia)Project.GetMediaForChannel((CoreNode)target, Project.AnnotationChannelName)).getText();
            System.Diagnostics.Debug.Write("\"" + text + "\"");
            if (e.Selected) System.Diagnostics.Debug.Write(" is selected\n");
            else System.Diagnostics.Debug.Write(" is deselected\n");
        }
    }
}
