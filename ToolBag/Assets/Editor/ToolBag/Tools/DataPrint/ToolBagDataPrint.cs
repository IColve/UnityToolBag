using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;
using System;

namespace ToolBag.DataPrint
{
    public class ToolBagDataPrint : ToolBagBaseData
    {
        public override string Name
        {
            get
            {
                return "数据展示";
            }
        }

        private static ToolBagDataPrintTree treeView;
        private static TreeViewState treeViewState;
        private static int lastShowType = -1;
        private static bool needExpand;
        private static bool useOldExpand = true;

        private static string searchStr;

        private static int id = 0;

        public static int ID
        {
            get
            {
                id += 1;
                return id;
            }
        }

        public override void ShowWindow()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(3);

            useOldExpand = GUILayout.Toggle(useOldExpand, "是否记录展开状态");

            GUILayout.Space(3);

            searchStr = GUILayout.TextArea(searchStr);

            if (GUILayout.Button("查找"))
            {
                SearchValue();
            }

            GUILayout.Space(5);

            GUILayout.Space(1);

            if (GUILayout.Button("复制选中信息"))
            {
                CopySelectContent();
            }

            GUILayout.Space(1);

            if (GUILayout.Button("选中物体信息"))
            {
                if (Selection.activeGameObject != null)
                {
                    SetData(ToolBagDataPrintUtil.PrintData(Selection.activeGameObject));
                }
            }

            GUILayout.Space(1);

            if (treeView != null)
            {
                treeView.OnGUI(new Rect(0, 160, ToolBagDetailWindow.instance.position.width, ToolBagDetailWindow.instance.position.height - 160));
            }
            
            GUILayout.EndVertical();
        }

        public static void SetData(ToolBagPrintData data)
        {
            if (treeViewState == null)
            {
                treeViewState = new TreeViewState();
            }

            if (data == null)
            {
                return;
            }

            IList<int> expandIdList = null;
            if (treeView != null)
            {
                expandIdList = treeView.GetExpanded();
            }

            treeView = new ToolBagDataPrintTree(treeViewState);
            id = 0;
            treeView.root.AddChild(GetTreeViewItem(data));
            treeView.Reload();
            if (expandIdList != null)
            {
                if (needExpand && useOldExpand)
                {
                    for (int i = 0; i < expandIdList.Count; i++)
                    {
                        treeView.SetExpanded(expandIdList[i], true);
                    }
                }
                else
                {
                    for (int i = 0; i < expandIdList.Count; i++)
                    {
                        treeView.SetExpanded(expandIdList[i], false);
                    }
                }
            }
        }

        private static TreeViewItem GetTreeViewItem(ToolBagPrintData baseData)
        {
            TreeViewItem item = new TreeViewItem(0, -1, "Root");
            List<TreeViewItem> itemList = GetChildItems(1, baseData.childDataList);

            if (itemList != null)
            {
                itemList.ForEach(x => item.AddChild(x));
            }

            return item;
        }

        private static List<TreeViewItem> GetChildItems(int index, List<ToolBagPrintData> printDatas)
        {
            if (printDatas == null)
            {
                return new List<TreeViewItem>();
            }

            List<TreeViewItem> viewItems = new List<TreeViewItem>();
            printDatas.ForEach(x =>
            {
                string title = "";
                if (x.childDataList == null || x.childDataList.Count == 0)
                {
                    title = x.name + " | " + x.content;
                }
                else
                {
                    title = x.name;
                }

                TreeViewItem item = new TreeViewItem(ID, 0, title);
                viewItems.Add(item);
                if (x.childDataList != null && x.childDataList.Count > 0)
                {
                    List<TreeViewItem> childItems = GetChildItems(0, x.childDataList);
                    childItems.ForEach(y => item.AddChild(y));
                }
            });
            return viewItems;
        }

        private void CopySelectContent()
        {
            if (treeView?.root == null)
            {
                return;
            }

            List<int> selectList = treeView.GetSelection().ToList();

            if (selectList.Count > 0)
            {
                int selectIndex = selectList[0];
                RecursionFindSelect(treeView.root, selectIndex);
            }
        }

        private bool RecursionFindSelect(TreeViewItem treeViewItem, int selectIndex)
        {
            if (treeViewItem == null)
            {
                return false;
            }

            if (treeViewItem.id == selectIndex)
            {
                TextEditor t = new TextEditor();
                t.text = treeViewItem.displayName;
                t.OnFocus();
                t.Copy();
                return true;
            }

            if (treeViewItem.children != null)
            {
                for (int i = 0; i < treeViewItem.children.Count; i++)
                {
                    if (RecursionFindSelect(treeViewItem.children[i], selectIndex))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void SearchValue()
        {
            if (string.IsNullOrEmpty(searchStr))
            {
                return;
            }

            if (treeView == null || treeView.root == null)
            {
                return;
            }

            List<int> idList = new List<int>();
            List<int> expandIdList = new List<int>();
            string lowerSearchStr = searchStr.ToLower();

            Action<TreeViewItem, TreeViewItem> action = null;

            Action<TreeViewItem> expandAction = null;

            expandAction = item =>
            {
                if (item == null)
                {
                    return;
                }

                if (!expandIdList.Contains(item.id))
                {
                    expandIdList.Add(item.id);
                }

                expandAction(item.parent);
            };

            action = (item, parent) =>
            {
                if (item.displayName.ToLower().Contains(lowerSearchStr))
                {
                    idList.Add(item.id);

                    expandAction(parent);
                }

                if (item.children != null)
                {
                    item.children.ForEach(x => action(x, item));
                }
            };

            treeView.root.children.ForEach(x => action(x, treeView.root));

            expandIdList.ForEach(x => treeView.SetExpanded(x, true));

            treeView.SetSelection(idList);
        }
    }
}