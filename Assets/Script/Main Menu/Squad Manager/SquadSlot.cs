using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadSlot : MonoBehaviour
{
    [Header("UI References")]
    public Transform previewParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;  
    public TextMeshProUGUI levelText;       
    public TextMeshProUGUI availabilityText;
    public Button button;
    public GameObject plusIcon;

    private OwnedCharacterData assignedCharacter;
    private GameObject previewInstance;

    private void Start()
    {
        button.onClick.AddListener(() => SquadManager.Instance.OpenCharacterSelection(this));
    }

    public void AssignCharacter(OwnedCharacterData character)
    {
        assignedCharacter = character;

        // Bersihkan preview sebelumnya
        foreach (Transform child in previewParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // Load dan spawn preview modular
        GameObject previewPrefab = Resources.Load<GameObject>("ModularPreview");
        if (previewPrefab != null)
        {
            previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(-0.05f, -1.5f, 0f);
            previewInstance.transform.localScale = new Vector3(1.5f, 1f, 1f);
            previewInstance.transform.localRotation = Quaternion.identity;

            var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(character);

                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                {
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }

            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
            HideUnusedPreviewParts(previewInstance);
        }
        else
        {
            Debug.LogWarning("ModularPreview prefab not found in Resources folder!");
        }

        nameText.text = character.characterName;
        classText.text = $"{character.characterClass}";
        levelText.text = $"Lvl: {character.level}";
        availabilityText.text = $"Avail: {character.availability}/{character.maxAvailability}";

        if (plusIcon != null) plusIcon.SetActive(false);

        MapManager.Instance.UpdatePlayerStatsUI();
    }

    public void ClearSlot()
    {
        assignedCharacter = null;
        nameText.text = "";
        classText.text = "";
        levelText.text = "";
        availabilityText.text = "";

        foreach (Transform child in previewParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        if (plusIcon != null) plusIcon.SetActive(true);
    }

    public bool IsEmpty()
    {
        return assignedCharacter == null;
    }

    public OwnedCharacterData AssignedCharacter => assignedCharacter;

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
