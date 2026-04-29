using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("References")]
    
    public GameObject mapExplanationPanel;
    public GameObject mapExplanationPanel2;
    public GameObject mapExplanationPanel3;
    public GameObject mapExplanationPanel4;
    public GameObject mapExplanationPanel5;
    public GameObject mapExplanationPanel6;
    public GameObject mapExplanationPanel7;
    public GameObject mapExplanationPanel8;
    public GameObject mapExplanationPanel9;
    public GameObject mapExplanationPanel10;
    public GameObject mapExplanationPanel11;
    public AreaNode area13;
    public AreaNode area0;
    public AreaNode area1;
    public AreaNode area2;
    public Canvas area13Canvas;
    public Canvas area0Canvas;
    public Canvas area1Canvas;
    public Canvas area2Canvas;
    public Button closeButton;
    public Button closeButton3;
    public Button closeButton35;
    public Button closeButton5;
    public Button closeButton6;
    public Button closeButton7;
    public Button closeButton8;
    public Button closeButton10;
    public Button closeButton11;


    private bool originalUnlocked13;
    private bool originalUnlocked1;
    private bool originalUnlocked2;
    private bool hasClosedMapExplanation4 = false;
    private bool hasClosedMapExplanation9 = false;

    public static Tutorial Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Simpan kondisi awal
        originalUnlocked13 = area13.isUnlocked;
        originalUnlocked1 = area1.isUnlocked;
        originalUnlocked2 = area2.isUnlocked;

        // Tampilkan panel
        mapExplanationPanel.SetActive(true);

        // Atur area 13 → unlock + sorting order lebih tinggi
        area13.Unlock();
        area13Canvas.sortingOrder = 2;

        // Lock area 1 & 2
        area1.isUnlocked = false;
        area1.UpdateVisual();

        area2.isUnlocked = false;
        area2.UpdateVisual();

        // Event tombol close
        closeButton.onClick.AddListener(CloseMapExplanation);
        closeButton3.onClick.AddListener(CloseMapExplanation3);
        closeButton5.onClick.AddListener(CloseMapExplanation5);
        closeButton6.onClick.AddListener(CloseMapExplanation6);
        closeButton7.onClick.AddListener(CloseMapExplanation7);
        closeButton8.onClick.AddListener(CloseMapExplanation8);
        closeButton10.onClick.AddListener(CloseMapExplanation10);
        closeButton11.onClick.AddListener(CloseMapExplanation11);
    }

    private void CloseMapExplanation()
    {
        mapExplanationPanel.SetActive(false);

        area13.isUnlocked = originalUnlocked13;
        area13Canvas.sortingOrder = 0;
        area13.UpdateVisual();

        area0Canvas.sortingOrder = 1;
        area0.UpdateVisual();

        area1.isUnlocked = originalUnlocked1;
        area1Canvas.sortingOrder = 1;
        area1.UpdateVisual(); 

        area2.isUnlocked = originalUnlocked2;
        area2Canvas.sortingOrder = 1;
        area2.UpdateVisual();

        mapExplanationPanel2.SetActive(true);
    }

        private void Update()
    {
        // Jika confirmation panel aktif → panggil CloseMapExplanation2
        if (MapManager.Instance != null && MapManager.Instance.confirmationPanel.activeSelf)
        {
            CloseMapExplanation2();
        }

        // Jika Tab shop (0) aktif → panggil CloseMapExplanation4 sekali saja
        if (TabManager.instance != null && TabManager.instance.tabs.Length > 0)
        {
            if (TabManager.instance.tabs[0].activeSelf && !hasClosedMapExplanation4)
            {
                CloseMapExplanation4();
                hasClosedMapExplanation4 = true;
            }
        }
    }

    private void CloseMapExplanation2()
    {
        mapExplanationPanel2.SetActive(false);

        area0Canvas.sortingOrder = 0;
        area0.UpdateVisual();

        area1.isUnlocked = originalUnlocked1;
        area1Canvas.sortingOrder = 0;
        area1.UpdateVisual();

        area2.isUnlocked = originalUnlocked2;
        area2Canvas.sortingOrder = 0;
        area2.UpdateVisual();

        mapExplanationPanel3.SetActive(true);
    }

    private void CloseMapExplanation3()
    {
        mapExplanationPanel3.SetActive(false);

        mapExplanationPanel4.SetActive(true);
    }

    private void CloseMapExplanation4()
    {
        mapExplanationPanel4.SetActive(false);

        mapExplanationPanel5.SetActive(true);
    }

    private void CloseMapExplanation5()
    {
        mapExplanationPanel5.SetActive(false);

        mapExplanationPanel6.SetActive(true);
    }

    private void CloseMapExplanation6()
    {
        mapExplanationPanel6.SetActive(false);

        mapExplanationPanel7.SetActive(true);
    }

    private void CloseMapExplanation7()
    {
        mapExplanationPanel7.SetActive(false);

        mapExplanationPanel8.SetActive(true);
    }

    private void CloseMapExplanation8()
    {
        mapExplanationPanel8.SetActive(false);

        mapExplanationPanel9.SetActive(true);
    }

    //ter trigger di shop manager saat sudah purchase 4 character
    public void TriggerCloseMapExplanation9()
    {
        if (!hasClosedMapExplanation9)
        {
            hasClosedMapExplanation9 = true;
            CloseMapExplanation9();
        }
    }

    private void CloseMapExplanation9()
    {
        mapExplanationPanel9.SetActive(false);

        mapExplanationPanel10.SetActive(true);
    }

    private void CloseMapExplanation10()
    {
        mapExplanationPanel10.SetActive(false);

        mapExplanationPanel11.SetActive(true);
    }

    private void CloseMapExplanation11()
    {
        mapExplanationPanel11.SetActive(false);
    }
}
