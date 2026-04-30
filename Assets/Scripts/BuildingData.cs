using System;

[System.Serializable]
public class BuildingData
{
    public string id;
    public string name;
    public float x;
    public float y;
    public int spriteIndex;

    public BuildingData(string name, float x, float y, int spriteIndex)
    {
        id = Guid.NewGuid().ToString();
        this.name = name;
        this.x = x;
        this.y = y;
        this.spriteIndex = spriteIndex;
    }
}