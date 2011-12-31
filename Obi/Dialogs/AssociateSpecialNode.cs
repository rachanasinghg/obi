using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssociateSpecialNode : Form
    {
        private ObiRootNode m_ObiNode;
        private bool m_IsShowAll = false;
        List<EmptyNode> listOfFirstNodeOfSpecialNodes = new List<EmptyNode>();
        List<EmptyNode> listOfAnchorNodes = new List<EmptyNode>();
        private EmptyNode m_SelectedNode = null;
        Dictionary<EmptyNode, EmptyNode> nodes_phraseMap = new Dictionary<EmptyNode, EmptyNode>(); // used for importing sections
        
        public AssociateSpecialNode(ObiRootNode obiNode, EmptyNode selectedNode)
        {
            m_ObiNode = obiNode;
            m_SelectedNode = selectedNode;
            InitializeComponent();
            m_txtBox_SectionName.Text = selectedNode.ToString() + " = ";
            AddToListBox();           
        }

        public Dictionary<EmptyNode,EmptyNode> DictionaryToMapValues
        { get { return nodes_phraseMap; } }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            if (m_btn_ShowAll.Text == "Show all")
            {
                m_IsShowAll = true;
                m_lb_listOfAllAnchorNodes.Visible = true;
                AddToListBox();
                m_txtBox_SectionName.Visible = false;
                m_btn_ShowAll.Text = "Show selected";
            }
            else if (m_btn_ShowAll.Text == "Show selected")
            {
                m_txtBox_SectionName.Visible = true;
                m_lb_listOfAllAnchorNodes.Visible = false;
                m_btn_ShowAll.Text = "Show all";
            }
        }

        public void AddToListBox()
        {
            List<SectionNode> listOfAllSections = new List<SectionNode>();
            listOfAllSections = m_ObiNode.GetListOfAllSections();
            string tempString = "";
            int firstIndex = -1;
            foreach (SectionNode node in listOfAllSections)
            {
                for (int i = 0; i < node.PhraseChildCount; i++)
                {
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom)
                    {
                        tempString = node.PhraseChild(i).CustomRole;
                        if ( i < node.PhraseChildCount - 1 && tempString != node.PhraseChild(i + 1).CustomRole )
                        {
                            if (!m_IsShowAll)
                            {
                                if (firstIndex == -1)
                                {
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (i + 1));
                                    listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                                }
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));                               
                            }
                           
                            firstIndex = -1;
                        }
                        else
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                            }
                            if (i == node.PhraseChildCount - 1)
                            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                                if(!m_IsShowAll)
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));
                                firstIndex = -1;
                            }
                        }
                    }
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Anchor && m_IsShowAll)
                    {
        //                m_lb_listOfAllAnchorNodes.Items.Add(node.PhraseChild(i));
                        m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i) + " = " + node.PhraseChild(i).AssociatedNode);
                        listOfAnchorNodes.Add(node.PhraseChild(i));
                    }                    
                }
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
            EmptyNode anchorNode = null;
            if (m_IsShowAll && listOfAnchorNodes.Count > 0)
            {
                anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];

            }
            else
            {
                anchorNode = m_SelectedNode;

            }
            if (nodes_phraseMap.ContainsKey(anchorNode))
            {
                nodes_phraseMap[anchorNode] = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
            }
            else
            {
                nodes_phraseMap.Add(anchorNode, listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]);
            }
            
        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
            EmptyNode anchorNode = null;
            if (m_IsShowAll && listOfAnchorNodes.Count > 0)
            {
                anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];
                
            }
            else
            {
                anchorNode = m_SelectedNode;
                
            }
            if (nodes_phraseMap.ContainsKey(anchorNode))
            {
                nodes_phraseMap[anchorNode] = null;
            }
            else
            {
                nodes_phraseMap.Add(anchorNode, null);
            }

            
            m_lb_listOfAllAnchorNodes.Items.Insert(m_lb_listOfAllAnchorNodes.SelectedIndex,"Section " + listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].ParentAs<SectionNode>().Label + " "+ listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex] + " = ");
            m_lb_listOfAllAnchorNodes.Items.Remove(m_lb_listOfAllAnchorNodes.SelectedItem);
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.Items.Count > 0 && m_lb_ListOfSpecialNodes.Items.Count >0;
            if (m_lb_listOfAllAnchorNodes.Visible)
                m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
            else
                m_btn_Associate.Enabled = m_lb_ListOfSpecialNodes.SelectedItem != null;
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_btn_Associate.Enabled = (m_lb_listOfAllAnchorNodes.Items.Count > 0 && m_lb_ListOfSpecialNodes.Items.Count > 0);
            m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
            m_btn_Deassociate.Enabled = true;        
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {   
        }
    }
}


