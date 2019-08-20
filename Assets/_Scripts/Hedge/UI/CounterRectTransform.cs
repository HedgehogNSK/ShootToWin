using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hedge.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class CounterRectTransform : CounterLogger
    {
        RectTransform mask;
        [SerializeField] int minX;
        [SerializeField] int maxX;
        [SerializeField] float maxParameter;
        public CounterRectTransform()
        {
            OnUpdate += ParameterCatcher;
        }

        private void ParameterCatcher(CounterType type, object parameter)
        {
            if (!this || this.cntrType != type) return;
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
            int delta = maxX - minX;
            Debug.Log(parameter);
            float ratio = parameter / maxParameter;
            float currentX = (ratio>1? 1:ratio) * delta +minX;

            mask.sizeDelta = new Vector2(currentX, mask.sizeDelta.y);
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
