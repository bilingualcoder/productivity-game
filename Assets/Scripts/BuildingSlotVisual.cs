using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class BuildingSlotVisual : MonoBehaviour, IPointerClickHandler
{
    private SpriteRenderer spriteRenderer;

    private string slotId;
    private BuildingManager buildingManager;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(string slotId, Sprite sprite, BuildingManager buildingManager)
    {
        this.slotId = slotId;
        this.buildingManager = buildingManager;

        SetSprite(sprite);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked empty slot: " + slotId);

        if (buildingManager == null)
        {
            Debug.LogWarning("BuildingManager is null on BuildingSlotVisual.");
            return;
        }

        buildingManager.RequestBuildOnSlot(slotId);
    }
}