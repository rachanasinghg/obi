﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace Obi.Dialogs
{
    public partial class AutoPageGeneration : Form
    {
        private ProjectView.ProjectView m_ProjectView;
        private int m_StartingSectionIndex = 0;
        private List<SectionNode> m_sectionsList;
        private int m_GapsInPages = 0;
        private bool m_CanAddPage = false;
        private bool m_DeletePages = true;
        private DialogResult m_DeleteExistingPages = DialogResult.No;
        public AutoPageGeneration(ProjectView.ProjectView ProjectView)
        {
            InitializeComponent();

            m_ProjectView = ProjectView;
            
            m_sectionsList = ((ObiRootNode)m_ProjectView.Presentation.RootNode).GetListOfAllSections();
         
            for (int i = 1; i <= m_sectionsList.Count; i++)
            {
                m_cbStartingSectionIndex.Items.Add(i);
            }
            if (m_cbStartingSectionIndex.Items.Count > 0)
            {
                m_cbStartingSectionIndex.SelectedIndex = 0;
            }
            
            
        }

        public int GapsInPages
        {
            get
            {
                return m_GapsInPages;
            }
        }

        public int StartingSectionIndex
        {
            get
            {
                return m_StartingSectionIndex;
            }
        }
        public bool GenerateSpeech
        {
            get
            {
                return m_rbGenerateTTS.Checked;
            }
        }
        public bool CanAddPage
        {
            get
            {
                return m_CanAddPage;
            }
        }
        public bool DeletePages
        {
            get
            {
                return (m_DeleteExistingPages == DialogResult.Yes);
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {           
            m_CanAddPage = AddPage();
           
            if (!m_CanAddPage && m_DeleteExistingPages == DialogResult.No)
            {
                if (!m_DeletePages)
                {
                    this.Close();
                    m_DeletePages = true; 
                }
                else
                {
                    this.DialogResult = DialogResult.None;
                }
                            
            }  
        }


        private bool AddPage()
        {
            int tempGapsInPages;
            try
            {
                tempGapsInPages = Convert.ToInt32(m_txtGapsInPages.Text);
                if (tempGapsInPages <= 0)
                {
                    MessageBox.Show(Localizer.Message("Mints_invalid_input"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_txtGapsInPages.Focus();
                    return false;
                }
            }
            catch (System.FormatException)
            {
                MessageBox.Show(Localizer.Message("Mints_invalid_input"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_txtGapsInPages.Focus();
                return false;
            }
            m_GapsInPages = (tempGapsInPages * 60 * 1000); // time is converted into ms

            try
            {
                m_StartingSectionIndex = Convert.ToInt32(m_cbStartingSectionIndex.Text); // Starting section index is assigned from combo box
                m_StartingSectionIndex = m_StartingSectionIndex - 1;
            }
            catch (System.FormatException)
            {
                MessageBox.Show(Localizer.Message("GotoPageOrPhrase_EnterNumeric"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                m_cbStartingSectionIndex.Focus();
                return false;
            }
            if (((ObiRootNode)m_ProjectView.Presentation.RootNode).PageCount > 0)
            {
                m_DeleteExistingPages = MessageBox.Show(Localizer.Message("PagesInSectionsDetected"), Localizer.Message("Caption_Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (m_DeleteExistingPages == DialogResult.Yes)
                {
                    m_DeletePages = true;
                }
                else if (m_DeleteExistingPages == DialogResult.No)
                {
                    m_DeletePages = false;
                }
                m_cbStartingSectionIndex.Focus();
                return false;
            }

            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {

                m_ProjectView.Selection.Node = m_sectionsList[m_StartingSectionIndex]; // Starting Index assigned 

            }


            return true;
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}