using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagTestAccount : ToolBagBaseData
    {
        public override string Name
        {
            get { return "账号管理"; }
        }

        public override void ShowGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("注册账号"))
            {
                ToolBagTestAccountUtil.RegistNewAccount();
            }

            GUILayout.Space(2);

            if (GUILayout.Button("读取账号"))
            {
                ToolBagTestAccountUtil.CopyLastLoginAccount();
            }

            GUILayout.EndVertical();
        }

        private Vector2 scrollVec2 = Vector2.zero;
        public override void ShowWindow()
        {
            ToolBagTestAccountMainData mainData = ToolBagTestAccountUtil.LoadAccountMainData();

            bool hasLoad = false;
            string oldName = mainData.accountName;

            GUILayout.BeginHorizontal();
            GUILayout.TextArea("用户名");
            mainData.accountName = GUILayout.TextField(mainData.accountName);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            GUILayout.TextArea("账号");
            GUILayout.TextArea("注册时间");
            GUILayout.TextArea("最后使用时间");
            GUILayout.TextArea("");
            GUILayout.EndHorizontal();

            scrollVec2 = GUILayout.BeginScrollView(scrollVec2);

            if (mainData.accountDataList != null)
            {
                mainData.accountDataList.Sort((a, b) => -a.lastLoginTime.CompareTo(b.lastLoginTime));

                for (int i = 0; i < mainData.accountDataList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.TextArea(ToolBagTestAccountUtil.GetAccountStrByData(mainData.accountName, mainData.accountDataList[i]));
                    GUILayout.TextArea(ToolBagTestAccountUtil.GetSystemTimeFromStamp(mainData.accountDataList[i].registTime).ToString());
                    GUILayout.TextArea(ToolBagTestAccountUtil.GetSystemTimeFromStamp(mainData.accountDataList[i].lastLoginTime).ToString());
                    if (GUILayout.Button("使用账号"))
                    {
                        mainData.accountDataList[i].lastLoginTime = ToolBagTestAccountUtil.GetCurrentTimeStamp();
                        hasLoad = true;
                        ToolBagTestAccountUtil.CopyAccount(mainData.accountName, mainData.accountDataList[i]);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.Space(1);
                }
            }

            GUILayout.EndScrollView();

            if (oldName != mainData.accountName)
            {
                ToolBagTestAccountUtil.SaveAccountMainData(mainData);
            }
        }
    }
}