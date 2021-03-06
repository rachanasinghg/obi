using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Toolbar_EditAudio : UserControl
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private EmptyNode m_Node;
        private ProjectView m_ProjectView;
        private bool Dragging = false;
        private Point DragStart = Point.Empty;
        private KeyboardShortcuts_Settings keyboardShortcuts;
        private bool IsSelection = false;
        private Size m_InitialtoolStripSize;
      



         
        public Toolbar_EditAudio()
        {
            InitializeComponent();
            m_InitialtoolStripSize = this.toolStrip1.Size;
        }
        public Toolbar_EditAudio(ContentView contentView, Strip strip, EmptyNode node, ProjectView mProjectView)
            : this()
        {
            m_ContentView = contentView;
            m_Strip = strip;
            m_Node = node;
            m_ProjectView = mProjectView;          
            m_ProjectView.SelectionChanged+= new EventHandler(ProjectViewSelectionChanged);
         
            this.toolStrip1.MouseDown+= new MouseEventHandler(Toolbar_EditAudio_MouseDown);
            this.toolStrip1.MouseUp+= new MouseEventHandler(Toolbar_EditAudio_MouseUp);
            this.toolStrip1.MouseMove += new MouseEventHandler(Toolbar_EditAudio_MouseMove);

            this.toolStrip1.MinimumSize = this.Size;
            this.toolStrip1.MaximumSize = this.Size;
            this.toolStrip1.Size = this.Size;

            EditAudioPanelToolTipInit();

            if (m_ProjectView.ObiForm.Settings.ObiFont != this.Font.Name) //@fontconfig
            {
                SetFont(); //@fontconfig
            }
            
        }


        private void ProjectViewSelectionChanged(object sender, EventArgs e)
        {
            EnableDisableCut();
        }


        public void EnableDisableCut()
        {
            mbtnCuttoolStrip.Enabled = (m_ContentView.CanRemoveAudio ) && !m_ProjectView.TransportBar.IsRecorderActive && !m_ProjectView.ObiForm.Settings.Project_ReadOnlyMode;
            mbtnCopytoolStrip.Enabled = (m_ContentView.CanCopyAudio || m_ContentView.CanCopyBlock || m_ContentView.CanCopyStrip) && !m_ProjectView.TransportBar.IsRecorderActive && !m_ProjectView.ObiForm.Settings.Project_ReadOnlyMode;
            mbtnPastetoolStrip.Enabled = m_ProjectView.CanPaste && !m_ProjectView.ObiForm.Settings.Project_ReadOnlyMode;
            mbtnSplittoolStrip.Enabled =  m_ProjectView.CanSplitPhrase;
            mbtnDeletetoolStrip.Enabled = (m_ContentView.CanRemoveAudio) && !m_ProjectView.TransportBar.IsRecorderActive && !m_ProjectView.ObiForm.Settings.Project_ReadOnlyMode;
            mbtnMergetoolStrip.Enabled = m_ContentView.CanMergeBlockWithNext;
            mbtnAudioProcessingToolStripDropDown.Enabled = m_ProjectView.CanExportSelectedNodeAudio;
            
        }



        public void SetEditPanelFontSize(Size thisSize)
        {
            if (m_ContentView.ZoomFactor > 1.1 && m_ContentView.ZoomFactor < 4)
            {
                float tempZoomfactor;
                if (m_ContentView.ZoomFactor > 1.5)
                {
                    tempZoomfactor = 1.46f;
                }
                else
                {
                    tempZoomfactor = m_ContentView.ZoomFactor;
                }
                this.toolStrip1.MinimumSize = thisSize;
                this.toolStrip1.Font = new Font(this.toolStrip1.Font.Name, (this.toolStrip1.Font.Size + (float)3.0), FontStyle.Bold);
            }
            else
            {
                this.toolStrip1.MinimumSize = m_InitialtoolStripSize;
                this.toolStrip1.Size = m_InitialtoolStripSize;
                this.toolStrip1.Font = new Font(this.toolStrip1.Font.Name, (this.toolStrip1.Font.Size - (float)3.0), FontStyle.Regular);
            }
        }
        public void EditAudioPanelToolTipInit()
        {
            keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mCutToolStripMenuItem"))
            {
                this.mbtnCuttoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Cut") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCutToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnCuttoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Cut") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCutToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mCopyToolStripMenuItem"))
            {
                this.mbtnCopytoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Copy") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCopyToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnCopytoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Copy") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCopyToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mDeleteToolStripMenuItem"))
            {
                this.mbtnDeletetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Delete") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mDeleteToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnDeletetoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Delete") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mDeleteToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPasteToolStripMenuItem"))
            {
                this.mbtnPastetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Paste") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPasteToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnPastetoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Paste") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPasteToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mMergePhraseWithNextToolStripMenuItem"))
            {
                this.mbtnMergetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Merge") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mMergePhraseWithNextToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnMergetoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Merge") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mMergePhraseWithNextToolStripMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mPhrases_ApplyPhraseDetectionMenuItem"))
            {
                this.mbtnPraseDetectiontoolStrip.ToolTipText = Localizer.Message("EditAudioTT_PhraseDetect") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPhrases_ApplyPhraseDetectionMenuItem"].Value.ToString()) + ")";
                this.mbtnPraseDetectiontoolStrip.AccessibleName = Localizer.Message("EditAudioTT_PhraseDetect") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPhrases_ApplyPhraseDetectionMenuItem"].Value.ToString());
            }
            if (keyboardShortcuts.MenuNameDictionary.ContainsKey("mSplitPhraseToolStripMenuItem"))
            {
                this.mbtnSplittoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Split") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mSplitPhraseToolStripMenuItem"].Value.ToString()) + ")";
                this.mbtnSplittoolStrip.AccessibleName = Localizer.Message("EditAudioTT_Split") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mSplitPhraseToolStripMenuItem"].Value.ToString());
            }  

        }

        private void mbtnCuttoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Cut();
        }

        private void mbtnCopytoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Copy();
        }

        private void mbtnPastetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Paste();

        }

        private void mbtnSplittoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.SplitPhrase();
        }

        private void mbtnDeletetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Delete();

        }

        private void mbtnMergetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.MergeBlockWithNext();
        }

        private void mbtnPraseDetectiontoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.ApplyPhraseDetection();
        }

        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void toolStrip1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Toolbar_EditAudio_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragStart = new Point(e.X, e.Y);
            toolStrip1.Capture = true;
        }

        private void Toolbar_EditAudio_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
               
                    this.Left = Math.Max(0, e.X + this.Left - DragStart.X);
               
                    this.Top = Math.Max(0, e.Y + this.Top - DragStart.Y);
            }
         
        }

        private void Toolbar_EditAudio_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
            toolStrip1.Capture = false;
        }

        private void toolStrip1_MouseHover(object sender, EventArgs e)
        {
            
        }
        private void SetFont() //@fontconfig
        {
            this.Font = new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
            toolStrip1.Font = new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
        }

        private void m_ChangeVolumeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ProjectView.TransportBar.IsPlayerActive)
            {
                if (m_ProjectView.TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) m_ProjectView.TransportBar.Pause();
                m_ProjectView.TransportBar.Stop();
            }
            m_ProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.Amplify);
        }

        private void m_FadeInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.FadeIn);
        }

        private void m_FadeOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.FadeOut);
        }

        private void m_NormalizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_ProjectView.AudioProcessing(AudioLib.WavAudioProcessing.AudioProcessingKind.Normalize);
        }
    }
}
