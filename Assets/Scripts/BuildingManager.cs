using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    [Header("References")]
    public GameObject buildingPrefab;
    public MissionManager missionManager;

    [Header("Building Data")]
    public BuildingTypeData[] buildingTypes;
    public BuildingLayoutData initialLayout;

    [Header("Optional")]
    public GameObject emptySlotPrefab;

    private int currentPlayerLevel = 1;

    private List<BuildingData> buildings = new List<BuildingData>();
    private Dictionary<string, Building> spawnedBuildings = new Dictionary<string, Building>();
    private Dictionary<string, GameObject> spawnedEmptySlots = new Dictionary<string, GameObject>();

    private const string SAVE_KEY = "BUILDING_SAVE_DATA";

    private void Start()
    {
        LoadBuildings();

        if (buildings.Count == 0)
        {
            CreateStartBuildingsFromLayout();
        }

        SpawnEmptySlots();
    }

    private void CreateStartBuildingsFromLayout()
    {
        if (initialLayout == null)
        {
            Debug.LogWarning("Initial layout is not assigned.");
            return;
        }

        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot == null)
                continue;

            if (slot.builtAtStart)
            {
                CreateBuildingFromSlot(slot);
            }
        }
    }

    public void CreateBuildingFromSlot(BuildingSlot slot)
    {
        if (slot == null)
            return;

        if (IsSlotAlreadyBuilt(slot.slotId))
        {
            Debug.LogWarning("This slot already has a building: " + slot.slotId);
            return;
        }

        BuildingData data = new BuildingData(
            slot.slotId,
            slot.buildingTypeId,
            slot.position.x,
            slot.position.y
        );

        buildings.Add(data);
        SpawnBuilding(data);
        RemoveEmptySlot(slot.slotId);
        SaveBuildings();
    }

    public void BuildOnSlot(string slotId, int playerLevel)
    {
        BuildingSlot slot = GetSlotById(slotId);

        if (slot == null)
        {
            Debug.LogWarning("Slot not found: " + slotId);
            return;
        }

        if (IsSlotAlreadyBuilt(slotId))
        {
            Debug.LogWarning("Already built: " + slotId);
            return;
        }

        if (playerLevel < slot.unlockPlayerLevel)
        {
            Debug.LogWarning("Player level is too low. Required Level: " + slot.unlockPlayerLevel);
            return;
        }

        CreateBuildingFromSlot(slot);
    }

    public List<BuildingSlot> GetBuildableSlots(int playerLevel)
    {
        List<BuildingSlot> buildableSlots = new List<BuildingSlot>();

        if (initialLayout == null)
            return buildableSlots;

        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot == null)
                continue;

            bool unlocked = playerLevel >= slot.unlockPlayerLevel;
            bool alreadyBuilt = IsSlotAlreadyBuilt(slot.slotId);

            if (unlocked && !alreadyBuilt)
            {
                buildableSlots.Add(slot);
            }
        }

        return buildableSlots;
    }

    public List<BuildingSlot> GetLockedSlots(int playerLevel)
    {
        List<BuildingSlot> lockedSlots = new List<BuildingSlot>();

        if (initialLayout == null)
            return lockedSlots;

        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot == null)
                continue;

            bool lockedByLevel = playerLevel < slot.unlockPlayerLevel;
            bool alreadyBuilt = IsSlotAlreadyBuilt(slot.slotId);

            if (lockedByLevel && !alreadyBuilt)
            {
                lockedSlots.Add(slot);
            }
        }

        return lockedSlots;
    }

    private bool IsSlotAlreadyBuilt(string slotId)
    {
        return buildings.Exists(b => b.slotId == slotId);
    }

    private BuildingSlot GetSlotById(string slotId)
    {
        if (initialLayout == null)
            return null;

        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot != null && slot.slotId == slotId)
            {
                return slot;
            }
        }

        return null;
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
            " / SlotId: " + data.slotId +
            " / TypeId: " + data.buildingTypeId +
            " / Level: " + data.level
        );
    }

    private void SpawnEmptySlots()
    {
        if (initialLayout == null)
            return;

        if (emptySlotPrefab == null)
            return;


        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot == null)
                continue;

            if (IsSlotAlreadyBuilt(slot.slotId))
                continue;

            if (spawnedEmptySlots.ContainsKey(slot.slotId))
                continue;

            GameObject obj = Instantiate(emptySlotPrefab);
            obj.transform.position = new Vector3(slot.position.x, slot.position.y, 0);

            Sprite slotSprite = GetSlotSprite(slot, currentPlayerLevel);

            BuildingSlotVisual visual = obj.GetComponent<BuildingSlotVisual>();

            if (visual != null)
            {
                visual.Setup(slot.slotId, slotSprite, this);
            }
            else
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();

                if (renderer != null)
                    renderer.sprite = slotSprite;
            }

            spawnedEmptySlots[slot.slotId] = obj;
        }
    }

    private void RemoveEmptySlot(string slotId)
    {
        if (!spawnedEmptySlots.ContainsKey(slotId))
            return;

        Destroy(spawnedEmptySlots[slotId]);
        spawnedEmptySlots.Remove(slotId);
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

    [ContextMenu("Reset Building Save")]
    private void ResetBuildingSave()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();

        Debug.Log("Building save data deleted.");
    }

    private Sprite GetSlotSprite(BuildingSlot slot, int playerLevel)
    {
        if (playerLevel < slot.unlockPlayerLevel)
        {
            return slot.lockedSprite;
        }

        return slot.buildableSprite;
    }


    public void RefreshEmptySlotVisuals()
    {
        if (initialLayout == null)
            return;

        foreach (BuildingSlot slot in initialLayout.buildingSlots)
        {
            if (slot == null)
                continue;

            if (IsSlotAlreadyBuilt(slot.slotId))
                continue;

            if (!spawnedEmptySlots.ContainsKey(slot.slotId))
                continue;

            GameObject obj = spawnedEmptySlots[slot.slotId];
            Sprite slotSprite = GetSlotSprite(slot, currentPlayerLevel);

            BuildingSlotVisual visual = obj.GetComponent<BuildingSlotVisual>();

            if (visual != null)
            {
                visual.SetSprite(slotSprite);
            }
            else
            {
                SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();

                if (renderer != null)
                    renderer.sprite = slotSprite;
            }
        }
    }

    public void SetPlayerLevel(int playerLevel)
    {
        currentPlayerLevel = playerLevel;
        RefreshEmptySlotVisuals();
    }

    public void RequestBuildOnSlot(string slotId)
    {
        Debug.LogWarning("request build on slot" + slotId);
        BuildOnSlot(slotId, currentPlayerLevel);
    }
}