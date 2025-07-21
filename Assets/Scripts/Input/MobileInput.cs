using UnityEngine;

public class MobileInput : MonoBehaviour
{
    private const float TIME_HOLD_DRAG = 0.06f;

    private const float MIN_MAGNITUDE_DIRECTION = 5f;

    private float _timeHold;

    private bool _isTouchDown;

    private Vector3 _touchStartPosition;

    private IInputFilter _inputFilter;

    protected void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    public void RegisterInputFilter(IInputFilter inputFilter)
    {
        _inputFilter = inputFilter;
    }    

    public void UnregisterInputFilter()
    {
        _inputFilter = null;
    }

    private void Update()
    {
        // Handle touch
        if (Input.touchCount > 0)
        {
            if (!_isTouchDown)
            {
                HandleTouchDown();
            }

            if (_isTouchDown)
            {
                _timeHold += Time.deltaTime;
            }

            if (_timeHold > TIME_HOLD_DRAG)
            {
                HandleDrag();
            }
        }
        else if (_isTouchDown)
        {
            HandleTouchUp();
        }
    }

    private void HandleTouchDown()
    {
        _isTouchDown = true;
        _touchStartPosition = Input.mousePosition;
        //InGameInputHandler.Instance.TouchDownEvent(Input.mousePosition);
        if (_inputFilter != null)
        {
            _inputFilter.TouchDown(_touchStartPosition);
        }
    }

    private void HandleTouchUp()
    {
        _isTouchDown = false;
        _timeHold = 0f;
        //InGameInputHandler.Instance.TouchUpEvent(Input.mousePosition);
        if (_inputFilter != null)
        {
            _inputFilter.TouchUp(Input.mousePosition);
        }
    }

    private void HandleDrag()
    {
        if (Mathf.Abs(Input.mousePosition.x - float.MaxValue) < float.Epsilon
           || Mathf.Abs(Input.mousePosition.y - float.MaxValue) < float.Epsilon)
        {
            return;
        }

        Vector3 direction = Input.mousePosition - _touchStartPosition;
        bool isInputMoving = direction.sqrMagnitude >= MIN_MAGNITUDE_DIRECTION;

        if (isInputMoving)
        {
            //InGameInputHandler.Instance.DragEvent(Input.mousePosition);
            if (_inputFilter != null)
            {
                _inputFilter.Drag(Input.mousePosition);
            }
            _touchStartPosition = Input.mousePosition;
        }
    }
}