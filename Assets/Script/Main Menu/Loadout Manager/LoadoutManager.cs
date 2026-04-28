using UnityEngine;
using TMPro;

public class LoadoutManager : MonoBehaviour
{
    public GameObject characterItemPrefab;
    public Transform contentParent;
    public CharacterDetailUI detailPanel;

    [Header("UI - Empty Message")]
    public TextMeshProUGUI emptyText;

    [Header("UI Buttons")]
    public UnityEngine.UI.Button backToMapButton;

    void OnEnable()
    {
        // 🔹 Bersihkan isi lama
        foreach (Transform child in contentParent)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        bool hasCharacter = false;

        // 🔹 Tambahkan item loadout dari inventory
        foreach (var character in InventoryManager.Instance.ownedCharacters)
        {
            if (character == null)
            {
                Debug.LogWarning("LoadoutManager: Null OwnedCharacter in inventory list.");
                continue;
            }

            hasCharacter = true;

            GameObject itemObj = Instantiate(characterItemPrefab, contentParent);
            var ui = itemObj.GetComponent<LoadoutCharacterItemUI>();

            if (ui == null)
            {
                Debug.LogError("LoadoutManager: Prefab missing LoadoutCharacterItemUI component.");
                continue;
            }

            ui.Setup(character, detailPanel);
        }

        // 🔹 Tampilkan atau sembunyikan teks kosong
        if (emptyText != null)
            emptyText.gameObject.SetActive(!hasCharacter);

        // 🔹 Pastikan detail panel tersembunyi saat awal
        detailPanel.panel.SetActive(false);

        // 🔹 Setup tombol kembali ke tab Map
        if (backToMapButton != null)
        {
            backToMapButton.onClick.RemoveAllListeners();
            backToMapButton.onClick.AddListener(() =>
            {
                TabManager.instance?.OpenTab(1); // 1 = Tab Map
            });
        }
    }
}
