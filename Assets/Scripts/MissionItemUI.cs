using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionItemUI : MonoBehaviour
{
    public TMP_Text missionNameText;
    public Button completeButton;
    public Button deleteButton;

    private MissionManager missionManager;
    private string missionName;
    private int rewardXP;

    public void Setup(MissionManager manager, string name, int xp)
    {
        missionManager = manager;
        missionName = name;
        rewardXP = xp;

        missionNameText.text = missionName;

        completeButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();

        completeButton.onClick.AddListener(CompleteMission);
        deleteButton.onClick.AddListener(DeleteMission);
    }

    private void CompleteMission()
    {
        missionManager.CompleteMission(this, rewardXP);
        completeButton.interactable = false;
        missionNameText.text = missionName + " Complete";
    }

    private void DeleteMission()
    {
        missionManager.DeleteMission(this);
    }
}