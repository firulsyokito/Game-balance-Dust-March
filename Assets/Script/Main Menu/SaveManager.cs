using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class MapSaveData
{
    public string currentNodeName;
    public string resetTriggerNodeName;
    public List<string> unlockedNodes = new();
    public List<string> visitedNodes = new();
    public List<AreaState> areaStates = new();
}

[System.Serializable]
public class AreaState
{
    public string areaName;
    public string missionName;
    public string difficulty;
    public int enemyPerWave;
    public int goldReward;
}


public static class SaveManager
{
    private static string inventoryPath => Application.persistentDataPath + "/inventory.json";
    private static string mapPath => Application.persistentDataPath + "/mapstate.json";
    private static string mapBackupPath => Application.persistentDataPath + "/mapstate_backup.json";

    // ========== INVENTORY ==========
    public static void SaveInventory(List<OwnedCharacterData> characters)
    {
        string json = JsonUtility.ToJson(new CharacterListWrapper { characters = characters });
        File.WriteAllText(inventoryPath, json);
        Debug.Log("Inventory saved to: " + inventoryPath);
    }

    public static List<OwnedCharacterData> LoadInventory()
    {
        if (!File.Exists(inventoryPath))
            return new List<OwnedCharacterData>();

        string json = File.ReadAllText(inventoryPath);
        return JsonUtility.FromJson<CharacterListWrapper>(json)?.characters ?? new List<OwnedCharacterData>();
    }

    [System.Serializable]
    private class CharacterListWrapper
    {
        public List<OwnedCharacterData> characters;
    }

    // ========== MAP SAVE ==========
    public static void SaveMap(MapSaveData mapData)
    {
        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(mapPath, json);
        Debug.Log("Map data saved to: " + mapPath);
    }

    public static MapSaveData LoadMap()
    {
        if (!File.Exists(mapPath))
        {
            Debug.LogWarning("No map save found.");
            return null;
        }

        string json = File.ReadAllText(mapPath);
        return JsonUtility.FromJson<MapSaveData>(json);
    }

    public static void SaveMapBackup(MapSaveData mapData)
    {
        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(mapBackupPath, json);
        Debug.Log("Backup map saved to: " + mapBackupPath);
    }

    public static void RestoreMapFromBackup()
    {
        if (!File.Exists(mapBackupPath))
        {
            Debug.LogWarning("No backup map found.");
            return;
        }

        File.Copy(mapBackupPath, mapPath, overwrite: true);
        Debug.Log("Map restored from backup.");
    }

    public static void DeleteMapSave()
    {
        if (File.Exists(mapPath))
        {
            File.Delete(mapPath);
            Debug.Log("Map save deleted.");
        }
    }

    public static void ResetAllSaves()
    {
        // Hapus semua file
        if (File.Exists(inventoryPath))
            File.Delete(inventoryPath);

        if (File.Exists(mapPath))
            File.Delete(mapPath);

        if (File.Exists(mapBackupPath))
            File.Delete(mapBackupPath);

        // Reset gold
        PlayerPrefs.SetInt("gold", 1000);
        PlayerPrefs.Save();

        // Kosongkan inventory di memori jika InventoryManager ada
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ownedCharacters.Clear();
            SaveInventory(InventoryManager.Instance.ownedCharacters);
        }
        
        SquadTransferData.gameOverCheckEnabled = false;
        Debug.Log("✅ All save files deleted.");
        }   
}
