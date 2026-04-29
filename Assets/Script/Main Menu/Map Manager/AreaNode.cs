using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class AreaNode : MonoBehaviour
{
    [Header("Area Settings")]
    public int areaID;
    public bool isUnlocked = false;
    public bool isCurrentArea = false;
    public bool hasBeenCurrent = false;
    public AreaNode[] connectedAreas;

    [Header("Area Info")]
    public string missionName;
    public DifficultyLevel difficulty;
    public int enemyPerWave;
    public Sprite enemySprite;
    public int goldReward;
    public int expReward;

    [Header("UI References")]
    public Button button;
    public Image image;

    [Header("UI Display")]
    public TextMeshProUGUI missionNameText;
    public Image[] difficultyStars;

    [Header("Manager Reference")]
    public MapManager mapManager;

    public enum DifficultyLevel
    {
        Tutorial,
        Easy,
        Medium,
        Hard,
        Final
    }

    public void GenerateStatsFromDifficulty()
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                enemyPerWave = 2;
                goldReward = Random.Range(200, 400);
                expReward = 100;
                break;
            case DifficultyLevel.Medium:
                enemyPerWave = Random.Range(4, 5);
                goldReward = Random.Range(450, 650);
                expReward = 200;
                break;
            case DifficultyLevel.Hard:
                enemyPerWave = Random.Range(6, 7);
                goldReward = Random.Range(650, 850);
                expReward = 400;
                break;
            case DifficultyLevel.Final:
                enemyPerWave = 8;
                goldReward = 1000;
                expReward = 800;
                break;
        }
    }

    void Start()
    {
        button.onClick.AddListener(OnAreaClicked);
        UpdateVisual();
    }

    public void UpdateMapLabels()
    {
        if (missionNameText != null)
            missionNameText.text = missionName;

        if (difficultyStars != null && difficultyStars.Length > 0)
        {
            int level = 0;
            switch (difficulty)
            {
                case DifficultyLevel.Easy: level = 1; break;
                case DifficultyLevel.Medium: level = 2; break;
                case DifficultyLevel.Hard: level = 3; break;
            }

            for (int i = 0; i < difficultyStars.Length; i++)
            {
                difficultyStars[i].enabled = i < level;
            }
        }
    }

    public void Unlock()
    {
        if (hasBeenCurrent) return;

        isUnlocked = true;
        UpdateVisual();
    }

    public void SetAsCurrent()
    {
        mapManager.ClearAllCurrentAreas();
        isCurrentArea = true;
        hasBeenCurrent = true;
        UpdateVisual();
    }


    public void UpdateVisual()
    {
        button.interactable = isUnlocked;
        image.color = isCurrentArea ? Color.green : hasBeenCurrent ? Color.black : isUnlocked ? Color.white : Color.gray;
    }

    void OnAreaClicked()
    {
        if (isUnlocked && !isCurrentArea)
        {
            mapManager.ShowConfirmation(this);
        }
    }
}
