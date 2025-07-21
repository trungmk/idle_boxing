using UnityEngine;

public interface IInputFilter
{
    void TouchDown(Vector3 target);

    void Drag(Vector3 target);

    void TouchUp(Vector3 target);
}