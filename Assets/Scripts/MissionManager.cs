using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    public GameManager gameManager;

    public TMP_InputField missionInputField;
    public Transform missionListContent;
    public GameObject missionItemPrefab;

    public int defaultRewardXP = 20;

    private List<MissionData> missions = new List<MissionData>();

    private const string SAVE_KEY = "MISSION_SAVE_DATA";

    private void Start()
    {
        LoadMissions();
    }

    public void AddMission()
    {
        string missionName = missionInputField.text.Trim();

        if (string.IsNullOrEmpty(missionName))
            return;

        MissionData mission = new MissionData(missionName, defaultRewardXP);
        missions.Add(mission);

        CreateMissionUI(mission);

        missionInputField.text = "";

        SaveMissions();
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

        Destroy(item.gameObject);

        SaveMissions();
    }

    public void DeleteMission(MissionItemUI item)
    {
        missions.RemoveAll(m => m.id == item.missionId);

        Destroy(item.gameObject);

        SaveMissions();
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

        foreach (MissionData mission in missions)
        {
            CreateMissionUI(mission);
        }
    }
}