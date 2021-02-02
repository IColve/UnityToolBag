using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagManager : MonoBehaviour
    {
        private const string SaveKey = "ToolBagKey";
        
        [MenuItem("Tools/ToolBagWindow")]
        public static void ShowToolWindow()
        {
            ToolBagShowWindow.ShowWindow();
        }

        public static List<ToolBagBaseData> GetToolBagDatas()
        {
            List<ToolBagBaseData> toolBagBaseDatas = new List<ToolBagBaseData>();
            
            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes().Where(item => item.IsSubclassOf(typeof(ToolBagBaseData))).ToList();

            Debug.Log($"Tools Count : {types.Count}");

            types.ForEach(x =>
            {
                toolBagBaseDatas.Add(Activator.CreateInstance(x) as ToolBagBaseData);
            });
            
            toolBagBaseDatas.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));
            
            LoadToolDataStatus().ForEach(x =>
            {
                ToolBagBaseData saveData = toolBagBaseDatas.Find(y => y.Name == x.name);
                if (saveData != null) saveData.IsShow = x.isShow;
            });

            return toolBagBaseDatas;
        }

        public static void SaveToolDataStatus(List<ToolSaveData> saveDataList)
        {
            List<ToolSaveData> oldDatas = LoadToolDataStatus();
            
            saveDataList.ForEach(x =>
            {
                ToolSaveData oldData = oldDatas.Find(y => x.name == y.name);

                if (oldData != null)
                {
                    oldData.SetValue(x);
                }
                else
                {
                    oldDatas.Add(x);
                }
            });

            SaveDataList(oldDatas);
        }

        public static void SaveToolDataStatus(ToolSaveData saveData)
        {
            List<ToolSaveData> oldDatas = LoadToolDataStatus();

            ToolSaveData oldData = oldDatas.Find(y => saveData.name == y.name);

            if (oldData != null)
            {
                oldData.SetValue(saveData);
            }
            else
            {
                oldDatas.Add(saveData);
            }
            
            SaveDataList(oldDatas);
        }

        private static void SaveDataList(List<ToolSaveData> saveDataList)
        {
            string str = "";

            for (int i = 0; i < saveDataList.Count; i++)
            {
                str += JsonUtility.ToJson(saveDataList[i]);

                if (i != saveDataList.Count - 1)
                {
                    str += "|";
                }
            }
            
            PlayerPrefs.SetString(SaveKey, str);

            if (ToolBagShowWindow.instance != null)
            {
                ToolBagShowWindow.instance.LoadData();
            }
        }

        public static List<ToolSaveData> LoadToolDataStatus()
        {
            List<ToolSaveData> saveDataList = new List<ToolSaveData>();
            
            string str = PlayerPrefs.GetString(SaveKey);
            string[] strArray = str.Split('|');

            for (int i = 0; i < strArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strArray[i]))
                {
                    continue;
                }
                
                ToolSaveData saveData = JsonUtility.FromJson<ToolSaveData>(strArray[i]);

                saveDataList.Add(saveData);
            }
            
            return saveDataList;
        }
    }

    public class ToolSaveData
    {
        public string name;
        public bool isShow;

        public ToolSaveData(string name, bool isShow)
        {
            this.name = name;
            this.isShow = isShow;
        }

        public void SetValue(ToolSaveData data)
        {
            name = data.name;
            isShow = data.isShow;
        }
    }
}