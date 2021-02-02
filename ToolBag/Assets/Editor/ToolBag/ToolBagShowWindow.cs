using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagShowWindow : EditorWindow
    {
        public static ToolBagShowWindow instance;
        
        private static List<ToolBagBaseData> toolBagDataList;

        private Vector2 scrollValue;
        
        public static void ShowWindow()
        {
            if (instance != null)
            {
                return;
            }
            
            GetWindow(typeof(ToolBagShowWindow), false, "ToolBagWindow");
        }

        private void OnGUI()
        {
            if (instance == null)
            {
                instance = this;
            }
            
            if (toolBagDataList == null)
            {
                LoadData();
            }

            if (toolBagDataList.Count == 0)
            {
                return;
            }

            scrollValue = GUILayout.BeginScrollView(scrollValue);
            GUILayout.BeginVertical();

            GUILayout.Space(2);

            bool isShowList = toolBagDataList[0].IsShow;
            
            if (!isShowList)
            {
                if (GUILayout.Button("窗口面板"))
                {
                    ToolBagDetailWindow.ShowWindow();
                }
                
                GUILayout.Space(2);
                
                if (GUILayout.Button("打开编辑"))
                {
                    ToolBagManager.SaveToolDataStatus(new ToolSaveData("编辑面板", true));
                }
            }
            
            GUILayout.Space(10);

            for (int i = 0; i < toolBagDataList.Count; i++)
            {
                if (!toolBagDataList[i].IsShow || (isShowList && i != 0))
                {
                    continue;
                }
                
                GUILayout.TextArea(toolBagDataList[i].Name, new GUIStyle("DefaultCenteredText"){fontStyle = FontStyle.Bold});

                toolBagDataList[i].ShowGUI();
                GUILayout.Space(2);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        public void LoadData()
        {
            toolBagDataList = ToolBagManager.GetToolBagDatas();
        }
        
        private void OnDestroy()
        {
            instance = null;
        }
    }
}


