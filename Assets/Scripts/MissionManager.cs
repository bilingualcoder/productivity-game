using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject missionPanel;
    public TMP_InputField missionInputField;
    public Transform missionListContent;
    public GameObject missionItemPrefab;

    public int defaultRewardXP = 20;

    private List<MissionData> missions = new List<MissionData>();

    private string currentBuildingId;

    private const string SAVE_KEY = "MISSION_SAVE_DATA";

    private void Start()
    {
        LoadMissions();

        if (missionPanel != null)
            missionPanel.SetActive(false);
    }

    public void OpenMissionPanel(string buildingId)
    {
        currentBuildingId = buildingId;

        if (missionPanel != null)
            missionPanel.SetActive(true);

        RefreshMissionUI();
    }

    public void AddMission()
    {
        if (string.IsNullOrEmpty(currentBuildingId))
            return;

        string missionName = missionInputField.text.Trim();

        if (string.IsNullOrEmpty(missionName))
            return;

        MissionData mission = new MissionData(
            missionName,
            defaultRewardXP,
            currentBuildingId
        );

        missions.Add(mission);

        missionInputField.text = "";

        SaveMissions();
        RefreshMissionUI();
    }

    private void RefreshMissionUI()
    {
        ClearMissionUI();

        foreach (MissionData mission in missions)
        {
            if (mission.buildingId == currentBuildingId)
            {
                CreateMissionUI(mission);
            }
        }
    }

    private void ClearMissionUI()
    {
        foreach (Transform child in missionListContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateMissionUI(MissionData mission)
    {
        GameObject item = Instantiate(missionItemPrefab, missionListContent);

        MissionItemUI itemUI = item.GetComponent<MissionItemUI>();
        itemUI.Setup(this, mission.id, mission.title, mission.xpReward);
    }

    public void CompleteMission(MissionItemUI item, int rewardXP)
    {
        gameManager.AddXPToPlayer(rewardXP);

        missions.RemoveAll(m => m.id == item.missionId);

        SaveMissions();
        RefreshMissionUI();
    }

    public void DeleteMission(MissionItemUI item)
    {
        missions.RemoveAll(m => m.id == item.missionId);

        SaveMissions();
        RefreshMissionUI();
    }

    private void SaveMissions()
    {
        MissionSaveData saveData = new MissionSaveData();
        saveData.missions = missions;

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadMissions()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        MissionSaveData saveData = JsonUtility.FromJson<MissionSaveData>(json);

        missions = saveData.missions;
    }

    public void CloseMissionPanel()
    {
        currentBuildingId = null;

        if (missionPanel != null)
            missionPanel.SetActive(false);

        ClearMissionUI();
    }
}