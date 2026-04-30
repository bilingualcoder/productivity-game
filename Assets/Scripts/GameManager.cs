using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public PlayerData playerData;

    public TMP_Text levelText;
    public TMP_Text xpText;

    private void Start()
    {
        UpdateUI();
    }

    public void AddXPToPlayer(int amount)
    {
        playerData.AddXP(amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        levelText.text = "Level: " + playerData.level;
        xpText.text = "XP: " + playerData.currentXP + " / " + playerData.xpToNextLevel;
    }
}