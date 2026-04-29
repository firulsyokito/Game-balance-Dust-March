using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SquadTransferData
{
        public static List<OwnedCharacterData> activeSquad = new List<OwnedCharacterData>();
        public static bool playerWon = true;
        public static int pendingGoldReward = 0;
        public static int pendingExpReward = 0;
        public static bool pendingResetAfterWin = false;
        public static bool justFinishedBattle = false;
        public static bool gameOverCheckEnabled = false;

        public static List<OwnedCharacterData> removedCharacters = new List<OwnedCharacterData>();
}

public static class SquadMemory
{
    public static List<OwnedCharacterData> savedSquad = new List<OwnedCharacterData>();
}

public static class SquadSaveManager
{
    private static string squadPath => Application.persistentDataPath + "/squad.json";

    public static void SaveSquad(List<OwnedCharacterData> squad)
    {
        List<OwnedCharacterData> copiedSquad = new List<OwnedCharacterData>();

        foreach (var character in squad)
        {
            copiedSquad.Add(new OwnedCharacterData
            {
                characterName = character.characterName,
                characterClass = character.characterClass,
                level = character.level,
                attack = character.attack,
                hp = character.hp,
                traits = character.traits,
                availability = character.availability,
                maxAvailability = character.maxAvailability,
            });
        }

        string json = JsonUtility.ToJson(new SquadWrapper { squadMembers = copiedSquad });
        File.WriteAllText(squadPath, json);
        Debug.Log("✅ Squad saved to: " + squadPath);
    }

    public static List<OwnedCharacterData> LoadSquad()
    {
        if (!File.Exists(squadPath)) return new List<OwnedCharacterData>();

        string json = File.ReadAllText(squadPath);
        var wrapper = JsonUtility.FromJson<SquadWrapper>(json);
        return wrapper?.squadMembers ?? new List<OwnedCharacterData>();
    }

    [System.Serializable]
    private class SquadWrapper
    {
        public List<OwnedCharacterData> squadMembers;
    }
}
