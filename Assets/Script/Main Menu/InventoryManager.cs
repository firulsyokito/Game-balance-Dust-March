using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<OwnedCharacterData> ownedCharacters = new List<OwnedCharacterData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <- make persistent
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadInventory();

        // (optional) apply pending removals from SquadTransferData if any
        if (SquadTransferData.removedCharacters.Count > 0)
        {
            foreach (var rem in SquadTransferData.removedCharacters)
                ownedCharacters.RemoveAll(c => IsSameCharacter(c, rem));
            SaveManager.SaveInventory(ownedCharacters);
            SquadTransferData.removedCharacters.Clear();
        }
    }

    public bool IsSameCharacter(OwnedCharacterData a, OwnedCharacterData b)
    {
        if (a == null || b == null) return false;
        return a.characterName == b.characterName && a.level == b.level && a.hp == b.hp && a.attack == b.attack;
    }

    public void AddCharacter(OwnedCharacterData character)
    {
        ownedCharacters.Add(character);
        SaveManager.SaveInventory(ownedCharacters);
    }

    public void LoadInventory()
    {
        ownedCharacters = SaveManager.LoadInventory();
    }

        public int GetAvailableInventoryCount()
    {
        int count = 0;

        foreach (var character in ownedCharacters)
        {
            if (character.availability > 0)
            {
                count++;
            }
        }

        return count;
    }

     public int GetWoundedCharacterCountFromInventory()
    {
        int count = 0;

        foreach (var character in InventoryManager.Instance.ownedCharacters)
        {
            if (character.availability <= 0)
                count++;
        }

        return count;
    }

}
