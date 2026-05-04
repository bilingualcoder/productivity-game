using UnityEngine;

[CreateAssetMenu(fileName = "BuildingTypeData", menuName = "Game/Building Type Data")]
public class BuildingTypeData : ScriptableObject
{
    public int buildingTypeId;
    public string buildingName;

    public Sprite[] levelSprites;

    public int maxLevel = 3;
    public int baseXPToNextLevel = 100;
    public int xpIncreasePerLevel = 50;

    public Sprite GetSpriteForLevel(int level)
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return null;

        int index = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
        return levelSprites[index];
    }

    public int GetXPToNextLevel(int level)
    {
        return baseXPToNextLevel + ((level - 1) * xpIncreasePerLevel);
    }
}