using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Commands.TOC
{
   public abstract class MoveSectionNode : Command
    {
        protected SectionNode mNode;
        protected TreeNode mParent;
        protected int mIndex;

        public MoveSectionNode(SectionNode node, TreeNode parent)
        {
            mNode = node;
            mParent = parent;
            mIndex = node.Index;
        }

        /// <summary>
        /// Undo: restore the node
        /// </summary>
        public override void Undo()
        {
            mNode.Project.UndoMoveSectionNode(mNode, mParent, mIndex);
        }
    }
}
