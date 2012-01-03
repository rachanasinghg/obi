using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssignSpecialNodeToChunk : Form
    {
        private ObiRootNode m_ObiNode;
        private EmptyNode m_SelectedNode = null;
        private EmptyNode m_EndNode = null;
        private bool m_AssignRole = false;
        private bool m_IsYesToAll = false;

        public EmptyNode EndNode
        { get { return m_EndNode; } }

        public AssignSpecialNodeToChunk()
        {
           // m_ObiNode = obiNode;
           // m_SelectedNode = selectedNode;
            InitializeComponent();
        }

        public bool Is_AssignRole
        { get { return m_AssignRole; } }

        public bool Is_YesToAll
        { get { return m_IsYesToAll; } }

        private void m_btn_YesToAll_Click(object sender, EventArgs e)
        {
            m_IsYesToAll = true;
            Close();
        }

        private void m_btn_Yes_Click(object sender, EventArgs e)
        {
            m_AssignRole = true;
        }

        private void m_btn_No_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_btn_NoToAll_Click(object sender, EventArgs e)
        {

        }
    }
}