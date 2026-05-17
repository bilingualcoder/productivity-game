using UnityEngine;

[CreateAssetMenu(fileName = "BuildingLayoutData", menuName = "Game/Building Layout Data")]
public class BuildingLayoutData : ScriptableObject
{
    public BuildingSlot[] buildingSlots;
}