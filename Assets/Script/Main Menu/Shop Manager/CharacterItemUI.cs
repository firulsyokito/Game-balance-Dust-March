using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nameText, classText, statsText, traitsText, costText;
    public Button buyButton;
    public Transform previewParent;

    [Header("Placement After Buy")]
    public Transform prefabHolder;

    private OwnedCharacterData data;
    private int characterCost;
    private ShopManager shopManager;

    private GameObject previewInstance;
    private GameObject previewPrefab;

    public void Setup(OwnedCharacterData characterData, int cost, ShopManager manager, GameObject prefabFromManager)
    {
        data = characterData;
        characterCost = cost;
        shopManager = manager;

        nameText.text = data.characterName;
        classText.text = data.characterClass;
        traitsText.text = data.traits;
        costText.text = $"Cost: {characterCost}";
        statsText.text = $"ATK: {data.attack} / HP: {data.hp}";

        previewPrefab = prefabFromManager != null ? prefabFromManager : Resources.Load<GameObject>("ModularPreview");

        foreach (Transform child in previewParent)
        {
            if (child.gameObject.layer == LayerMask.NameToLayer("Unit"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }

        // Tampilkan preview baru
        if (previewPrefab != null && previewParent != null && previewParent.gameObject.scene.IsValid())
        {
            previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(-0.06f, -2.2f, 0f);
            //previewInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);


            var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(data);

                // Opsional: Mask interaction
                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                {
                    sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
            }

            HideUnusedPreviewParts(previewInstance);
            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
        }

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(BuyCharacter);
    }

    void BuyCharacter()
    {
        if (EconomyManager.Instance == null || !EconomyManager.Instance.SpendGold(characterCost))
        {
            Debug.LogWarning("Gold tidak cukup.");
            return;
        }

        shopManager.PurchaseCharacter(data); // Sudah otomatis AddCharacter + Save
        MapManager.Instance.UpdatePlayerStatsUI();

        // Pindahkan previewInstance ke prefabHolder
        if (previewInstance != null && prefabHolder != null)
        {
            previewInstance.transform.SetParent(prefabHolder, false);
            previewInstance.transform.localPosition = Vector3.zero;
            previewInstance.transform.localScale = Vector3.one;
        }

        Debug.Log($"✅ {data.characterName} dibeli, preview dipindah ke holder, dan data disimpan ke INVENTORY.");

        Destroy(gameObject); // Hapus UI item-nya
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

}
