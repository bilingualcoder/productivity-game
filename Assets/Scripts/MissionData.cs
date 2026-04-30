using System;

[Serializable]
public class MissionData
{
    public string id;
    public string title;
    public int xpReward;

    public MissionData(string title, int xpReward)
    {
        this.id = Guid.NewGuid().ToString();
        this.title = title;
        this.xpReward = xpReward;
    }
}