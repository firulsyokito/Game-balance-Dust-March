using UnityEngine;

public class TabManager : MonoBehaviour
{
    public static TabManager instance; // 🔹 static instance

    public GameObject[] tabs;
    public GameObject scrollView;

    void Awake()
    {
        // Pastikan hanya ada satu instance
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (var tab in tabs)
        {
            if (tab != null)
                tab.SetActive(false);
        }

        // Aktifkan Tab Map (index 1) di awal
        tabs[1].SetActive(true);
    }

    public void OpenTab(int index)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == index);
        }

        scrollView.SetActive(index == 2);
    }
}
