using System;

[System.Serializable]
public class BuildingData
{
    public string id;

    public string slotId;

    public int buildingTypeId;

    public float x;
    public float y;

    public int level;
    public int currentXP;

    public BuildingData(string slotId, int buildingTypeId, float x, float y)
    {
        id = Guid.NewGuid().ToString();
        this.slotId = slotId;

        this.buildingTypeId = buildingTypeId;

        this.x = x;
        this.y = y;

        level = 1;
        currentXP = 0;
    }
}