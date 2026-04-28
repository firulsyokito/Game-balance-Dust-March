using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject playerUnitPrefab;
    public Transform[] spawnPoints;

    void Start()
    {
        var squad = SquadTransferData.activeSquad;

        for (int i = 0; i < squad.Count && i < spawnPoints.Length; i++)
        {
            GameObject unit = Instantiate(playerUnitPrefab, spawnPoints[i].position, Quaternion.identity);
            var charData = squad[i];

            unit.tag = "PlayerUnit";

            // Apply stats + simpan referensi karakter
            UnitStats unitStats = unit.GetComponent<UnitStats>();
            if (unitStats != null)
            {
                unitStats.SetStats(
                    charData.hp,
                    charData.attack,
                    charData.spd,
                    charData.range,
                    charData.fireRate,
                    charData // sekarang langsung kirim OwnedCharacterData
                );
            }

            var root = unit.GetComponent<UnitController>();
            if (root != null)
            {
                root.upCharacterBuilder.ApplyCharacterData(charData);
                root.lowCharacterBuilder.ApplyCharacterData(charData);
            }
            else
            {
                Debug.LogWarning("CharacterBuilder not found on playerUnitPrefab!");
            }
        }
    }
}
