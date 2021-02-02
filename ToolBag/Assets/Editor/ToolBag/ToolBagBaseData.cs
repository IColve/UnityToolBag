using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ToolBag
{
    public class ToolBagBaseData
    {
        protected bool isShow;

        protected int index = 100;
        
        public virtual string Name
        {
            get
            {
                return "";
            }
        }

        public virtual bool IsShow
        {
            get
            {
                return isShow;
            }
            set
            {
                isShow = value;
            }
        }

        public virtual int SortIndex
        {
            get
            {
                return index;
            }
        }
        
        public virtual void ShowGUI()
        {
            
        }
    }
}