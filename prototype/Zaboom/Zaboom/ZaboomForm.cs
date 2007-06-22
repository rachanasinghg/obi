using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class ZaboomForm : Form
    {   
        public ZaboomForm()
        {
            InitializeComponent();
            Text = "Zaboom";
            Status = "Ready.";
            projectPanel.Player.StateChanged += new Audio.StateChangedHandler(Player_StateChanged);
            projectPanel.Player.EndOfAudioAsset += new Audio.EndOfAudioAssetHandler(Player_EndOfAudioAsset);
        }

        void Player_StateChanged(object sender, Audio.StateChangedEventArgs e)
        {
            Status = projectPanel.Player.State.ToString();
            System.Diagnostics.Debug.Print(e.PreviousState.ToString() + " => " + projectPanel.Player.State.ToString());
        }

        void Player_EndOfAudioAsset(object sender, EventArgs e)
        {
            // projectPanel.Player.Stop();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProjectDialog dialog = new NewProjectDialog(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "project.zab",
                "Zaboom project files|*.zab",
                "Untitled Zaboom Project");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                projectPanel.Project = new Project(dialog.Title, dialog.ProjectLocation);
                projectPanel.Project.StateChanged += new StateChangedHandler(Project_StateChanged);
                projectPanel.Project.Save();
            }
        }

        void Project_StateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.Change == StateChange.Closed)
            {
                Text = "Zaboom";
            }
            else if (e.Change == StateChange.Modified || e.Change == StateChange.Opened || e.Change == StateChange.Saved)
            {
                Text = "Zaboom - " + projectPanel.Project.TitleSaved;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                projectPanel.Project.Save();
            }
        }

        private void importFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ("WAV files|*.wav");
                dialog.Multiselect = false;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        projectPanel.Project.ImportAudioFile(dialog.FileName);
                    }
                    catch (Exception e_)
                    {
                        MessageBox.Show(
                            String.Format("Could not open WAV file {0}: {1}", dialog.FileName, e_.Message),
                            "Error opening WAV file",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void viewSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (projectPanel.Project != null)
            {
                SourceView dialog = new SourceView(projectPanel.Project);
                dialog.Show();
            }
        }

        private string Status { set { statusLabel.Text = value; } }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            projectPanel.Player.Stop();
        }
    }
}