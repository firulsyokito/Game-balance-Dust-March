using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadManager : MonoBehaviour
{
    public static SquadManager Instance;

    public GameObject selectionPanel;
    public GameObject squadPanel;
    public GameObject characterItemPrefab;
    public Transform contentParent;
    public List<SquadSlot> squadSlots;
    public Button closeButton;
    public TextMeshProUGUI emptyText;

    private List<OwnedCharacterData> justRemoved = new List<OwnedCharacterData>();
    private SquadSlot currentSlot;
    private HashSet<OwnedCharacterData> usedCharacters = new HashSet<OwnedCharacterData>();

    public SquadSlot CurrentSlot => currentSlot;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        closeButton.onClick.AddListener(CloseSelectionPanel);

        List<OwnedCharacterData> loadedSquad = SquadSaveManager.LoadSquad();

        for (int i = 0; i < squadSlots.Count && i < loadedSquad.Count; i++)
        {
            var saved = loadedSquad[i];
            var original = InventoryManager.Instance.ownedCharacters.Find(c =>
                c.characterName == saved.characterName &&
                c.level == saved.level &&
                c.attack == saved.attack &&
                c.hp == saved.hp &&
                c.characterClass == saved.characterClass
            );

            if (original != null)
            {
                original.availability = saved.availability;
                original.maxAvailability = saved.maxAvailability;

                squadSlots[i].AssignCharacter(original);
                usedCharacters.Add(original);
            }
            else
            {
                Debug.LogWarning($"⚠️ Character not found in inventory: {saved.characterName}");
            }

            RemoveDeadCharactersFromSquad();
        }

        MapManager.Instance?.UpdatePlayerStatsUI();
    }

    public void RefreshSquadSlotsUI()
    {
        for (int i = 0; i < squadSlots.Count; i++)
        {
            if (!squadSlots[i].IsEmpty())
            {
                var character = squadSlots[i].AssignedCharacter;
                if (character != null)
                    squadSlots[i].AssignCharacter(character);
            }
            else
            {
                squadSlots[i].ClearSlot();
            }
        }
    }

    public void OpenCharacterSelection(SquadSlot slot)
    {
        currentSlot = slot;

        selectionPanel.SetActive(true);
        squadPanel.SetActive(false);

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        bool hasCharacter = false;

        foreach (var character in InventoryManager.Instance.ownedCharacters)
        {
            bool isUsed = usedCharacters.Contains(character);
            bool isUnavailable = character.availability <= 0;
            bool isCurrentSlotCharacter = (currentSlot.AssignedCharacter == character);

            // tampilkan kalau dia belum dipakai, atau dia adalah karakter di slot ini
            if (!isUsed || isCurrentSlotCharacter)
            {
                GameObject item = Instantiate(characterItemPrefab, contentParent);
                var ui = item.GetComponent<SquadCharacterItemUI>();

                // disable kalau dia bukan karakter slot ini dan sedang dipakai atau unavailable
                bool disable = (!isCurrentSlotCharacter && (isUsed || isUnavailable));

                ui.Setup(character, OnCharacterSelected, disable);
                hasCharacter = true;
            }
        }

        if (emptyText != null)
            emptyText.gameObject.SetActive(!hasCharacter);
    }

    public void OnCharacterSelected(OwnedCharacterData character)
    {
        if (currentSlot.AssignedCharacter != null)
        {
            usedCharacters.Remove(currentSlot.AssignedCharacter);
        }

        currentSlot.AssignCharacter(character);
        usedCharacters.Add(character);
        selectionPanel.SetActive(false);
        squadPanel.SetActive(true);
    }

    public void CloseSelectionPanel()
    {
        selectionPanel.SetActive(false);
        squadPanel.SetActive(true);
    }

    public void ApplyTravelAvailability()
    {
        justRemoved.Clear();

        foreach (var slot in squadSlots)
        {
            if (slot.AssignedCharacter != null)
            {
                slot.AssignedCharacter.availability--;

                if (slot.AssignedCharacter.availability <= 0)
                {
                    justRemoved.Add(slot.AssignedCharacter);
                    usedCharacters.Remove(slot.AssignedCharacter);
                    slot.ClearSlot();
                }
            }
        }
    }

    public void RestoreAvailabilityToNonSquad()
    {
        var squadMembers = new HashSet<OwnedCharacterData>();

        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty())
                squadMembers.Add(slot.AssignedCharacter);
        }

        foreach (var character in InventoryManager.Instance.ownedCharacters)
        {
            if (!squadMembers.Contains(character) && !justRemoved.Contains(character))
            {
                if (character.availability < character.maxAvailability)
                {
                    character.availability++;
                }
            }
        }
    }

    public void RemoveUnavailableCharactersFromSquad()
    {
        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty() && slot.AssignedCharacter.availability <= 0)
            {
                usedCharacters.Remove(slot.AssignedCharacter);
                slot.ClearSlot();
            }
        }
    }

    public void RemoveDeadCharactersFromSquad()
    {
        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty() && slot.AssignedCharacter.isDead)
            {
                usedCharacters.Remove(slot.AssignedCharacter);
                slot.ClearSlot();
            }
        }
    }

    public bool HasActiveSquad()
    {
        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty()) return true;
        }
        return false;
    }

    public int GetActiveSquadCount()
    {
        int count = 0;
        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty())
                count++;
        }
        return count;
    }

    public bool IsInSquad(OwnedCharacterData character)
    {
        foreach (var slot in squadSlots)
        {
            if (slot.AssignedCharacter == character)
                return true;
        }
        return false;
    }

    public List<OwnedCharacterData> GetSquadForBattle()
    {
        List<OwnedCharacterData> squad = new List<OwnedCharacterData>();

        foreach (var slot in squadSlots)
        {
            if (!slot.IsEmpty())
                squad.Add(slot.AssignedCharacter);
        }

        return squad;
    }

    public bool IsCharacterInSquad(OwnedCharacterData data)
    {
        return squadSlots.Exists(slot => slot.AssignedCharacter == data);
    }

    public void RemoveFromSquad(OwnedCharacterData data)
    {
        foreach (var slot in squadSlots)
        {
            if (slot.AssignedCharacter == data)
            {
                slot.ClearSlot();
                usedCharacters.Remove(data);
                break;
            }
        }
    }

    public void RemoveUsedCharacter(OwnedCharacterData data)
    {
        usedCharacters.Remove(data);
    }

    public void ShowSquadPanel()
    {
        selectionPanel.SetActive(false);
        squadPanel.SetActive(true);
    }
}
