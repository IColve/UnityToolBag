using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolBagActionWindow : EditorWindow
{
    public static ToolBagActionWindow instance;
    
    public static void ShowWindow()
    {
        if (instance != null)
        {
            return;
        }
            
        GetWindow(typeof(ToolBagActionWindow), false, "ToolBagDetailWindow");
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
    
        DrawBtn("打印全部代码行数", PrintAllCodeCount);
        DrawBtn("打印忽略空行的代码行数", PrintIgnoreCodeCount);
        
        GUILayout.EndVertical();
    }
    
    private void DrawBtn(string btnName, Action action)
    {
        if (GUILayout.Button(btnName))
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// 打印全部代码行数
    /// </summary>
    private void PrintAllCodeCount()
    {
        ToolBagUtil.PrintAllCodeLineCount();
    }

    /// <summary>
    /// 打印忽略空行的代码行数
    /// </summary>
    private void PrintIgnoreCodeCount()
    {
        ToolBagUtil.PrintIgnoreInvalidCodeLineCount();
    }
}
