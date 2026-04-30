using UnityEngine;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public GameManager gameManager;

    public TMP_InputField missionInputField;
    public Transform missionListContent;
    public GameObject missionItemPrefab;

    public int defaultRewardXP = 20;

    public void AddMission()
    {
        string missionName = missionInputField.text.Trim();

        if (string.IsNullOrEmpty(missionName))
            return;

        GameObject item = Instantiate(missionItemPrefab, missionListContent);

        MissionItemUI itemUI = item.GetComponent<MissionItemUI>();
        itemUI.Setup(this, missionName, defaultRewardXP);

        missionInputField.text = "";
    }

    public void CompleteMission(MissionItemUI item, int rewardXP)
    {
        gameManager.AddXPToPlayer(rewardXP);
    }

    public void DeleteMission(MissionItemUI item)
    {
        Destroy(item.gameObject);
    }
}