using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public GameObject buildingPrefab;
    public MissionManager missionManager;

    public BuildingTypeData[] buildingTypes;

    private List<BuildingData> buildings = new List<BuildingData>();
    private Dictionary<string, Building> spawnedBuildings = new Dictionary<string, Building>();

    private const string SAVE_KEY = "BUILDING_SAVE_DATA";

    private void Start()
    {
        LoadBuildings();

        if (buildings.Count == 0)
        {
            CreateBuilding(0, new Vector2(-3, 0)); // Gym
            CreateBuilding(1, new Vector2(0, 0));  // Study
            CreateBuilding(2, new Vector2(3, 0));  // Home
        }
    }

    public void CreateBuilding(int buildingTypeId, Vector2 position)
    {
        BuildingData data = new BuildingData(
            buildingTypeId,
            position.x,
            position.y
        );

        buildings.Add(data);
        SpawnBuilding(data);
        SaveBuildings();
    }

    private void SpawnBuilding(BuildingData data)
    {
        GameObject obj = Instantiate(buildingPrefab);
        obj.transform.position = new Vector3(data.x, data.y, 0);

        Building building = obj.GetComponent<Building>();

        if (building == null)
        {
            Debug.LogError("Building component is missing on BuildingPrefab.");
            return;
        }

        BuildingTypeData typeData = GetBuildingTypeData(data.buildingTypeId);

        if (typeData == null)
        {
            Debug.LogError("BuildingTypeData not found. TypeId: " + data.buildingTypeId);
            return;
        }

        Sprite sprite = typeData.GetSpriteForLevel(data.level);

        building.Setup(
            data.id,
            typeData.buildingName,
            data.level,
            sprite,
            missionManager
        );

        spawnedBuildings[data.id] = building;

        Debug.Log(
            "Spawn building: " +
            typeData.buildingName +
            " / TypeId: " + data.buildingTypeId +
            " / Level: " + data.level
        );
    }

    public BuildingData GetBuildingData(string buildingId)
    {
        return buildings.Find(b => b.id == buildingId);
    }

    public BuildingTypeData GetBuildingTypeData(int buildingTypeId)
    {
        foreach (BuildingTypeData typeData in buildingTypes)
        {
            if (typeData != null && typeData.buildingTypeId == buildingTypeId)
            {
                return typeData;
            }
        }

        return null;
    }

    public string GetBuildingName(string buildingId)
    {
        BuildingData building = GetBuildingData(buildingId);

        if (building == null)
            return "Unknown Building";

        BuildingTypeData typeData = GetBuildingTypeData(building.buildingTypeId);

        if (typeData == null)
            return "Unknown Building";

        return typeData.buildingName;
    }

    public int GetXPToNextLevel(BuildingData building)
    {
        if (building == null)
            return 0;

        BuildingTypeData typeData = GetBuildingTypeData(building.buildingTypeId);

        if (typeData == null)
            return 0;

        return typeData.GetXPToNextLevel(building.level);
    }

    public bool AddXPToBuilding(string buildingId, int xpAmount)
    {
        BuildingData building = buildings.Find(b => b.id == buildingId);

        if (building == null)
        {
            Debug.LogWarning("Building not found: " + buildingId);
            return false;
        }

        BuildingTypeData typeData = GetBuildingTypeData(building.buildingTypeId);

        if (typeData == null)
        {
            Debug.LogWarning("BuildingTypeData not found: " + building.buildingTypeId);
            return false;
        }

        bool leveledUp = false;

        building.currentXP += xpAmount;

        while (building.level < typeData.maxLevel &&
               building.currentXP >= typeData.GetXPToNextLevel(building.level))
        {
            building.currentXP -= typeData.GetXPToNextLevel(building.level);
            building.level++;
            leveledUp = true;

            Debug.Log(typeData.buildingName + " leveled up to Lv." + building.level);
        }

        if (building.level >= typeData.maxLevel)
        {
            building.currentXP = 0;
        }

        RefreshBuildingVisual(building);

        //if (leveledUp && spawnedBuildings.ContainsKey(building.id))
        //{
        //    spawnedBuildings[building.id].PlayLevelUpEffect();
        //}

        SaveBuildings();

        return leveledUp;
    }

    private void RefreshBuildingVisual(BuildingData data)
    {
        if (!spawnedBuildings.ContainsKey(data.id))
        {
            Debug.LogWarning("Spawned building not found: " + data.id);
            return;
        }

        Building building = spawnedBuildings[data.id];
        BuildingTypeData typeData = GetBuildingTypeData(data.buildingTypeId);

        if (typeData == null)
            return;

        Sprite sprite = typeData.GetSpriteForLevel(data.level);

        building.Setup(
            data.id,
            typeData.buildingName,
            data.level,
            sprite,
            missionManager
        );
    }

    private void SaveBuildings()
    {
        BuildingSaveData saveData = new BuildingSaveData();
        saveData.buildings = buildings;

        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadBuildings()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
            return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        BuildingSaveData saveData = JsonUtility.FromJson<BuildingSaveData>(json);

        if (saveData == null || saveData.buildings == null)
            return;

        buildings = saveData.buildings;

        foreach (BuildingData building in buildings)
        {
            SpawnBuilding(building);
        }
    }
}