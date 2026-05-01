using System;

[System.Serializable]
public class BuildingData
{
    public string id;
    public string name;

    public int buildingTypeId;

    public float x;
    public float y;

    public int level;
    public int currentXP;
    public int xpToNextLevel;

    public BuildingData(string name, int buildingTypeId, float x, float y)
    {
        id = Guid.NewGuid().ToString();

        this.name = name;
        this.buildingTypeId = buildingTypeId;

        this.x = x;
        this.y = y;

        level = 1;
        currentXP = 0;
        xpToNextLevel = 100;
    }
}