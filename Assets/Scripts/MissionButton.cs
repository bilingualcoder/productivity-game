using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionButton : MonoBehaviour
{
    public GameManager gameManager;
    public int rewardXP = 20;

    private Button button;
    private TMP_Text buttonText;
    private bool isCompleted = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void CompleteMission()
    {
        if (isCompleted) return;

        isCompleted = true;
        gameManager.AddXPToPlayer(rewardXP);

        button.interactable = false;
        buttonText.text = "Complete";
    }
}