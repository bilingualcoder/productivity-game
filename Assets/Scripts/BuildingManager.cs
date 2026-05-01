using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public GameObject buildingPrefab;
    public MissionManager missionManager;

    public BuildingSpriteSet[] buildingSpriteSets;

    private List<BuildingData> buildings = new List<BuildingData>();
    private Dictionary<string, Building> spawnedBuildings = new Dictionary<string, Building>();

    private const string SAVE_KEY = "BUILDING_SAVE_DATA";

    private void Start()
    {
        LoadBuildings();

        if (buildings.Count == 0)
        {
            CreateBuilding("Gym", 0, new Vector2(-3, 0));
            CreateBuilding("Study", 1, new Vector2(0, 0));
            CreateBuilding("Home", 2, new Vector2(3, 0));
        }
    }

    public void CreateBuilding(string name, int buildingTypeId, Vector2 position)
    {
        BuildingData data = new BuildingData(
            name,
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

        Debug.Log(
            "Spawn building: " + data.name +
            " / TypeId: " + data.buildingTypeId +
            " / Level: " + data.level
        );

        Building building = obj.GetComponent<Building>();

        if (building == null)
        {
            Debug.LogError("Building component is missing on BuildingPrefab.");
            return;
        }

        Sprite sprite = GetSpriteForBuilding(data.buildingTypeId, data.level);

        building.Setup(
            data.id,
            data.name,
            data.level,
            sprite,
            missionManager
        );

        spawnedBuildings[data.id] = building;
    }

    private Sprite GetSpriteForBuilding(int buildingTypeId, int level)
    {
        if (buildingSpriteSets == null || buildingSpriteSets.Length == 0)
        {
            Debug.LogWarning("Building Sprite Sets is empty.");
            return null;
        }

        BuildingSpriteSet spriteSet = null;

        foreach (BuildingSpriteSet set in buildingSpriteSets)
        {
            if (set.buildingTypeId == buildingTypeId)
            {
                spriteSet = set;
                break;
            }
        }

        if (spriteSet == null)
        {
            Debug.LogWarning("No sprite set found for buildingTypeId: " + buildingTypeId);
            return null;
        }

        if (spriteSet.levelSprites == null || spriteSet.levelSprites.Length == 0)
        {
            Debug.LogWarning("No level sprites found for buildingTypeId: " + buildingTypeId);
            return null;
        }

        int index = Mathf.Clamp(level - 1, 0, spriteSet.levelSprites.Length - 1);
        return spriteSet.levelSprites[index];
    }

    public BuildingData GetBuildingData(string buildingId)
    {
        return buildings.Find(b => b.id == buildingId);
    }

    public bool AddXPToBuilding(string buildingId, int xpAmount)
    {
        BuildingData building = buildings.Find(b => b.id == buildingId);

        if (building == null)
        {
            Debug.LogWarning("Building not found: " + buildingId);
            return false;
        }

        bool leveledUp = false;

        building.currentXP += xpAmount;

        while (building.currentXP >= building.xpToNextLevel)
        {
            building.currentXP -= building.xpToNextLevel;
            building.level++;
            building.xpToNextLevel += 50;
            leveledUp = true;

            Debug.Log(building.name + " leveled up to Lv." + building.level);
        }

        RefreshBuildingVisual(building);
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

        Sprite sprite = GetSpriteForBuilding(data.buildingTypeId, data.level);

        building.Setup(
            data.id,
            data.name,
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