using UnityEngine;

[System.Serializable]
public class BuildingSlot
{
    [Header("Slot Info")]
    public string slotId;

    [Tooltip("Inspector에서 구분하기 위한 이름")]
    public string label;

    [Header("Building Info")]
    public int buildingTypeId;

    [Header("Position")]
    public Vector2 position;

    [Header("Unlock")]
    public int unlockPlayerLevel = 1;

    [Tooltip("게임 시작 시 처음부터 지어져 있는 건물인지")]
    public bool builtAtStart = false;

    [Header("Slot Visuals")]
    [Tooltip("아직 플레이어 레벨이 부족할 때 보일 스프라이트")]
    public Sprite lockedSprite;

    [Tooltip("레벨은 충분하지만 아직 건설하지 않았을 때 보일 스프라이트")]
    public Sprite buildableSprite;
}