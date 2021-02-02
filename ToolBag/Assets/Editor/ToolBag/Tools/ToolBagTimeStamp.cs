using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolBag
{
    public class ToolBagTimeStamp : ToolBagBaseData
    {
        public override string Name
        {
            get
            {
                return "时间转换";
            }
        }

        public string inputValueStr = "";
        public string outTimeStr = "";
        
        public override void ShowGUI()
        {
            inputValueStr = GUILayout.TextField(inputValueStr);
            
            GUILayout.Space(1);
            
            if (GUILayout.Button("转换"))
            {
                long time = 0;
                if (long.TryParse(inputValueStr, out time))
                {
                    outTimeStr = GetDateTime(time).ToString();
                }
            }
            
            GUILayout.Space(1);
            
            outTimeStr = GUILayout.TextField(outTimeStr);
        }

        public static DateTime GetDateTime(long lStamp)
        {
            TimeSpan pTimeSpan = GetTimeSpan(lStamp);
            DateTime pDateTime = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);

            pDateTime = pDateTime.Add(pTimeSpan);

            return pDateTime.ToLocalTime();
        }
        
        public static TimeSpan GetTimeSpan(long lSeconds)
        {
            long lTime = long.Parse(lSeconds + @"0000000");
            return new TimeSpan(lTime);
        }
    }
}