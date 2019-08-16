using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Controllers.Mobile
{
    public class Joystick : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
    {
        [Range(0.0f,0.99f)]
        [SerializeField]float noizeGate =0;
        [Range(0.2f, 3f)]
        [SerializeField]float handlerRange = 1;
        [SerializeField]protected RectTransform joystickBase;
        [SerializeField]protected RectTransform joystickHandle;
        

        protected bool released = false;
        protected Vector2 input = Vector2.zero;
        protected Canvas canvas;
        protected Vector2 handleStartPosition;
        
        public virtual void OnDrag(PointerEventData eventData)
        {
            input = CalcInput(eventData);           
            joystickHandle.anchoredPosition = handleStartPosition +input * joystickBase.sizeDelta / 2 * canvas.scaleFactor* handlerRange;

        }

        protected Vector2 CalcInput(PointerEventData eventData)
        {
            Camera cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                cam = canvas.worldCamera;

            Vector2 pos = RectTransformUtility.WorldToScreenPoint(cam, joystickBase.position);
            Vector2 input = (eventData.position - pos - joystickBase.rect.center * canvas.scaleFactor) / (joystickBase.sizeDelta / 2 * canvas.scaleFactor);

            return InputFilter(input);
        }
        
        protected Vector2 InputFilter(Vector2 input)
        {
            if (input.sqrMagnitude > 1)return input.normalized;
            if (input.sqrMagnitude < noizeGate)return Vector2.zero;
            return input;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            released = false;

        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            released = true;
            joystickHandle.anchoredPosition = handleStartPosition;
           
        }
        
        protected void Awake()
        {
            handleStartPosition = joystickHandle.anchoredPosition;
            canvas = GetComponentInParent<Canvas>();

        }        
    }
}