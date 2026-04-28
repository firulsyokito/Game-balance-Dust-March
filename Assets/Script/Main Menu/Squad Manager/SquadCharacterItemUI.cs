using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SquadCharacterItemUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform previewParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI availabilityText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI traitsText;
    public Button pickButton;
    public TextMeshProUGUI pickButtonText;

    private OwnedCharacterData characterData;
    private System.Action<OwnedCharacterData> callback;
    private GameObject previewInstance;

    public void Setup(OwnedCharacterData data, System.Action<OwnedCharacterData> onSelect, bool disabled)
    {
        characterData = data;
        callback = onSelect;

        // Bersihkan preview sebelumnya
        foreach (Transform child in previewParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Buat preview karakter
        GameObject previewPrefab = Resources.Load<GameObject>("ModularPreview");
        if (previewPrefab != null)
        {
            previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(0f, -2f, 0f);
            previewInstance.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            previewInstance.transform.localRotation = Quaternion.identity;

            var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(data);
                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                    sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }

            HideUnusedPreviewParts(previewInstance);
            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
        }

        // Set UI Text
        nameText.text = data.characterName;
        classText.text = data.characterClass;
        levelText.text = "Lvl " + data.level;
        statsText.text = $"ATK: {data.attack} / HP: {data.hp}";
        traitsText.text = data.traits;

        // Atur warna availability
        if (data.availability < 0)
            availabilityText.color = Color.red; // Wounded
        else if (data.availability == 0)
            availabilityText.color = new Color(1f, 0.5f, 0f); // Orange untuk Tired
        else
            availabilityText.color = Color.white; // Normal

        availabilityText.text = $"Avail: {data.availability}/{data.maxAvailability}";

        pickButton.onClick.RemoveAllListeners();

        bool isInSquad = SquadManager.Instance.IsCharacterInSquad(data);

        // Kondisi tombol
        if (data.availability < 0) // wounded
        {
            pickButtonText.text = "Wounded";
            pickButton.interactable = false;
        }
        else if (data.availability == 0) // tired
        {
            pickButtonText.text = "Tired";
            pickButton.interactable = false;
        }
        else if (isInSquad)
        {
            pickButtonText.text = "Deselect";
            pickButton.interactable = true;
            pickButton.onClick.AddListener(() =>
            {
                SquadManager.Instance.RemoveFromSquad(data);
                SquadManager.Instance.RemoveUsedCharacter(data);
                SquadManager.Instance.ShowSquadPanel();
            });
        }
        else
        {
            pickButtonText.text = "Select";
            pickButton.interactable = !disabled;
            pickButton.onClick.AddListener(() => callback.Invoke(characterData));
        }
    }

    private void HideUnusedPreviewParts(GameObject previewInstance)
    {
        string[] unwantedNames = {
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
