using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public GameManager gameManager;
    public BuildingManager buildingManager;

    public GameObject missionPanel;
    public TMP_InputField missionInputField;
    public Transform missionListContent;
    public GameObject missionItemPrefab;

    public TMP_Text buildingNameLevelText;
    public TMP_Text buildingXPText;
    public TMP_Text levelUpMessageText;

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
        {
            missionPanel.SetActive(true);
        }

        if (levelUpMessageText != null)
        {
            levelUpMessageText.text = "";
        }

        RefreshBuildingInfoUI();
        RefreshMissionUI();
    }

    public void CloseMissionPanel()
    {
        currentBuildingId = null;

        if (missionPanel != null)
        {
            missionPanel.SetActive(false);
        }

        if (levelUpMessageText != null)
        {
            levelUpMessageText.text = "";
        }

        ClearMissionUI();
        RefreshBuildingInfoUI();
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

    public void CompleteMission(MissionItemUI item, int rewardXP)
    {
        gameManager.AddXPToPlayer(rewardXP);

        MissionData mission = missions.Find(m => m.id == item.missionId);

        if (mission != null && buildingManager != null)
        {
            bool leveledUp = buildingManager.AddXPToBuilding(mission.buildingId, rewardXP);

            if (levelUpMessageText != null)
            {
                if (leveledUp)
                {
                    BuildingData building = buildingManager.GetBuildingData(mission.buildingId);
                    levelUpMessageText.text =
                        building.name + " leveled up to Lv." + building.level + "!";
                }
                else
                {
                    levelUpMessageText.text = "";
                }
            }
        }

        missions.RemoveAll(m => m.id == item.missionId);

        SaveMissions();

        RefreshBuildingInfoUI();
        RefreshMissionUI();
    }

    public void DeleteMission(MissionItemUI item)
    {
        missions.RemoveAll(m => m.id == item.missionId);

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

        if (itemUI == null)
        {
            Debug.LogError("MissionItemUI component is missing on missionItemPrefab.");
            return;
        }

        itemUI.Setup(this, mission.id, mission.title, mission.xpReward);
    }

    private void RefreshBuildingInfoUI()
    {
        if (buildingManager == null || string.IsNullOrEmpty(currentBuildingId))
        {
            if (buildingNameLevelText != null)
                buildingNameLevelText.text = "";

            if (buildingXPText != null)
                buildingXPText.text = "";

            return;
        }

        BuildingData building = buildingManager.GetBuildingData(currentBuildingId);

        if (building == null)
        {
            if (buildingNameLevelText != null)
                buildingNameLevelText.text = "Unknown Building";

            if (buildingXPText != null)
                buildingXPText.text = "";

            return;
        }

        if (buildingNameLevelText != null)
        {
            buildingNameLevelText.text = building.name + " Lv." + building.level;
        }

        if (buildingXPText != null)
        {
            buildingXPText.text =
                "Building XP: " + building.currentXP + " / " + building.xpToNextLevel;
        }
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

        if (saveData == null || saveData.missions == null)
            return;

        missions = saveData.missions;
    }
}