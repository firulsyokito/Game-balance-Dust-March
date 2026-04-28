using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmationManager : MonoBehaviour
{
    public static ConfirmationManager Instance;

    [Header("Exit Panel")]
    public GameObject exitPanel;
    public Button exitYesButton;
    public Button exitNoButton;
    public Button openExitButton;

    [Header("Reset Panel")]
    public GameObject resetPanel;
    public Button resetOkButton;
    public Button resetExitButton;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Button gameOverExitButton;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (openExitButton != null) openExitButton.onClick.AddListener(ShowExitPanel);
        
        if (exitPanel != null) exitPanel.SetActive(false);
        if (resetPanel != null) resetPanel.SetActive(false);

        if (exitYesButton != null) exitYesButton.onClick.AddListener(HandleExitYes);
        if (exitNoButton != null) exitNoButton.onClick.AddListener(() => exitPanel.SetActive(false));

        if (resetOkButton != null) resetOkButton.onClick.AddListener(HandleResetOk);
        if (resetExitButton != null) resetExitButton.onClick.AddListener(HandleResetExit);

        if (gameOverExitButton != null) gameOverExitButton.onClick.AddListener(HandleNewGameExit);
    }

    public void ShowExitPanel()
    {
        if (exitPanel != null) exitPanel.SetActive(true);
    }

    public void ShowResetPanel()
    {
        if (resetPanel != null) resetPanel.SetActive(true);
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    void HandleExitYes()
    {
        if (MapManager.Instance != null)
        {
            // 💾 Simpan map sebelum keluar
            SaveManager.SaveMap(MapManager.Instance.CollectCurrentMapData());
        }
        SceneManager.LoadScene(0);
    }

    void HandleResetOk()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.ResetMapProgress();
            SaveManager.SaveMap(MapManager.Instance.CollectCurrentMapData());
        }
        if (resetPanel != null) resetPanel.SetActive(false);
    }

    void HandleResetExit()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.ResetMapProgress();
            SaveManager.SaveMap(MapManager.Instance.CollectCurrentMapData());
        }
        SceneManager.LoadScene(0);
    }
    
    void HandleNewGameExit()
    {
        // Hapus semua file save
        SaveManager.ResetAllSaves();
        Debug.Log("Save data has been reset. Starting new game.");

        // Kembali ke menu utama
        SceneManager.LoadScene(0);
    }
}
