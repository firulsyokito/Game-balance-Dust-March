using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button pauseButton;
    public Button resumeButton;
    public Button settingButton;
    public Button quitButton;

    public Image pauseButtonImage;
    public Sprite pauseIcon;  
    public Sprite playIcon;  

    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(Resume);
        settingButton.onClick.AddListener(Setting);
        quitButton.onClick.AddListener(QuitGame);

        UpdatePauseButtonIcon();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        UpdatePauseButtonIcon();
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        UpdatePauseButtonIcon();
    }

    public void Setting()
    {
        GameOverManager.Instance.TriggerWin();
    }

    public void QuitGame()
    {
        GameOverManager.Instance.TriggerLose();
    }

    private void UpdatePauseButtonIcon()
    {
        if (pauseButtonImage != null)
        {
            pauseButtonImage.sprite = isPaused ? playIcon : pauseIcon;
        }
    }
}
