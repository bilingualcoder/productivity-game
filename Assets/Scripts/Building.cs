using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{
    public string buildingId;

    private MissionManager missionManager;

    public SpriteRenderer spriteRenderer;
    public TMP_Text nameText;
    public TMP_Text levelText;

    public void Setup(
        string id,
        string buildingName,
        int level,
        Sprite sprite,
        MissionManager manager
    )
    {
        buildingId = id;
        missionManager = manager;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (nameText != null)
        {
            nameText.text = buildingName;
        }

        if (levelText != null)
        {
            levelText.text = "Lv." + level;
        }

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