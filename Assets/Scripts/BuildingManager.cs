using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public GameObject buildingPrefab;
    public MissionManager missionManager;
    public Sprite[] buildingSprites;

    private List<BuildingData> buildings = new List<BuildingData>();

    private const string SAVE_KEY = "BUILDING_SAVE_DATA";

    private void Start()
    {
        LoadBuildings();

        if (buildings.Count == 0)
        {
            CreateBuilding("Gym", new Vector2(-3, 0));
            CreateBuilding("Study", new Vector2(0, 0));
            CreateBuilding("Home", new Vector2(3, 0));
        }
    }

    public void CreateBuilding(string name, Vector2 position)
    {
        int spriteIndex = Random.Range(0, buildingSprites.Length);

        BuildingData data = new BuildingData(
            name,
            position.x,
            position.y,
            spriteIndex
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

        Sprite sprite = buildingSprites[data.spriteIndex];

        building.Setup(data.id, sprite, missionManager);
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

        buildings = saveData.buildings;

        foreach (BuildingData building in buildings)
        {
            SpawnBuilding(building);
        }
    }
}