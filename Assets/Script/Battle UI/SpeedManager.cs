using UnityEngine;
using UnityEngine.UI;

public class SpeedManager : MonoBehaviour
{
    public Button toggleButton;
    public Image buttonImage;
    public Sprite normalSprite;
    public Sprite fastSprite;
    public Sprite slowSprite;

    private float[] speedLevels = { 1f, 2f, 0.5f };
    private int currentIndex = 0;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleSpeed);
        ApplySpeed();
    }

    void ToggleSpeed()
    {
        currentIndex = (currentIndex + 1) % speedLevels.Length;
        ApplySpeed();
    }

    void ApplySpeed()
    {
        Time.timeScale = speedLevels[currentIndex];
        
        switch (currentIndex)
        {
            case 0:
                buttonImage.sprite = normalSprite;
                break;
            case 1:
                buttonImage.sprite = fastSprite;
                break;
            case 2:
                buttonImage.sprite = slowSprite;
                break;
        }
    }
}
