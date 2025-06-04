using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public GameObject mapPanel;
    public SpriteRenderer innerBackground;
    public PlayerMovement playerMovement;  // Drag in Player GameObject with PlayerMovement script

    // Map Tracked Objects
    public RectTransform playerIcon;
    public Transform playerTransform;
    public RectTransform blackHoleIcon;
    public Transform blackHoleTransform;

    // Configurable Settings
    public float playerIconSizeOverride = 30f; // Player icon fixed size
    public float maxWorldSize = 50f;           // Used for scaling other objects
    public float maxIconSize = 40f;
    public float minIconSize = 5f;

    private bool isMapVisible = false;
    public GameObject hudPanel;
    public RectTransform focusRing;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMapVisible = !isMapVisible;
            mapPanel.SetActive(isMapVisible);
            hudPanel.SetActive(!isMapVisible); // hide HUD when map is open
        }


        if (!isMapVisible || innerBackground == null || mapPanel == null)
            return;

        Bounds bounds = innerBackground.bounds;
        RectTransform mapRect = mapPanel.GetComponent<RectTransform>();

        if (playerTransform != null && playerIcon != null)
        {
            PositionMapIcon(playerTransform, playerIcon, bounds, mapRect, true, playerIconSizeOverride);
        }

        if (blackHoleTransform != null && blackHoleIcon != null)
        {
            PositionMapIcon(blackHoleTransform, blackHoleIcon, bounds, mapRect, false);
        }

        // ➕ Add more objects here:
        // PositionMapIcon(planetTransform, planetIcon, bounds, mapRect, false);
    }


    void PositionMapIcon(
        Transform worldObject,
        RectTransform icon,
        Bounds worldBounds,
        RectTransform mapRect,
        bool rotateIcon,
        float overrideSize = -1f // Optional: if > 0, use this size directly
    )
    {
        float normX = Mathf.InverseLerp(worldBounds.min.x, worldBounds.max.x, worldObject.position.x);
        float normY = Mathf.InverseLerp(worldBounds.min.y, worldBounds.max.y, worldObject.position.y);

        float mapWidth = mapRect.rect.width;
        float mapHeight = mapRect.rect.height;

        Vector2 mapPos = new Vector2(
            (normX - 0.5f) * mapWidth,
            (normY - 0.5f) * mapHeight
        );

        icon.anchoredPosition = mapPos;

        if (rotateIcon)
            icon.rotation = worldObject.rotation;

        float iconSize = overrideSize > 0f
            ? overrideSize
            : Mathf.Lerp(minIconSize, maxIconSize, Mathf.Clamp01(worldObject.localScale.x / maxWorldSize));

        icon.sizeDelta = new Vector2(iconSize, iconSize);
    }

    public void ShowFocusRingAt(RectTransform icon)
    {
        if (focusRing == null) return;

        focusRing.gameObject.SetActive(true);
        focusRing.position = icon.position;
        focusRing.sizeDelta = icon.sizeDelta * 1.5f; // scale ring a bit larger
    }

    public void HideFocusRing()
    {
        if (focusRing != null)
            focusRing.gameObject.SetActive(false);
    }

}
