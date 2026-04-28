using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform previewParent; // 🔄 Tempat spawn prefab karakter
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI traitsText;
    public TextMeshProUGUI statsText;
    public Button closeButton;
    public GameObject scrollView;
    public GameObject panel;

    private GameObject previewInstance; // ✅ variabel ini yang hilang di versi kamu

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    public void Show(OwnedCharacterData data)
    {
        if (data == null)
        {
            Debug.LogError("❌ Tried to show character detail but data is NULL!");
            return;
        }

        // 🧹 Bersihkan preview sebelumnya
        foreach (Transform child in previewParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // 🛠 Spawn prefab modular preview
        GameObject previewPrefab = Resources.Load<GameObject>("ModularPreview");
        if (previewPrefab != null)
        {
            previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(0f, -1.26f, 0f);
            previewInstance.transform.localScale = new Vector3(1f, 0.85f, 0.85f);
            previewInstance.transform.localRotation = Quaternion.identity;

            // Apply data ke builder
             var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(data);

                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            HideUnusedPreviewParts(previewInstance);
            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
        }
        else
        {
            Debug.LogWarning("❌ ModularPreview prefab not found in Resources!");
        }

        // 📝 Isi detail teks
        nameText.text = "Name : " + data.characterName;
        classText.text = data.characterClass;
        levelText.text = "Level : " + data.level;
        traitsText.text = "Traits: " + data.traits;
        statsText.text = $"ATK: {data.attack}                  HP: {data.hp}\n" +
                         $"FireRate: {data.fireRate:F1}       Range: {data.range}          SPD: {data.spd}";

        // 🔄 Toggle panel
        if (scrollView != null) scrollView.SetActive(false);
        if (panel != null) panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        if (scrollView != null) scrollView.SetActive(true);
    }

        private void HideUnusedPreviewParts(GameObject previewInstance)
    {
        string[] unwantedNames = new string[]
        {
            "LowerBodyBone", "WeaponBone", "FirePoint", "Selected", "Shadow", "HitPoint", "Canvas",
        };

        Transform[] allChildren = previewInstance.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (System.Array.Exists(unwantedNames, name => child.name == name))
                child.gameObject.SetActive(false);
        }
    }
}
