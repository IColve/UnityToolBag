using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagDataEditor : ToolBagBaseData
    {
        private List<ToolBagBaseData> toolBagDataList;
        
        public override string Name
        {
            get
            {
                return "编辑面板";
            }
        }

        private int index = 0;

        public override void ShowGUI()
        {
            if (toolBagDataList == null)
            {
                toolBagDataList = ToolBagManager.GetToolBagDatas().FindAll(x => x.Name != Name);
            }

            for (int i = 0; i < toolBagDataList.Count; i++)
            {
                toolBagDataList[i].IsShow = GUILayout.Toggle(toolBagDataList[i].IsShow, toolBagDataList[i].Name);
                GUILayout.Space(1);
            }

            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("取消"))
            {
                ToolBagManager.SaveToolDataStatus(new ToolSaveData(Name, false));
            }
            
            if (GUILayout.Button("保存"))
            {
                OnSaveBtnClick();
            }
            
            GUILayout.EndHorizontal();
        }

        public void OnSaveBtnClick()
        {
            List<ToolSaveData> saveDataList = new List<ToolSaveData>();
            
            if (toolBagDataList != null)
            {
                toolBagDataList.ForEach(x =>
                {
                    saveDataList.Add(new ToolSaveData(x.Name, x.IsShow));
                });
            }
            
            saveDataList.Add(new ToolSaveData(Name, false));
            
            ToolBagManager.SaveToolDataStatus(saveDataList);
        }
    }
}