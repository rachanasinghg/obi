using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SelectMergeSectionRange : Form
    {
        List<SectionNode> m_SectionList = null;
        List<SectionNode> m_SelectedSectionList = null;
        private int m_selectedIndex;

        public SelectMergeSectionRange()
        {
            InitializeComponent();
        }

        public SelectMergeSectionRange(List<SectionNode> sectionsList, int selectedIndexOfSection) : this()
        {
            m_SectionList = sectionsList;
            m_selectedIndex = selectedIndexOfSection;
            populateListboxForSectionsToMerge();            
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            for (int i = m_lb_listofSectionsToMerge.SelectedIndex; i < m_lb_listofSectionsToMerge.SelectedItems.Count; i++)
            {
                if(m_SectionList[m_selectedIndex].PhraseChildCount < 7000)
                   m_SelectedSectionList.Add((SectionNode)m_lb_listofSectionsToMerge.Items[i]);             
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void populateListboxForSectionsToMerge()
        {
            for (int i = 0; i <= (m_SectionList.Count - 1); i++)
            {
                if (m_SectionList[i].Level >= m_SectionList[m_selectedIndex].Level)
                    m_lb_listofSectionsToMerge.Items.Add(m_SectionList[i]);
                else
                    return;
            }
        }
    }
}