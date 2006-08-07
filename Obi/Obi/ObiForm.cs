using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX.DirectSound;

using Obi.Dialogs;

namespace Obi
{
    /// <summary>
    /// The main for of the application.
    /// The form consists mostly of a menu bar and a project panel.
    /// We also keep an undo stack (the command manager) and settings.
    /// </summary>
    public partial class ObiForm : Form
    {
        private Project mProject;                  // the project currently being authored
        private Settings mSettings;                // application settings
        private Commands.CommandManager mCmdMngr;  // the undo stack for this project (should it belong to the project?)

        // filter for opening/saving XUK files
        public static readonly string XUKFilter = "Obi project file (*.xuk)|*.xuk|Any file|*.*";

        /// <summary>
        /// Initialize a new form. No project is opened at creation time.
        /// </summary>
        public ObiForm()
        {
            InitializeComponent();
            mProject = null;
            mSettings = null;
            mCmdMngr = new Commands.CommandManager();
            InitializeSettings();
            mOpenRecentProjectToolStripMenuItem.Enabled = mSettings.RecentProjects.Count > 0;
            FormUpdateClosedProject();  // no project opened, same as if we closed a project.
        }

        #region GUI event handlers

        /// <summary>
        /// Create a new project if the current one was closed properly, or if none was open.
        /// </summary>
        private void mNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.NewProject dialog = new Dialogs.NewProject(mSettings.DefaultPath);
            dialog.CreateTitleSection = mSettings.CreateTitleSection;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (ClosedProject())
                {
                    mProject = new Project();
                    mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                    mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
                    mProject.Create(dialog.Path, dialog.Title, mSettings.IdTemplate, mSettings.UserProfile,
                        dialog.CreateTitleSection);
                    mSettings.CreateTitleSection = dialog.CreateTitleSection;
                    AddRecentProject(mProject.XUKPath);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project from a XUK file by prompting the user for a file location.
        /// Try to close a possibly open project first.
        /// </summary>
        private void mOpenProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = XUKFilter;
                dialog.InitialDirectory = mSettings.DefaultPath;
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    OpenProject(dialog.FileName);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Clear the list of recently opened files (prompt the user first.)
        /// </summary>
        private void mClearListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(Localizer.Message("clear_recent_text"),
                Localizer.Message("clear_recent_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) ClearRecentList();
            Ready();
        }

        /// <summary>
        /// Save the current project under its current name, or ask for one if none is defined yet.
        /// </summary>
        private void mSaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                mProject.Save();
                mCmdMngr.Clear();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Save the project under a (presumably) different name.
        /// </summary>
        private void mSaveProjectasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = XUKFilter;
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                mProject.SaveAs(dialog.FileName);
                AddRecentProject(dialog.FileName);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Revert the project to its last saved state.
        /// </summary>
        private void mDiscardChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject.Unsaved)
            {
                DialogResult discard = MessageBox.Show(Localizer.Message("discard_changes_text"),
                    Localizer.Message("discard_changes_caption"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (discard == DialogResult.Yes)
                {
                    ForceOpenProject(mProject.XUKPath);
                }
                else
                {
                    Ready();
                }
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Close the current project.
        /// </summary>
        private void mCloseProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                mProject.Close();
                mProject = null;
                mCmdMngr.Clear();
            }
        }

        /// <summary>
        /// Exit if and only if the currently open project was saved correctly.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosedProject())
            {
                Application.Exit();
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Edit the metadata for the project.
        /// </summary>
        private void metadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProject != null)
            {
                Dialogs.EditSimpleMetadata dialog = new Dialogs.EditSimpleMetadata(mProject.Metadata);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    mProject.Touch();
                }
                Ready();
            }
        }

        /// <summary>
        /// Touch the project so that it seems that it was modified.
        /// </summary>
        private void touchProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.Touch();
        }

