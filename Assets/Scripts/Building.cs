using UnityEngine;

public class Building : MonoBehaviour
{
    public string buildingId;
    private MissionManager missionManager;

    public void Setup(string id, Sprite sprite, MissionManager manager)
    {
        buildingId = id;
        missionManager = manager;

        GetComponent<SpriteRenderer>().sprite = sprite;

        Debug.Log("Building setup complete: " + buildingId);
    }

    public void OpenMission()
    {
        Debug.Log("Building clicked: " + buildingId);

        if (missionManager != null)
        {
            missionManager.OpenMissionPanel(buildingId);
        }
        else
        {
            Debug.LogWarning("MissionManager is null");
        }
    }
}