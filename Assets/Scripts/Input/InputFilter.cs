using System;
using UnityEngine;

public class InputFilter : IInputFilter
{
    public Action<Vector3> OnTouchDown;
    public Action<Vector3> OnTouchUp;
    public Action<Vector3> OnDrag;

    public void Drag(Vector3 target)
    {
        if (OnDrag != null)
        {
            OnDrag(target);
        }
        else
        {
            Debug.LogWarning("OnDrag action is not set.");
        }
    }

    public void TouchDown(Vector3 target)
    {
        if (OnTouchDown != null)
        {
            OnTouchDown(target);
        }
        else
        {
            Debug.LogWarning("OnTouchDown action is not set.");
        }
    }

    public void TouchUp(Vector3 target)
    {
        if (OnTouchUp != null)
        {
            OnTouchUp(target);
        }
        else
        {
            Debug.LogWarning("OnTouchUp action is not set.");
        }
    }
}