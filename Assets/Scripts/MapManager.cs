using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public TMP_Text mapDistanceText;



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

        if (playerMovement != null && playerMovement.trackedTarget != null && mapDistanceText != null)
        {
            float distance = Vector2.Distance(playerMovement.transform.position, playerMovement.trackedTarget.position);
            mapDistanceText.text = FormatDistance(distance);
        }
        else if (mapDistanceText != null)
        {
            mapDistanceText.text = "";
        }

    }

    string FormatDistance(float distance)
    {
        if (distance >= 1000f)
            return $"{(distance / 1000f):F2} km";
        else
            return $"{distance:F0} m";
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
        if (focusRing == null || mapDistanceText == null) return;

        focusRing.gameObject.SetActive(true);
        focusRing.position = icon.position;
        focusRing.sizeDelta = icon.sizeDelta * 1.5f;

        // Offset the distance label slightly below the ring
        mapDistanceText.gameObject.SetActive(true);
        mapDistanceText.rectTransform.position = icon.position + new Vector3(0, -30f, 0); // adjust as needed
    }


    public void HideFocusRing()
    {
        if (focusRing != null)
            focusRing.gameObject.SetActive(false);

        if (mapDistanceText != null)
            mapDistanceText.gameObject.SetActive(false);
    }

}
