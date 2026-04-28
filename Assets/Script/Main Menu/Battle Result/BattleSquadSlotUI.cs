using UnityEngine;
using TMPro;

public class BattleSquadSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform previewParent;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI classText;
    public TextMeshProUGUI availabilityText;
    public TextMeshProUGUI statusText; // Wounded / Die
    public TextMeshProUGUI levelText; 
    public TextMeshProUGUI ExpText; 

    [Header("Status Frames")]
    public GameObject woundedFrame;
    public GameObject diedFrame;

    public void AssignCharacter(OwnedCharacterData character, string status)
    {
        // Bersihkan preview sebelumnya
        foreach (Transform child in previewParent)
            Destroy(child.gameObject);

        // Load modular preview seperti SquadSlot
        GameObject previewPrefab = Resources.Load<GameObject>("ModularPreview");
        if (previewPrefab != null)
        {
            var previewInstance = Instantiate(previewPrefab, previewParent);
            previewInstance.transform.localPosition = new Vector3(-0.05f, -1.5f, 0f);
            previewInstance.transform.localScale = new Vector3(1.5f, 1f, 1f);
            previewInstance.transform.localRotation = Quaternion.identity;

            var builder = previewInstance.GetComponent<CharacterBuilder>();
            if (builder != null)
            {
                builder.ApplyCharacterData(character);

                var spriteRenderers = previewInstance.GetComponentsInChildren<SpriteRenderer>();
                foreach (var sr in spriteRenderers)
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            CharacterPreviewUtil.DisableGameplayComponents(previewInstance);
            HideUnusedPreviewParts(previewInstance);
        }

        // Set nama & class
        nameText.text = character.characterName;
        classText.text = character.characterClass;

        // Avail/maxAvail + warna
        availabilityText.text = $"Avail: {character.availability}/{character.maxAvailability}";
        if (character.availability > 0)
            availabilityText.color = Color.white;
        else if (character.availability == 0)
            availabilityText.color = Color.yellow;
        else
            availabilityText.color = Color.red;

        // Status frame reset
        if (woundedFrame != null) woundedFrame.SetActive(false);
        if (diedFrame != null) diedFrame.SetActive(false);

        // Set status wounded / die / ready + aktifkan frame
        if (!string.IsNullOrEmpty(status))
        {
            if (status == "Die")
            {
                statusText.color = Color.red;
                if (diedFrame != null) diedFrame.SetActive(true);
            }
            else if (status == "Wounded")
            {
                statusText.color = new Color(1f, 0.65f, 0f); // oranye
                if (woundedFrame != null) woundedFrame.SetActive(true);
            }
            statusText.text = status;
        }
        else
        {
            statusText.text = "Ready";
            statusText.color = Color.green;
        }

        // Tampilkan LVL & EXP
        levelText.text = $"Lv {character.level}";
        ExpText.text = $"EXP : {character.experience}/{character.expToNextLevel}";
    }

    private void HideUnusedPreviewParts(GameObject previewInstance)
    {
        string[] unwantedNames = { "LowerBodyBone", "WeaponBone", "FirePoint", "Selected", "Shadow", "HitPoint", "Canvas" };
        Transform[] allChildren = previewInstance.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (System.Array.Exists(unwantedNames, name => child.name == name))
                child.gameObject.SetActive(false);
        }
    }
}
