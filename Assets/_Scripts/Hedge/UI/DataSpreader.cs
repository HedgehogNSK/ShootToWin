using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hedge.UI
{
    public enum DataType
    {
        Points,
        Health
    }

    public abstract class DataSpreader : MonoBehaviour
    {
        public DataType dataType;
        static public Action<DataType, object> OnUpdate;       
        static protected Dictionary<DataType,object> cacheObjDict = new Dictionary<DataType, object>();
       
        static DataSpreader()
        {
            OnUpdate += CachedLastHandle;
        }

        public DataSpreader()
        {
            OnUpdate += ParameterHandler;
        }
        static void CachedLastHandle(DataType type, object obj)
        {
            cacheObjDict[type] = obj;
        }

        private void Start()
        {
            foreach(var data in cacheObjDict)
            {
                Debug.Log("["+data.Key +";" +data.Value+"]");
            }
            if (cacheObjDict.TryGetValue(dataType, out object obj))
            {
                ParameterHandler(dataType, obj);
            }
        }
        protected abstract void ParameterHandler(DataType type, object obj);


    }

}
