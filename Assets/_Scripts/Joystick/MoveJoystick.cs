using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers.Mobile
{
    public class MoveJoystick : Joystick
    {
        static public event System.Action<Joystick, Vector2> OnMove;

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            OnMove?.Invoke(this, input);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            joystickHandle.position = eventData.pressPosition;

            OnMove?.Invoke(this, input);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            input = Vector2.zero;
            OnMove?.Invoke(this, input);
        }
    }
}