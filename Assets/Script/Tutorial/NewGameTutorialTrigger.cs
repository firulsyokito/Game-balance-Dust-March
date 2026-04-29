using UnityEngine;

public class NewGameTutorialTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tutorialGameObject;

    void Start()
    {
        if (tutorialGameObject == null)
        {
            Debug.LogWarning("Tutorial GameObject is not assigned in the inspector!");
            return;
        }

        // Cek apakah ini New Game
        if (PlayerPrefs.GetInt("IsNewGame", 0) == 1)
        {
            tutorialGameObject.SetActive(true);
            
            // Reset agar tidak aktif lagi di sesi berikutnya
            PlayerPrefs.SetInt("IsNewGame", 0);
            PlayerPrefs.Save();
        }
        else
        {
            tutorialGameObject.SetActive(false);
        }
    }
}
