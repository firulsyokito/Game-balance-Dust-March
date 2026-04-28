using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutCharacterItemUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI availableText;
    public Button button;
    public GameObject highlightBorder;
    public Transform previewParent;

    private OwnedCharacterData characterData;
    private GameObject previewInstance;

    public void Setup(OwnedCharacterData data, CharacterDetailUI detailPanel)
    {
        characterData = data;

        foreach (Transform child in previewParent)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        // 🎭 Load prefab modular preview
        GameObject previewPrefab = Resources.Load<GameObject>("ModularPreview");
        if (previewPrefab != null)
        {
            previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(0.01f, -1.4f, 0f);
            previewInstance.transform.localScale = new Vector3(1.5f, 1f, 1f);
            previewInstance.transform.localRotation = Quaternion.identity;

            // 🛠 Apply data ke CharacterBuilder
            var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(data);

                // Mask Interaction
                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                    sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }

            // 🚫 Hide parts yang tidak perlu
            HideUnusedPreviewParts(previewInstance);

            // 🚫 Nonaktifkan komponen gameplay + collider supaya klik UI tidak ketahan
            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
            DisableAllColliders(previewInstance);
        }
        else
        {
            Debug.LogWarning("⚠ ModularPreview prefab not found in Resources!");
        }

        // 📝 Update teks UI
        nameText.text = data.characterName;
        classText.text = data.characterClass;
        levelText.text = $"{data.level}";
        availableText.text = $"Avail: {data.availability}/{data.maxAvailability}";

    // Ubah warna sesuai kondisi
    if (data.availability > 0)
    {
        availableText.color = Color.white; // avail > 0 → putih
    }
    else if (data.availability == 0)
    {
        availableText.color = Color.yellow; // avail = 0 → kuning
    }
    else // avail < 0
    {
        availableText.color = Color.red; // avail < 0 → merah
    }

        // 🖱 Setup tombol (hapus listener lama dulu)
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (detailPanel != null)
            {
                Debug.Log($"📜 Showing detail for {characterData.characterName}");
                detailPanel.Show(characterData);
            }
            else
            {
                Debug.LogError("❌ Detail Panel is NULL! Assign it when calling Setup().");
            }
        });
    }

    public void SetHighlight(bool on)
    {
        if (highlightBorder != null)
            highlightBorder.SetActive(on);
    }

    private void HideUnusedPreviewParts(GameObject previewInstance)
    {
        string[] unwantedNames = new string[]
        {
            "LowerBodyBone", "WeaponBone", "FirePoint", "Selected", "Shadow", "HitPoint", "Canvas",
            "Right Thigh", "Right Foot", "Right Feet",
            "Left Thigh", "Left Foot", "Left Feet"
        };

        Transform[] allChildren = previewInstance.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (System.Array.Exists(unwantedNames, name => child.name == name))
                child.gameObject.SetActive(false);
        }
    }

    private void DisableAllColliders(GameObject obj)
    {
        foreach (var col in obj.GetComponentsInChildren<Collider>())
            col.enabled = false;
        foreach (var col2D in obj.GetComponentsInChildren<Collider2D>())
            col2D.enabled = false;
    }
}
