using UnityEngine;

public class ResetSaveData : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("All PlayerPrefs deleted.");
    }
}