        /// <summary>
        /// Show or hide the NCX panel in the project panel.
        /// </summary>
        private void mShowhideTableOfContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mProjectPanel.TOCPanelVisible)
            {
                mProjectPanel.HideTOCPanel();
                toolStripStatusLabel1.Text = Localizer.Message("status_toc_hidden");
                FormUpdateShowHideTOC();
            }
            else
            {
                mProjectPanel.ShowTOCPanel();
                toolStripStatusLabel1.Text = Localizer.Message("status_toc_shown");
                FormUpdateShowHideTOC();
            }
        }

        /// <summary>
        /// Edit the user profile through the user profile dialog.
        /// </summary>
        private void userSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.UserProfile dialog = new Dialogs.UserProfile(mSettings.UserProfile);
            dialog.ShowDialog();
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Project tab. (JQ)
        /// </summary>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings);
            dialog.SelectProjectTab();
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Edit the preferences, starting from the Audio tab. (JQ)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void audioPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.Preferences dialog = new Dialogs.Preferences(mSettings);
            dialog.SelectAudioTab();
            if (dialog.ShowDialog() == DialogResult.OK) UpdateSettings(dialog);
            Ready();
        }

        /// <summary>
        /// Update the settings after the user has made some changes in the preferrences dialog. (JQ)
        /// </summary>
        private void UpdateSettings(Dialogs.Preferences dialog)
        {
            if (dialog.IdTemplate.Contains("#")) mSettings.IdTemplate = dialog.IdTemplate;
            if (Directory.Exists(dialog.DefaultDir)) mSettings.DefaultPath = dialog.DefaultDir;
            mSettings.LastOutputDevice = dialog.OutputDevice;
            Audio.AudioPlayer.Instance.SetDevice(this, dialog.OutputDeviceIndex);
            mSettings.LastInputDevice = dialog.InputDevice;
            Audio.AudioRecorder.Instance.SetInputDeviceForRecording(this, dialog.InputDeviceIndex);
            mSettings.AudioChannels = dialog.AudioChannels;
            mSettings.SampleRate = dialog.SampleRate;
            mSettings.BitDepth = dialog.BitDepth;
        }

        /// <summary>
        /// Save the settings when closing.
        /// TODO: check for closing project?
        /// </summary>
        private void ObiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                mSettings.SaveSettings();
            }
            catch (Exception x)
            {
                MessageBox.Show(String.Format(Localizer.Message("save_settings_error_text"), x.Message),
                    Localizer.Message("save_settings_error_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void mProject_StateChanged(object sender, Events.Project.StateChangedEventArgs e)
        {
            switch (e.Change)
            {
                case Obi.Events.Project.StateChange.Closed:
                    FormUpdateClosedProject();
                    break;
                case Obi.Events.Project.StateChange.Modified:
                    FormUpdateModifiedProject();
                    break;
                case Obi.Events.Project.StateChange.Opened:
                    mProjectPanel.Project = mProject;
                    FormUpdateOpenedProject();
                    mCmdMngr.Clear();
                    mProjectPanel.SynchronizeWithCoreTree(mProject.getPresentation().getRootNode());
                    break;
                case Obi.Events.Project.StateChange.Saved:
                    FormUpdateSavedProject();
                    break;
            }
        }

        private void mProject_CommandCreated(object sender, Events.Project.CommandCreatedEventArgs e)
        {
            mCmdMngr.Add(e.Command);
            FormUpdateUndoRedoLabels();
        }

        /// <summary>
        /// Setup the TOC and strip menus in the same way as the context menus for TOCPanel and StripManager.
        /// </summary>
        private void ObiForm_Load(object sender, EventArgs e)
        {
            // The TOC menu behaves like the context menu in the TOC view.
            mProjectPanel.TOCPanel.SelectedTreeNode += new Events.Node.SelectedHandler(TOCPanel_SelectedTreeNode);
            mAddSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mAddSectionToolStripMenuItem_Click);
            mAddSubSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mAddSubSectionToolStripMenuItem_Click);
            mDeleteSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mDeleteSectionToolStripMenuItem_Click);
            mRenameSectionToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mRenameToolStripMenuItem_Click);
            mMoveUpToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mMoveUpToolStripMenuItem_Click);
            mMoveDownToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mMoveDownToolStripMenuItem_Click);
            mMoveInToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.increaseLevelToolStripMenuItem_Click);
            mMoveOutToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.decreaseLevelToolStripMenuItem_Click);
            mShowInStripviewToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.TOCPanel.mShowInStripViewToolStripMenuItem_Click);

            // The strip menu behaves like the context menu in the strip view.
            mProjectPanel.StripManager.SelectedStrip += new Events.Strip.SelectedHandler(StripManagerPanel_SelectedStrip);
            mProjectPanel.StripManager.SelectedAudioBlock += new Events.Strip.SelectedHandler(StripManagerPanel_SelectedAudioBlock);
            mAddStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mAddStripToolStripMenuItem_Click);
            mRenameStripToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mRenameStripToolStripMenuItem_Click);
            mImportAudioFileToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mImportAssetToolStripMenuItem_Click);
            mPlayAudioBlockToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mPlayAudioBlockToolStripMenuItem_Click);
            mSplitAudioBlockToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mSplitAudioBlockToolStripMenuItem_Click);
            mRenameAudioBlockToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mRenameAudioBlockToolStripMenuItem_Click);
            mShowInTOCViewToolStripMenuItem.Click +=
                new EventHandler(mProjectPanel.StripManager.mShowInTOCViewToolStripMenuItem_Click);

            mProjectPanel.TOCPanel.VisibleChanged +=
                new EventHandler(delegate(object _sender, EventArgs _e) { FormUpdateShowHideTOC(); });
        }

        /// <summary>
        /// Update the TOC menu when a tree node is (de)selected.
        /// </summary>
        private void TOCPanel_SelectedTreeNode(object sender, Events.Node.SelectedEventArgs e)
        {
            mAddSubSectionToolStripMenuItem.Enabled = e.Selected;
            mDeleteSectionToolStripMenuItem.Enabled = e.Selected;
            mRenameSectionToolStripMenuItem.Enabled = e.Selected;
            mMoveSectionToolStripMenuItem.Enabled = e.CanMoveUp || e.CanMoveDown || e.CanMoveIn || e.CanMoveOut;
            mMoveUpToolStripMenuItem.Enabled = e.CanMoveUp;
            mMoveDownToolStripMenuItem.Enabled = e.CanMoveDown;
            mMoveInToolStripMenuItem.Enabled = e.CanMoveIn;
            mMoveOutToolStripMenuItem.Enabled = e.CanMoveOut;
            mShowInStripviewToolStripMenuItem.Enabled = e.Selected;
        }

        /// <summary>
        /// Update the strip menu when a strip is (de)selected.
        /// Affects "rename strip", "import audio file", "show in TOC view".
        /// </summary>
        private void StripManagerPanel_SelectedStrip(object sender, Events.Strip.SelectedEventArgs e)
        {
            mRenameStripToolStripMenuItem.Enabled = e.Selected;
            mImportAudioFileToolStripMenuItem.Enabled = e.Selected;
            mShowInTOCViewToolStripMenuItem.Enabled = e.Selected;
        }

        /// <summary>
        /// Update the strip menu when a block is (de)selected.
        /// Affects "play audio block", "split audio block", "rename audio block"
        /// </summary>
        private void StripManagerPanel_SelectedAudioBlock(object sender, Events.Strip.SelectedEventArgs e)
        {
            mPlayAudioBlockToolStripMenuItem.Enabled = e.Selected;
            mSplitAudioBlockToolStripMenuItem.Enabled = e.Selected;
            mRenameAudioBlockToolStripMenuItem.Enabled = e.Selected;
        }

        #endregion

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        /// <param name="path">The path of the XUK file to open.</param>
        private void OpenProject(string path)
        {
            if (ClosedProject())
            {
                ForceOpenProject(path);
            }
            else
            {
                Ready();
            }
        }

        /// <summary>
        /// Open a project without asking anything (using for reverting, for instance.)
        /// </summary>
        /// <param name="path">The path of the project to open.</param>
        private void ForceOpenProject(string path)
        {
            try
            {
                mProject = new Project();
                mProject.StateChanged += new Obi.Events.Project.StateChangedHandler(mProject_StateChanged);
                mProject.CommandCreated += new Obi.Events.Project.CommandCreatedHandler(mProject_CommandCreated);
                mProject.Open(path);
                AddRecentProject(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Localizer.Message("open_project_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                mProject = null;
            }
        }


        /// <summary>
        /// Initialize the application settings: get the settings from the saved user settings or the system
        /// and add the list of recent projects (at least those that actually exist) to the recent project menu.
        /// </summary>
        private void InitializeSettings()
        {
            mSettings = Settings.GetSettings();

            for (int i = mSettings.RecentProjects.Count - 1; i >= 0; --i)
            {
                if (!AddRecentProjectsItem((string)mSettings.RecentProjects[i]))
                {
                    mSettings.RecentProjects.RemoveAt(i);
                }
            }
            // Try to use the last input and output devices
            ArrayList devices = Audio.AudioPlayer.Instance.GetOutputDevices();
            if (devices.Count == 0)
            {
                MessageBox.Show(Localizer.Message("no_output_device_text"), Localizer.Message("no_output_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            Audio.AudioPlayer.Instance.SetDevice(this, mSettings.LastOutputDevice);
            devices = Audio.AudioRecorder.Instance.GetInputDevices();
            if (devices.Count == 0)
            {
                MessageBox.Show(Localizer.Message("no_input_device_text"), Localizer.Message("no_input_device_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            Audio.AudioRecorder.Instance.SetInputDeviceForRecording(this, mSettings.LastInputDevice);
        }

        /// <summary>
        /// Clear the list of recent projects.
        /// </summary>
        private void ClearRecentList()
        {
            while (mOpenRecentProjectToolStripMenuItem.DropDownItems.Count > 2)
            {
                mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(0);
            }
            mSettings.RecentProjects.Clear();
            mOpenRecentProjectToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Add a project to the list of recent projects.
        /// If the project was already in the list, promote it to the top of the list.
        /// Update the recent menu if necessary.
        /// </summary>
        /// <param name="path">The path of the project to add.</param>
        private void AddRecentProject(string path)
        {
            if (mSettings.RecentProjects.Count == 0)
            {
                mOpenRecentProjectToolStripMenuItem.Enabled = true;
            }
            else
            {
                if (mSettings.RecentProjects.Contains(path))
                {
                    int i = mSettings.RecentProjects.IndexOf(path);
                    mSettings.RecentProjects.RemoveAt(i);
                    mOpenRecentProjectToolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }
            if (AddRecentProjectsItem(path)) mSettings.RecentProjects.Insert(0, path);
        }

        /// <summary>
        /// Add an item in the recent projects list, if the file actually exists.
        /// </summary>
        /// <param name="path">The path of the item to add.</param>
        /// <returns>True if the file was added.</returns>
        private bool AddRecentProjectsItem(string path)
        {
            if (File.Exists(path))
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = Path.GetFileName(path);
                item.Click += new System.EventHandler(delegate(object sender, EventArgs e) { OpenProject(path); });
                mOpenRecentProjectToolStripMenuItem.DropDownItems.Insert(0, item);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when no project is opened.
        /// </summary>
        private void FormUpdateClosedProject()
        {
            this.Text = Localizer.Message("obi");
            mSaveProjectToolStripMenuItem.Enabled = false;                        // cannot save
            mSaveProjectasToolStripMenuItem.Enabled = false;                      // cannot save as
            mDiscardChangesToolStripMenuItem.Enabled = false;                     // cannot discard changes
            mCloseProjectToolStripMenuItem.Enabled = false;                       // cannot close
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)   // cannot do any edit
            {
                item.Enabled = false;
            }
            FormUpdateUndoRedoLabels();
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)    // cannot modify the TOC
            {
                item.Enabled = false;
            }
            foreach (ToolStripItem item in mStripsToolStripMenuItem.DropDownItems)  // cannot modify the strips
            {
                item.Enabled = false;
            }
            mProjectPanel.Project = null;
            if (mProject == null)
            {
                Ready();
            }
            else
            {
                toolStripStatusLabel1.Text = String.Format(Localizer.Message("closed_project"), mProject.Metadata.Title);
            }
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when a project is opened.
        /// </summary>
        private void FormUpdateOpenedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = false;
            mSaveProjectasToolStripMenuItem.Enabled = true;
            mDiscardChangesToolStripMenuItem.Enabled = false;
            mCloseProjectToolStripMenuItem.Enabled = true;
            foreach (ToolStripItem item in editToolStripMenuItem.DropDownItems)     // can do edits
            {
                item.Enabled = true;
            }
            FormUpdateUndoRedoLabels();
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)     // can modify the TOC
            {
                item.Enabled = true;
            }
            foreach (ToolStripItem item in mStripsToolStripMenuItem.DropDownItems)  // can modify the strips
            {
                item.Enabled = true;
            }
            FormUpdateShowHideTOC();
            toolStripStatusLabel1.Text = String.Format(Localizer.Message("opened_project"), mProject.XUKPath);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is saved.
        /// </summary>
        private void FormUpdateSavedProject()
        {
            this.Text = String.Format("{0} - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = false;
            mDiscardChangesToolStripMenuItem.Enabled = false;
            FormUpdateUndoRedoLabels();
            toolStripStatusLabel1.Text = String.Format(Localizer.Message("saved_project"), mProject.LastPath);
        }

        /// <summary>
        /// Update the form to reflect enabled/disabled actions when the project is modified.
        /// </summary>
        private void FormUpdateModifiedProject()
        {
            this.Text = String.Format("{0}* - {1}", mProject.Metadata.Title, Localizer.Message("obi"));
            mSaveProjectToolStripMenuItem.Enabled = true;
            mSaveProjectasToolStripMenuItem.Enabled = true;
            mDiscardChangesToolStripMenuItem.Enabled = true;
            mCloseProjectToolStripMenuItem.Enabled = true;
            Ready();
        }

        /// <summary>
        /// Change the label of th Show/Hide TOC menu item depending on the visibility of the NCX panel.
        /// When the TOC panel is hidden, the only enabled item in the TOC panel is "show TOC".
        /// When the TOC panel is shown, menu items (except "show TOC" and "add section") are enabled only if there is
        /// a currently selected node in the tree.
        /// </summary>
        private void FormUpdateShowHideTOC()
        {
            mShowhideTableOfCOntentsToolStripMenuItem.Text =
                Localizer.Message(mProjectPanel.TOCPanelVisible ? "hide_toc_label" : "show_toc_label");
            foreach (ToolStripItem item in mTocToolStripMenuItem.DropDownItems)
            {
                // this is incorrect--we need the same parameters as the selected event from the TOC tree.
                // need to save the last event somewhere maybe?
                item.Enabled = mProjectPanel.TOCPanelVisible && mProjectPanel.TOCPanel.Selected;
            }
            mShowhideTableOfCOntentsToolStripMenuItem.Enabled = true;
            mAddSectionToolStripMenuItem.Enabled = mProjectPanel.TOCPanelVisible;
        }

        private void FormUpdateUndoRedoLabels()
        {
            if (mCmdMngr.HasUndo)
            {
                mUndoToolStripMenuItem.Enabled = true;
                mUndoToolStripMenuItem.Text = String.Format(Localizer.Message("undo_label"), Localizer.Message("undo"),
                    mCmdMngr.UndoLabel);
            }
            else
            {
                mUndoToolStripMenuItem.Enabled = false;
                mUndoToolStripMenuItem.Text = Localizer.Message("undo");
            }
            if (mCmdMngr.HasRedo)
            {
                mRedoToolStripMenuItem.Enabled = true;
                mRedoToolStripMenuItem.Text = String.Format(Localizer.Message("redo_label"), Localizer.Message("redo"),
                    mCmdMngr.RedoLabel);
            }
            else
            {
                mRedoToolStripMenuItem.Enabled = false;
                mRedoToolStripMenuItem.Text = Localizer.Message("redo");
            }
        }

        /// <summary>
        /// Check whether a project is currently open and not saved; prompt the user about what to do.
        /// </summary>
        /// <returns>True if there is no open project or the currently open project could be closed.</returns>
        private bool ClosedProject()
        {
            if (mProject != null && mProject.Unsaved)
            {
                DialogResult result = MessageBox.Show(Localizer.Message("closed_project_text"),
                    Localizer.Message("closed_project_caption"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:
                        mProject.SaveAs(mProject.XUKPath);
                        mProject.Close();
                        return true;
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.No:
                        mProject.Close();
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                if (mProject != null) mProject.Close();
                return true;
            }
        }

        /// <summary>
        /// Update the status bar to say "Ready."
        /// </summary>
        private void Ready()
        {
            toolStripStatusLabel1.Text = Localizer.Message("ready");
        }

        /// <summary>
        /// Handle the undo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mUndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCmdMngr.HasUndo)
            {
                mCmdMngr.Undo();
                FormUpdateUndoRedoLabels();
            }
        }

        /// <summary>
        /// Handle the redo menu item.
        /// If there is something to undo, undo it and update the labels of undo and redo
        /// to synchronize them with the command manager.
        /// </summary>
        private void mRedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mCmdMngr.HasRedo)
            {
                mCmdMngr.Redo();
                FormUpdateUndoRedoLabels();
            }
        }

        /// <summary>
        /// Find the first phrase in the book and play it.
        /// For debugging purposes only!
        /// </summary>
        private void playFirstPhraseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visitors.PhraseEnumerator enumerator = new Visitors.PhraseEnumerator();
            mProject.RootNode.acceptDepthFirst(enumerator);
            if (enumerator.Phrases.Count > 0)
            {
                Dialogs.Play dialog = new Dialogs.Play(enumerator.Phrases[0]);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Find the first phrase in the book and split it.
        /// For debugging purposes only!
        /// </summary>
        private void splitFirstAudioBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visitors.PhraseEnumerator enumerator = new Visitors.PhraseEnumerator();
            mProject.RootNode.acceptDepthFirst(enumerator);
            if (enumerator.Phrases.Count > 0)
            {
                Dialogs.Split dialog = new Dialogs.Split(enumerator.Phrases[0], 0.0);
                dialog.ShowDialog();
            }
        }

        /// <summary>
        /// Quick test for recording dialog
        /// For debugging purposes only!
        /// </summary>
        private void startRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mProject.StartRecording(mSettings);
        }
    }
}