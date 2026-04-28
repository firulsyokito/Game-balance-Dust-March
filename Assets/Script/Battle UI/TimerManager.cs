using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TMP_Text timerText;

    private bool isGameOver = false;

    void Start()
    {
        UpdateTimerUI();
    }

    void Update()
    {
        if (isGameOver) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI();
        }
        else
        {
            TriggerGameOver();
        }
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = seconds.ToString();
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        timeRemaining = 0;
        UpdateTimerUI();

        GameOverManager.Instance?.TriggerLose();
    } 

}
