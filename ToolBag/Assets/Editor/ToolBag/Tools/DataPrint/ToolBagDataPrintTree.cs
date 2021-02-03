using UnityEditor.IMGUI.Controls;

namespace ToolBag.DataPrint
{
    public class ToolBagDataPrintTree : TreeView
    {
        public TreeViewItem root;

        public ToolBagDataPrintTree(TreeViewState state) : base(state)
        {
            root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        }

        protected override TreeViewItem BuildRoot()
        {
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
    }

}

