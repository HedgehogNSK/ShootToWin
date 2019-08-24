using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hedge.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class TransformDataSpreader : DataSpreader
    {
        RectTransform mask;
#pragma warning disable CS0649
        [SerializeField] int minX;
        [SerializeField] int maxX;
#pragma warning restore CS0649

        static protected Dictionary<DataType, object> MaxParamDict = new Dictionary<DataType, object>();      

        static public void ForceSetMaxParameter(DataType type, object obj)
        {
            MaxParamDict.Add(type, obj);
        }

        protected override void ParameterHandler(DataType type, object parameter)
        {
            if (!this || dataType != type) return;
            //This check must be here, cause gameobject can be unactive, but it must get information already
            if (!mask) mask = GetComponent<RectTransform>();
            if (mask)
            {
                if (parameter is Int32 )
                {
                    TransformResize((int)parameter);
                }
                else if(parameter is Single)
                {
                    TransformResize((float)parameter);
                }
                else if(parameter is Double)
                {
                    TransformResize((double)parameter);
                }                
                else
                {
                    Debug.LogWarning("Для данного типа данных не написан сценарий обработки.");
                }
            }
        }
        
        private void TransformResize(float parameter)
        {
            object maxParameter;
            if (!MaxParamDict.TryGetValue(dataType, out maxParameter))
                maxParameter = 0.0f;
            
            
            if (maxParameter is float)
            {
                if (parameter > (float)maxParameter)
                {
                    MaxParamDict[dataType] = parameter;
                }
                int delta = maxX - minX;
                float ratio = parameter / (float)maxParameter;
                float currentX = (ratio > 1 ? 1 : ratio) * delta + minX;

                mask.sizeDelta = new Vector2(currentX, mask.sizeDelta.y);
            }
        }

        private void TransformResize(int parameter)
        {
            TransformResize((float)parameter);
        }
        private void TransformResize(double parameter)
        {
            TransformResize((float)parameter);
        }

    }
}
