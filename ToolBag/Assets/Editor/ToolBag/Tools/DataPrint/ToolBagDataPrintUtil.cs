﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ToolBag.DataPrint
{
    public class ToolBagDataPrintUtil
    {
        private static List<object> objList = new List<object>();

        public static ToolBagPrintData PrintData(object obj)
        {
            objList.Clear();

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start(); // 性能监测开始

            if (obj == null)
            {
                return null;
            }

            ToolBagPrintData baseData = new ToolBagPrintData();

            RecursionPrintData(baseData, obj);

            stopwatch.Stop();

            Debug.Log("print data success time is " + stopwatch.Elapsed.TotalMilliseconds + "ms");

            objList.Clear();

            return baseData;
        }


        private static void RecursionPrintData(ToolBagPrintData parentData, object obj, string name = null)
        {
            if (obj != null)
            {
                int index = objList.FindIndex(x => x.Equals(obj));
                if (index != -1)
                {
                    ToolBagPrintData data = new ToolBagPrintData();
                    data.name = name == null ? obj.GetType().Name : name;
                    data.content = obj.ToString();
                    parentData.childDataList.Add(data);
                    return;
                }

                objList.Add(obj);
            }

            ToolBagPrintData childData = new ToolBagPrintData();

            childData.name = name == null ? obj.GetType().ToString() : name;

            if (obj is string str)
            {
                childData.content = str;
            }
            else if (obj.GetType().IsArray)
            {
                Array array = obj as Array;

                for (int i = 0; i < array.Length; i++)
                {
                    RecursionPrintData(childData, array.GetValue(i));
                }
            }
            else if (obj is IList iList)
            {
                for (int i = 0; i < iList.Count; i++)
                {
                    RecursionPrintData(childData, iList[i]);
                }
            }
            else if (obj.GetType().IsEnum)
            {
                childData.content = obj.ToString();
            }
            else if (obj is Vector3 || obj is Vector4 || obj is Vector2 || obj is Quaternion)
            {
                childData.content = obj.ToString();
            }
            else if (obj is IDictionary iDic)
            {
                ToolBagPrintData keyData = new ToolBagPrintData();
                keyData.name = "keys";

                foreach (object key in iDic.Keys)
                {
                    RecursionPrintData(keyData, key, key.GetType().ToString());
                }

                ToolBagPrintData valueData = new ToolBagPrintData();
                valueData.name = "value";

                foreach (object value in iDic.Values)
                {
                    RecursionPrintData(valueData, value, value.GetType().ToString());
                }

                childData.childDataList.Add(keyData);
                childData.childDataList.Add(valueData);
            }
            else
            {
                List<FieldInfo> infos = obj.GetType().GetRuntimeFields().ToList();
                infos.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));

                if (infos.Count == 0)
                {
                    childData.content = obj.ToString();
                }
                else
                {
                    for (int i = 0; i < infos.Count; i++)
                    {
                        RecursionPropertyInfo(infos[i], obj, childData);
                    }
                }

                MonoBehaviour[] components = null;
                if (obj is Transform childTran)
                {
                    components = childTran.GetComponents<MonoBehaviour>();
                }
                else if (obj is GameObject gameObject)
                {
                    components = gameObject.GetComponents<MonoBehaviour>();
                }

                if (components != null)
                {
                    for (int i = 0; i < components.Length; i++)
                    {
                        RecursionPrintData(childData, components[i], "Mono_" + components[i].GetType());
                    }
                }

                if (obj is MonoBehaviour)
                {
                    FieldInfo[] fileInfos = obj.GetType().GetFields();
                    for (int i = 0; i < fileInfos.Length; i++)
                    {
                        try
                        {
                            object o = fileInfos[i].GetValue(obj);

                            if (o == null || (o is GameObject gameObj && gameObj.GetInstanceID() == 0))
                            {
                                ToolBagPrintData data = new ToolBagPrintData();
                                data.name = fileInfos[i].Name;
                                data.content = "Null";
                                childData.childDataList.Add(data);
                            }
                            else
                            {
                                RecursionPrintData(childData, o, fileInfos[i].Name);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(fileInfos[i].Name + " Error");
                            throw;
                        }
                    }
                }
            }

            try
            {
                parentData.childDataList.Add(childData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void RecursionPropertyInfo(FieldInfo info, object obj, ToolBagPrintData childData)
        {
            if (info.FieldType.ToString() == "UnityEngine.Component")
            {
                return;
            }

            object childObj = null;

            try
            {
                childObj = info.GetValue(obj);
            }
            catch
            {
            }

            if (info.FieldType.BaseType?.FullName == "System.ValueType")
            {
                ToolBagPrintData data = new ToolBagPrintData();
                data.name = info.Name;
                data.content = childObj.ToString();
                childData.childDataList.Add(data);
            }
            else
            {
                if (childObj is GameObject gameObj)
                {
                    if (gameObj == null || gameObj.GetInstanceID() == 0)
                    {
                        ToolBagPrintData data = new ToolBagPrintData();
                        data.name = info.Name;
                        data.content = "Null";
                        childData.childDataList.Add(data);
                        return;
                    }
                }

                if (childObj != null)
                {
                    RecursionPrintData(childData, childObj, info.Name);
                }
                else
                {
                    ToolBagPrintData data = new ToolBagPrintData();
                    data.name = info.Name;
                    data.content = "Null";
                    childData.childDataList.Add(data);
                }
            }
        }
    }
}

public class ToolBagPrintData
{
    public string name;
    public string content;
    public List<ToolBagPrintData> childDataList;

    public ToolBagPrintData()
    {
        childDataList = new List<ToolBagPrintData>();
        content = "";
    }
}