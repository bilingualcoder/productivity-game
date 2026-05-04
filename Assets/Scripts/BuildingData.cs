using System;

[System.Serializable]
public class BuildingData
{
    public string id;

    public int buildingTypeId;

    public float x;
    public float y;

    public int level;
    public int currentXP;

    public BuildingData(int buildingTypeId, float x, float y)
    {
        id = Guid.NewGuid().ToString();

        this.buildingTypeId = buildingTypeId;

        this.x = x;
        this.y = y;

        level = 1;
        currentXP = 0;
    }
}