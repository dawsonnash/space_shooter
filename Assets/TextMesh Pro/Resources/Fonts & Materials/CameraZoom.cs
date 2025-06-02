using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomedOutSize = 20f;
    public float zoomedInSize = 5f;
    public float zoomSpeed = 5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = zoomedInSize;
    }

    void Update()
    {
        bool isHoldingZoomKey = Input.GetKey(KeyCode.E);
        float targetSize = isHoldingZoomKey ? zoomedOutSize : zoomedInSize;

        // Smoothly zoom toward target
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);
    }
}
