using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerData playerData;

    public TMP_Text levelText;
    public TMP_Text xpText;

    private const string XP_KEY = "PLAYER_XP";
    private const string LEVEL_KEY = "PLAYER_LEVEL";

    private void Start()
    {
        LoadPlayerData();
        UpdateUI();
    }

    public void AddXPToPlayer(int amount)
    {
        playerData.AddXP(amount);
        SavePlayerData();
        UpdateUI();
    }

    private void UpdateUI()
    {
        levelText.text = "Level: " + playerData.level;
        xpText.text = "XP: " + playerData.currentXP + " / " + playerData.xpToNextLevel;
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt(XP_KEY, playerData.currentXP);
        PlayerPrefs.SetInt(LEVEL_KEY, playerData.level);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        playerData.currentXP = PlayerPrefs.GetInt(XP_KEY, 0);
        playerData.level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }
}