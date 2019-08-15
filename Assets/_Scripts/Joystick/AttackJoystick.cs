using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Controllers.Mobile
{
    public class AttackJoystick : Joystick
    {
        static public event System.Action<Joystick, Vector2,bool> OnAim;
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnAim?.Invoke(this, input, released);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            OnAim?.Invoke(this, input, released);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnAim?.Invoke(this, input, released);
        }

    }
}
