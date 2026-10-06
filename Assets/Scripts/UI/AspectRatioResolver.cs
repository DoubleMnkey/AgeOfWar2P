using UnityEngine;

[ExecuteAlways]
public class AspectRatioResolver : MonoBehaviour
{
    [SerializeField] private float targetAspectX = 16f;
    [SerializeField] private float targetAspectY = 9f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
        }

        ApplyAspectRatio();
    }

    private void OnEnable()
    {
        ApplyAspectRatio();
    }

    private void OnPreRender()
    {
        ApplyAspectRatio();
    }

    public void ApplyAspectRatio()
    {
        if (cam == null) cam = GetComponent<Camera>();

        if (Screen.width <= 0 || Screen.height <= 0) return;

        float targetAspect = targetAspectX / targetAspectY;
        float windowAspect = (float)Screen.width / (float)Screen.height;

        if (float.IsNaN(windowAspect) || float.IsInfinity(windowAspect)) return;

        float scaleHeight = targetAspect / windowAspect;

        if (scaleHeight < 1.0f)
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0f;
            cam.rect = rect;
        }
        else
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = 1.0f / scaleHeight;
            rect.x = 0f;
            rect.y = (1.0f - (1.0f / scaleHeight)) / 2.0f;
            cam.rect = rect;
        }
    }
}