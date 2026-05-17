using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Player Data")]
    public PlayerData playerData;

    [Header("UI")]
    public TMP_Text levelText;
    public TMP_Text xpText;

    [Header("Managers")]
    public BuildingManager buildingManager;

    private const string XP_KEY = "PLAYER_XP";
    private const string LEVEL_KEY = "PLAYER_LEVEL";

    private void Start()
    {
        LoadPlayerData();
        UpdateUI();

        if (buildingManager != null)
        {
            buildingManager.SetPlayerLevel(playerData.level);
        }
    }

    public void AddXPToPlayer(int amount)
    {
        int oldLevel = playerData.level;

        playerData.AddXP(amount);

        bool leveledUp = playerData.level > oldLevel;

        SavePlayerData();
        UpdateUI();

        if (buildingManager != null)
        {
            buildingManager.SetPlayerLevel(playerData.level);
        }

        if (leveledUp)
        {
            Debug.Log("Player leveled up to Lv." + playerData.level);
        }
    }

    public int GetPlayerLevel()
    {
        if (playerData == null)
            return 1;

        return playerData.level;
    }

    private void UpdateUI()
    {
        if (playerData == null)
            return;

        if (levelText != null)
        {
            levelText.text = "Level: " + playerData.level;
        }

        if (xpText != null)
        {
            xpText.text = "XP: " + playerData.currentXP + " / " + playerData.xpToNextLevel;
        }
    }

    private void SavePlayerData()
    {
        if (playerData == null)
            return;

        PlayerPrefs.SetInt(XP_KEY, playerData.currentXP);
        PlayerPrefs.SetInt(LEVEL_KEY, playerData.level);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        if (playerData == null)
        {
            Debug.LogWarning("PlayerData is not assigned.");
            return;
        }

        playerData.currentXP = PlayerPrefs.GetInt(XP_KEY, 0);
        playerData.level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }
}