using UnityEngine;
using UnityEngine.EventSystems;

public class MapIconClick : MonoBehaviour, IPointerClickHandler
{
    public Transform worldTarget;
    public PlayerMovement playerMovement;
    public string displayName = "Target";

    public MapManager mapManager;
    public RectTransform iconRect; // this icon's RectTransform


public void OnPointerClick(PointerEventData eventData)
{
    if (playerMovement == null || worldTarget == null) return;

    bool alreadySelected = playerMovement.trackedTarget == worldTarget;

    if (alreadySelected)
    {
        playerMovement.trackedTarget = null;
        if (mapManager != null) mapManager.HideFocusRing();
    }
    else
    {
        playerMovement.trackedTarget = worldTarget;
        if (mapManager != null && iconRect != null) mapManager.ShowFocusRingAt(iconRect);
    }

    Debug.Log("Selected: " + displayName);
}

}
