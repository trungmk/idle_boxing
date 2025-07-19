using UnityEngine;

public enum Orientation { Vertical, Horizontal }

public class UICamera : MonoBehaviour
{
    [SerializeField]
    private Camera _uiCamera;

    [Header("Reference Resolution (width:height)")]
    [SerializeField, Min(1f)]
    private float _referenceWidth = 1080f;

    [SerializeField, Min(1f)]
    private float _referenceHeight = 1920f;

    [Header("Reference Orthographic Size")]
    [SerializeField, Min(0.1f)]
    private float _referenceOrthoSizeVertical = 10.5f;

    [SerializeField, Min(0.1f)]
    private float _referenceOrthoSizeHorizontal = 10.5f;

    [Header("Orientation")]
    [SerializeField]
    private Orientation _orientation = Orientation.Horizontal;

    public bool IsScaleByVertical { get; private set; }
    public bool IsScaleByHorizontal { get; private set; }
    public Camera Camera => _uiCamera;

    private void Awake()
    {
        AdjustUIRatio();
    }

    public void AdjustUIRatio()
    {
        if (_uiCamera == null)
        {
            return;
        } 

        float targetAspect = _referenceWidth / _referenceHeight;
        float windowAspect = (float) Screen.width / Screen.height;

        switch (_orientation)
        {
            case Orientation.Vertical:

                if (windowAspect > targetAspect)
                {
                    _uiCamera.orthographicSize = _referenceOrthoSizeVertical;
                    IsScaleByVertical = false;
                    IsScaleByHorizontal = false;
                }
                else
                {
                    float scaleFactor = targetAspect / windowAspect;
                    _uiCamera.orthographicSize = _referenceOrthoSizeVertical * scaleFactor;
                    IsScaleByVertical = true;
                    IsScaleByHorizontal = false;
                }

                break;

            case Orientation.Horizontal:
                float reverseTargetAspect = _referenceHeight / _referenceWidth;
                float windowAspectInv = (float) Screen.height / Screen.width;

                if (windowAspectInv > reverseTargetAspect)
                {
                    _uiCamera.orthographicSize = _referenceOrthoSizeHorizontal;
                    IsScaleByHorizontal = false;
                    IsScaleByVertical = false;
                }
                else
                {
                    float scaleFactor = reverseTargetAspect / windowAspectInv;
                    _uiCamera.orthographicSize = _referenceOrthoSizeHorizontal * scaleFactor;
                    IsScaleByHorizontal = true;
                    IsScaleByVertical = false;
                }

                break;
        }
    }
}