using UnityEngine;

public class CameraEdgeColor : MonoBehaviour
{
    public Camera mainCamera;                // Assign your Main Camera
    public Transform outerBackground;        // Assign your large black background sprite
    public Color innerColor = Color.black;   // Normal space color
    public Color edgeColor = Color.red;      // Warning color
    public float edgeFadeDistance = 50f;     // How far from the edge before it starts fading

    private Vector2 worldMinBounds;
    private Vector2 worldMaxBounds;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (outerBackground != null)
        {
            Vector3 center = outerBackground.position;
            Vector3 size = outerBackground.localScale;

            float halfWidth = size.x / 2f;
            float halfHeight = size.y / 2f;

            worldMinBounds = new Vector2(center.x - halfWidth, center.y - halfHeight);
            worldMaxBounds = new Vector2(center.x + halfWidth, center.y + halfHeight);
        }
        else
        {
            Debug.LogWarning("Outer background not assigned.");
        }
    }

    void Update()
    {
        UpdateCameraBackground();
    }

    void UpdateCameraBackground()
    {
        Vector2 pos = transform.position;

        float distanceToEdgeX = Mathf.Min(pos.x - worldMinBounds.x, worldMaxBounds.x - pos.x);
        float distanceToEdgeY = Mathf.Min(pos.y - worldMinBounds.y, worldMaxBounds.y - pos.y);
        float distanceToEdge = Mathf.Min(distanceToEdgeX, distanceToEdgeY);

        float t = Mathf.InverseLerp(edgeFadeDistance, 0f, distanceToEdge); // 0 = center, 1 = at edge
        mainCamera.backgroundColor = Color.Lerp(innerColor, edgeColor, t);
    }
}
