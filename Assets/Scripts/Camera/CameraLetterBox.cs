using UnityEngine;

public class CameraLetterbox : MonoBehaviour
{
    [SerializeField] private int _targetXAspect = 16;
    [SerializeField] private int _targetYAspect = 9;
    private float _targetAspectRatio;

    private Camera _camera;
    private float _lastWidth;
    private float _lastHeight;
    private Rect _originalCameraRect;

    void Awake()
    {
        _targetAspectRatio = (float)_targetXAspect / _targetYAspect;
        _camera = GetComponent<Camera>();
        _originalCameraRect = _camera.rect;
        UpdateCamera();
    }

    void Update()
    {
        if (Screen.width != _lastWidth || Screen.height != _lastHeight)
        {
            UpdateCamera();
        }
    }

    void OnDisable()
    {
        if (_camera != null)
            _camera.rect = _originalCameraRect;
    }

    void UpdateCamera()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        float windowAspectRatio = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspectRatio / _targetAspectRatio;

        Rect rect = _camera.rect;

        if (scaleHeight < 1.0f)
        {
            // Add letterbox (black bars top/bottom)
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            // Add pillarbox (black bars left/right)
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
        }

        _camera.rect = rect;
    }

}