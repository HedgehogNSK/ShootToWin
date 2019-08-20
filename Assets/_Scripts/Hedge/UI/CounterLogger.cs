using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hedge.UI
{
    public enum CounterType
    {
        Points,
        Health
    }

    public abstract class CounterLogger : MonoBehaviour
    {
        public CounterType cntrType;
        static public Action<CounterType, object> OnUpdate;
    
    }

}
