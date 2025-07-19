using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoSingleton<CameraManager>
{
    [Header("Camera reference")]
    [SerializeField] 
    private Camera _mainCamera;

    [SerializeField]
    private CinemachineCamera _cinemachineCamera;

    [SerializeField]
    private UICamera _uiCamera;

    public Camera MainCamera => _mainCamera;

    public CinemachineCamera CinemachineCamera => _cinemachineCamera;

    public UICamera UICamera => _uiCamera;

    public void SetTrackingTarget(Transform transform)
    {
        if (_cinemachineCamera != null)
        {
            _cinemachineCamera.LookAt = transform;
        }
        else
        {
            Debug.LogWarning("CinemachineCamera is not assigned.");
        }
    }

    public void SetFollow(Transform transform)
    {
        if (_cinemachineCamera != null)
        {
            _cinemachineCamera.Follow = transform;
        }
        else
        {
            Debug.LogWarning("CinemachineCamera is not assigned.");
        }
    }
}
