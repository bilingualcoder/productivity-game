using System;

[System.Serializable]
public class MissionData
{
    public string id;
    public string title;
    public int xpReward;
    public string buildingId;

    public MissionData(string title, int xpReward, string buildingId)
    {
        id = Guid.NewGuid().ToString();
        this.title = title;
        this.xpReward = xpReward;
        this.buildingId = buildingId;
    }
}