using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public GameObject gameOverPanel;
    public Image resultImage;
    public Sprite winSprite;
    public Sprite loseSprite;
    public TextMeshProUGUI goldRewardText;
    public Button returnToMapButton;
    public GameObject goldRewardGroup;

    private bool gameOverTriggered = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameOverPanel.SetActive(false);
        returnToMapButton.onClick.AddListener(ReturnToMap);
    }

    public void TriggerWin()
    {
        if (gameOverTriggered) return;

        Time.timeScale = 0f;
        gameOverTriggered = true;
        SquadTransferData.playerWon = true;
        resultImage.sprite = winSprite;
        int reward = SquadTransferData.pendingGoldReward;
        goldRewardText.text = $"{reward}";
        gameOverPanel.SetActive(true);
        goldRewardGroup.SetActive(true);
        ProcessBattleResult(true);
    }

    public void TriggerLose()
    {
        if (gameOverTriggered) return;

        Time.timeScale = 0f;
        gameOverTriggered = true;
        SquadTransferData.playerWon = false;
        resultImage.sprite = loseSprite;
        SquadTransferData.pendingGoldReward = 0;
        int reward = SquadTransferData.pendingGoldReward;
        goldRewardText.text = $"{reward}";
        gameOverPanel.SetActive(true);
        goldRewardGroup.SetActive(false);
        ProcessBattleResult(false);
    }

    private void ProcessBattleResult(bool won)
    {
        int baseExpReward = SquadTransferData.pendingExpReward;
        int expReward = won ? baseExpReward : Mathf.RoundToInt(baseExpReward * 0.25f);

        foreach (var unit in UnitStats.allUnits)
        {
            if (unit.teamID == 0 && unit.LinkedCharacterData != null && !unit.LinkedCharacterData.isDead)
            {
                var data = unit.LinkedCharacterData;

                // Tambah EXP
                data.experience += expReward;

                // Cek level up
                while (data.experience >= data.expToNextLevel)
                {
                    data.experience -= data.expToNextLevel;
                    data.level++;
                    data.attack += 1;
                    data.hp += 3;

                    // Logika threshold baru: 
                    // 1-10 nambah 50, 11 keatas nambah 500
                    if (data.level <= 10)
                    {
                        data.expToNextLevel += 50;
                    }
                    else
                    {
                        data.expToNextLevel += 500;
                    }
                }
            }
        }
        // Simpan langsung hasilnya ke JSON
        SquadSaveManager.SaveSquad(SquadTransferData.activeSquad);
    }

    void ReturnToMap()
    {
        SquadTransferData.justFinishedBattle = true;
        SquadTransferData.gameOverCheckEnabled = true;
        
        foreach (var unit in UnitStats.allUnits)
        {
            if (unit.teamID == 0)
            {
                unit.ApplyAvailabilityPenalty();
            }
        }

        SquadSaveManager.SaveSquad(SquadTransferData.activeSquad);

        Time.timeScale = 1f;
        gameOverTriggered = false;
        SceneManager.LoadScene(1);
    }
}
