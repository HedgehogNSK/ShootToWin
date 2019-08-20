﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Hedge.Tools;

namespace Hedge
{
    namespace UI
    {
       

        [RequireComponent(typeof(Text))]
        public class CounterText : CounterLogger
        {
            Text counterTXT;           

            public CounterText()
            {
                OnUpdate += TextCatcher;
            }


            void ChangeText(string str)
            {
                counterTXT.text = str;
            }
            void ChangeText(int str)
            {
                ChangeText((float)str);
            }

            void ChangeText(float number)
            {

                counterTXT.text = number.ToShortNumber();
            }

            void TextCatcher(CounterType txtType, object obj)
            {
                if (!this || this.cntrType != txtType) return;
                //This check must be here, cause gameobject can be unactive, but it must get information already
                if (!counterTXT) counterTXT = GetComponent<Text>();
                if (counterTXT)
                {
                    if ((obj is Int32) || (obj is Single))
                    {
                        ChangeText((int)obj);
                    }
                    else if (obj is String)
                    {
                        ChangeText((string)obj);
                    }
                    else
                    {
                        Debug.LogWarning("Для данного типа данных не написан сценарий обработки.");
                    }
                }
            }


            private void OnDestroy()
            {
                OnUpdate -= TextCatcher;
            }
        }
    }
}