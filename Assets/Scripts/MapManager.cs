using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel;
    public SpriteRenderer innerBackground;
    public RectTransform playerIcon;
    public Transform playerTransform;

    public float mapScale = 0.01f; // Scale world position to fit map UI

    private bool isMapVisible = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMapVisible = !isMapVisible;
            mapPanel.SetActive(isMapVisible);
        }

        if (isMapVisible && playerIcon != null && playerTransform != null && innerBackground != null)
        {
            Bounds bounds = innerBackground.bounds;

            // Normalize player's position within inner bounds (0–1 range)
            float normX = Mathf.InverseLerp(bounds.min.x, bounds.max.x, playerTransform.position.x);
            float normY = Mathf.InverseLerp(bounds.min.y, bounds.max.y, playerTransform.position.y);

            // Convert to map panel space (assuming anchored center)
            float mapWidth = mapPanel.GetComponent<RectTransform>().rect.width;
            float mapHeight = mapPanel.GetComponent<RectTransform>().rect.height;

            Vector2 mapPos = new Vector2(
                (normX - 0.5f) * mapWidth,
                (normY - 0.5f) * mapHeight
            );

            playerIcon.anchoredPosition = mapPos;
            playerIcon.rotation = playerTransform.rotation; // optional
        }

    }
}
