using UnityEngine;
using TMPro;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    public int gold = 0;
    public TextMeshProUGUI goldText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gold = PlayerPrefs.GetInt("gold", 1000);
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        PlayerPrefs.SetInt("gold", gold);
        UpdateUI();
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            PlayerPrefs.SetInt("gold", gold);
            UpdateUI();
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {gold}";
    }

    public int GetGold()
    {
        return gold;
    }
}
