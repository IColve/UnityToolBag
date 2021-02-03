using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagDetailWindow : EditorWindow
    {
        private List<ToolBagBaseData> toolBagDataList;
        private ToolBagBaseData selectData;
        
        public static void ShowWindow()
        {
            GetWindow(typeof(ToolBagDetailWindow), false, "ToolBagDetailWindow");
        }

        private void OnGUI()
        {
            if (toolBagDataList == null)
            {
                LoadData();
            }

            GUILayout.BeginVertical();
            
            GUILayout.Space(2);
            
            if (selectData == null)
            {
                toolBagDataList.ForEach(x =>
                {
                    if (GUILayout.Button(x.Name))
                    {
                        selectData = x;
                    }
                    
                    GUILayout.Space(2);
                });
            }
            else
            {
                if (GUILayout.Button("返回"))
                {
                    selectData = null;
                }
                
                GUILayout.Space(5);
 
                if (selectData != null)
                {
                    GUILayout.TextArea(selectData.Name, new GUIStyle("DefaultCenteredText"){fontStyle = FontStyle.Bold});
                    
                    GUILayout.Space(2);
                    
                    selectData?.ShowWindow();
                }
            }
            
            GUILayout.EndVertical();
        }

        public void LoadData()
        {
            toolBagDataList = ToolBagManager.GetToolBagDatas().FindAll(x => x.HasWindow);
        }
    }
}