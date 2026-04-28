using UnityEngine;
using UnityEngine.UI;

public class BattleSquadStatusPanel : MonoBehaviour
{
    [Header("UI References")]
    public Transform squadParent;
    public GameObject squadSlotPrefab;
    public Button closeButton;

    [Header("Root Container")]
    public GameObject rootContainer; // Empty parent, isi panel & UI

    void Start()
    {
        if (SquadTransferData.justFinishedBattle)
        {
            rootContainer.SetActive(true);   // ✅ tampilkan panel
            ShowStatus();

            if (closeButton != null)
                closeButton.onClick.AddListener(ClosePanelAndRemoveDead);

            // Reset flag supaya tidak muncul lagi di scene berikutnya
            SquadTransferData.justFinishedBattle = false;
        }
        else
        {
            rootContainer.SetActive(false);  // ✅ sembunyikan kalau bukan habis battle
        }

    }

    public void ShowStatus()
    {
        // Bersihkan slot lama
        foreach (Transform child in squadParent)
            Destroy(child.gameObject);

        // Tampilkan semua karakter di activeSquad
        foreach (var character in SquadTransferData.activeSquad)
        {
            CreateSlot(character, character.isDead);
        }

        // Tambahkan karakter mati yang tidak ada di activeSquad
        foreach (var deadChar in SquadTransferData.removedCharacters)
        {
            if (!SquadTransferData.activeSquad.Contains(deadChar))
                CreateSlot(deadChar, true);
        }
    }

    private void CreateSlot(OwnedCharacterData character, bool isDead)
    {
        GameObject slotObj = Instantiate(squadSlotPrefab, squadParent);
        var slotUI = slotObj.GetComponent<BattleSquadSlotUI>();

        if (slotUI != null)
        {
            string statusText = "";
            if (isDead)
                statusText = "Die";
            else if (character.availability < 0)
                statusText = "Wounded";

            slotUI.AssignCharacter(character, statusText);
        }
    }

   private void ClosePanelAndRemoveDead()
    {
        // Ambil semua karakter mati
        var deadCharacters = SquadTransferData.activeSquad.FindAll(c => c.isDead);

        // Hapus dari activeSquad
        SquadTransferData.activeSquad.RemoveAll(c => c.isDead);

        // Hapus juga dari Inventory
        foreach (var deadChar in deadCharacters)
        {
            InventoryManager.Instance.ownedCharacters.RemoveAll(invChar => 
                InventoryManager.Instance.IsSameCharacter(invChar, deadChar)
            );
        }

        // Bersihkan removedCharacters
        SquadTransferData.removedCharacters.Clear();

        // Simpan squad terbaru ke JSON
        SquadSaveManager.SaveSquad(SquadTransferData.activeSquad);

        // Simpan inventory terbaru ke JSON
        SaveManager.SaveInventory(InventoryManager.Instance.ownedCharacters);

        if (MapManager.Instance != null)
        {
            MapManager.Instance.UpdatePlayerStatsUI();
            MapManager.Instance.CheckGameOverCondition();
        }

        // Nonaktifkan root container (bukan cuma panel)
        if (rootContainer != null)
            rootContainer.SetActive(false);
        else
            gameObject.SetActive(false); // fallback kalau lupa assign
}

}
