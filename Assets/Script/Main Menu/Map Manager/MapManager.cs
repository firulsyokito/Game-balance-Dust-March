using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Area References")]
    public AreaNode[] allAreas;

    [Header("Reset Trigger")]
    public AreaNode resetTriggerArea;

    [Header("UI - Confirmation")]
    public GameObject confirmationPanel;
    public Button confirmButton;
    public Button cancelButton;

    [Header("UI - Difficulty Icon")]
    public Image difficultyIcon;
    public Sprite easyIcon;
    public Sprite mediumIcon;
    public Sprite hardIcon;
    public Sprite finalIcon;

    [Header("UI - Area Info")]
    public TextMeshProUGUI missionNameText;
    public GameObject areaInfoPanel;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI enemyCountText;
    public Image enemyImage;
    public TextMeshProUGUI goldRewardText;
    public TextMeshProUGUI briefText;

    [Header("UI - Player Stats")]
    public TextMeshProUGUI squadCountText;
    public TextMeshProUGUI inventoryCountText;
    public TextMeshProUGUI goldAmountText;
    public Slider progressBar;

    private int exploredNodeCount = 0;
    private int totalNodeCount;

    [Header("Managers")]
    public ShopManager shopManager;
    public ConfirmationManager ConfirmationManager;

    private AreaNode selectedArea;
    private AreaNode currentArea;

    [Header("Mission Name Pool")]
    public string[] possibleMissionNames = {
        "Rescue Operation", "Sabotage Supply Line", "Secure the Ruins",
        "Evacuate Civilians", "Data Retrieval", "Enemy Encampment",
        "Night Raid", "Defense Protocol", "Ghost Recon",
        "Final Stand", "Mighty Encounter", "Thunder Raid", "Zerk Push"
    };

    private Dictionary<string, List<string>> missionBriefings = new Dictionary<string, List<string>>()
    {
        { "Rescue Operation", new List<string> {
            "A group of allies are trapped behind enemy lines. Get in, get them out.",
            "The clock is ticking. Extract the captives before it's too late.",
            "Stealth is key. Rescue civilians without alerting the enemy."
        }},
        { "Sabotage Supply Line", new List<string> {
            "Their logistics are vulnerable. Strike now.",
            "Disrupt their food and fuel supply. Leave nothing behind.",
            "Sabotage the enemy’s lifeline to slow down their advance."
        }},
        { "Secure the Ruins", new List<string> {
            "An ancient site holds valuable intel. Secure and hold it.",
            "Enemy forces patrol the ruins. Expect resistance.",
            "Push through the rubble and claim what's ours."
        }},
        {
        "Evacuate Civilians", new List<string> {
            "Time is running out. Get everyone to safety.",
            "Enemy forces are approaching fast. Evacuate the civilians now.",
            "Protect the convoy and make sure no one is left behind."
        }},
        { "Data Retrieval", new List<string> {
            "Sensitive intel is stored nearby. Retrieve it before the enemy does.",
            "Sneak in, extract the data, and get out alive.",
            "The success of future operations depends on this data."
        }},
        { "Enemy Encampment", new List<string> {
            "Infiltrate and neutralize their forward base.",
            "Strike the heart of the enemy camp and cripple their operations.",
            "This stronghold has to fall. Hit fast, hit hard."
        }},
        { "Night Raid", new List<string> {
            "Strike under the cover of darkness. Speed and silence are your allies.",
            "Enemies won't see it coming. Hit hard, retreat fast.",
            "Night ops require nerves of steel. Don’t get surrounded."
        }},
        { "Defense Protocol", new List<string> {
            "The enemy is coming. Hold your ground at all costs.",
            "Establish a perimeter and brace for waves of attacks.",
            "Activate the defense grid and stand firm."
        }},
        { "Ghost Recon", new List<string> {
            "Go unseen. Gather recon without raising alarms.",
            "Observe, report, and disappear like a ghost.",
            "Silent movement is vital. Avoid unnecessary conflict."
        }},
        { "Final Stand", new List<string> {
            "This is it. Make every move count.",
            "Our last line of defense. There is no fallback.",
            "Hold the line. We either win or fall here."
        }},
        { "Mighty Encounter", new List<string> {
            "An elite enemy force blocks your path. Engage with caution.",
            "You face their champions now. Show them your strength.",
            "Only the best make it through. Fight like legends."
        }},
        { "Thunder Raid", new List<string> {
            "Rain down destruction like thunder. Fast, loud, and deadly.",
            "Strike swiftly and overwhelm them before they regroup.",
            "Let the battlefield echo with your fury."
        }},
        { "Zerk Push", new List<string> {
            "Unleash chaos. Push forward with unrelenting force.",
            "No time to think. Just charge and dominate.",
            "This mission demands brute strength and relentless pressure."
        }}
    };

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        MapSaveData loaded;
        SquadManager.Instance.RemoveUnavailableCharactersFromSquad();

        if (!SquadTransferData.playerWon)
        {
            // Kalau kalah
            SaveManager.RestoreMapFromBackup();
            SquadTransferData.playerWon = true;
            Debug.Log("🟥 Player lost. Restoring previous state...");
            SquadTransferData.pendingGoldReward = 0;
        }
        else
        {
            // Kalau menang
            EconomyManager.Instance.AddGold(SquadTransferData.pendingGoldReward);
            Debug.Log($"💰 Gold reward {SquadTransferData.pendingGoldReward} added after win.");
            SquadTransferData.pendingGoldReward = 0;

            // ✅ Tambahan: Kalau menang di final battle, munculkan reset panel
            if (SquadTransferData.pendingResetAfterWin)
            {
                // Hentikan aksi langsung reset, tunggu konfirmasi pemain
                ConfirmationManager.Instance.ShowResetPanel();
                SquadTransferData.pendingResetAfterWin = false;
            }
        }

        loaded = SaveManager.LoadMap();
        if (loaded != null)
        {
            Debug.Log("🟢 Map state found, restoring...");
            RestoreMapState(loaded);
        }
        else
        {
            Debug.Log("🔁 No save found, starting new game.");
            AssignRandomMissionNames();
            foreach (var area in allAreas)
                area.GenerateStatsFromDifficulty();

            currentArea = allAreas[0];
            currentArea.Unlock();
            currentArea.SetAsCurrent();
            UnlockAdjacent(currentArea);
            exploredNodeCount = 0;
        }

        confirmationPanel.SetActive(false);
        areaInfoPanel.SetActive(false);
        totalNodeCount = allAreas.Length;

        SquadManager.Instance?.RefreshSquadSlotsUI();
        UpdatePlayerStatsUI();

        confirmButton.onClick.AddListener(ConfirmTravel);
        cancelButton.onClick.AddListener(CloseConfirmation);
        CheckGameOverCondition();
    }


    void AssignRandomMissionNames()
    {
        List<string> availableNames = new List<string>(possibleMissionNames);
        System.Random rng = new System.Random();

        foreach (var area in allAreas)
        {
            if (availableNames.Count == 0)
                availableNames = new List<string>(possibleMissionNames);

            int index = rng.Next(availableNames.Count);
            area.missionName = availableNames[index];
            availableNames.RemoveAt(index);
            area.UpdateMapLabels();
        }
    }

    void RestoreMapState(MapSaveData data)
    {
        foreach (var area in allAreas)
        {
            area.isUnlocked = false;
            area.isCurrentArea = false;

            if (data.unlockedNodes.Contains(area.name))
                area.Unlock();

            if (data.visitedNodes.Contains(area.name))
                area.hasBeenCurrent = true;

            if (area.name == data.currentNodeName)
            {
                currentArea = area;
                currentArea.SetAsCurrent();
                Debug.Log($"🟡 Restored current area: {currentArea.name}");
            }

            if (area.name == data.resetTriggerNodeName)
                resetTriggerArea = area;

            var state = data.areaStates.Find(s => s.areaName == area.name);
            if (state != null)
            {
                area.missionName = state.missionName;
                if (System.Enum.TryParse(state.difficulty, out AreaNode.DifficultyLevel diff))
                    area.difficulty = diff;

                area.enemyPerWave = state.enemyPerWave;
                area.goldReward = state.goldReward;
            }

            area.UpdateMapLabels();
            area.UpdateVisual();
        }

        if (currentArea == null)
        {
            Debug.LogWarning("⚠️ No current area found in save data.");
        }

        UnlockAdjacent(currentArea);
    }

    public void ShowConfirmation(AreaNode area)
    {
        if (area.hasBeenCurrent)
            return;

        selectedArea = area;
        confirmationPanel.SetActive(true);
        areaInfoPanel.SetActive(true);

        missionNameText.text = $"{area.missionName}";
        difficultyText.text = $"Difficulty : {area.difficulty}";
        enemyCountText.text = $"Enemy/Wave : {area.enemyPerWave}";
        enemyImage.sprite = area.enemySprite;
        goldRewardText.text = $"{area.goldReward} Gold";

        switch (area.difficulty)
        {
            case AreaNode.DifficultyLevel.Easy:
                difficultyIcon.sprite = easyIcon;
                break;
            case AreaNode.DifficultyLevel.Medium:
                difficultyIcon.sprite = mediumIcon;
                break;
            case AreaNode.DifficultyLevel.Hard:
                difficultyIcon.sprite = hardIcon;
                break;
            case AreaNode.DifficultyLevel.Final:
                difficultyIcon.sprite = finalIcon;
                break;
            default:
                difficultyIcon.sprite = null;
                break;
        }

        if (missionBriefings.TryGetValue(area.missionName, out var briefList))
        {
            int randomIndex = Random.Range(0, briefList.Count);
            briefText.text = briefList[randomIndex];
        }
        else
        {
            briefText.text = "No mission intel available. Proceed with caution.";
        }
    }


    void ConfirmTravel()
    {
        if (!SquadManager.Instance.HasActiveSquad())
        {
            Debug.LogWarning("No active characters in squad! Cannot travel.");
            briefText.text = "You must assign at least one character to the squad before starting the mission.";
            briefText.color = new Color(1f, 0.65f, 0f);;
            return;
        }

        SaveMapStateBackup();

        SquadTransferData.activeSquad = SquadManager.Instance.GetSquadForBattle();
        exploredNodeCount++;

        confirmationPanel.SetActive(false);
        areaInfoPanel.SetActive(false);

        currentArea.isCurrentArea = false;
        currentArea.UpdateVisual();

        currentArea = selectedArea;
        currentArea.SetAsCurrent();

        LockAllAreasExcept(currentArea);
        UnlockAdjacent(currentArea);

        shopManager.AddRandomCharactersToShop(4);

        SquadManager.Instance.ApplyTravelAvailability();
        SquadManager.Instance.RestoreAvailabilityToNonSquad();
        UpdatePlayerStatsUI();

        if (selectedArea == resetTriggerArea)
        {
            SquadTransferData.pendingResetAfterWin = true;
        }
        else
        {
            SquadTransferData.pendingResetAfterWin = false;
        }


        SaveMapState();
        SquadSaveManager.SaveSquad(SquadManager.Instance.GetSquadForBattle());
        SaveManager.SaveInventory(InventoryManager.Instance.ownedCharacters);
        SquadTransferData.pendingGoldReward = selectedArea.goldReward;
        WaveSpawner.enemyPerWave = selectedArea.enemyPerWave;

    int sceneToLoad = 2; // default jika tidak sesuai
    switch (selectedArea.difficulty)
    {
        case AreaNode.DifficultyLevel.Easy:
            sceneToLoad = 3;
            break;
        case AreaNode.DifficultyLevel.Medium:
            sceneToLoad = 4;
            break;
        case AreaNode.DifficultyLevel.Hard:
            sceneToLoad = 5;
            break;
        case AreaNode.DifficultyLevel.Final:
            sceneToLoad = 6;
            break;
    }

    SceneManager.LoadScene(sceneToLoad);
    }

    void SaveMapState()
    {
        MapSaveData data = CollectCurrentMapData();
        SaveManager.SaveMap(data);
    }

    void SaveMapStateBackup()
    {
        MapSaveData data = CollectCurrentMapData();
        SaveManager.SaveMapBackup(data);
    }

    public MapSaveData CollectCurrentMapData()
    {
        MapSaveData data = new MapSaveData();
        data.currentNodeName = currentArea.name;
        data.resetTriggerNodeName = resetTriggerArea?.name;

        foreach (var area in allAreas)
        {
            if (area.isUnlocked)
                data.unlockedNodes.Add(area.name);
            if (area.hasBeenCurrent)
                data.visitedNodes.Add(area.name);

            data.areaStates.Add(new AreaState
            {
                areaName = area.name,
                missionName = area.missionName,
                difficulty = area.difficulty.ToString(),
                enemyPerWave = area.enemyPerWave,
                goldReward = area.goldReward
            });
        }

        return data;
    }

    public void ResetMapProgress()
    {
        exploredNodeCount = 0;

        foreach (var area in allAreas)
        {
            area.hasBeenCurrent = false;
            area.isUnlocked = false;
            area.isCurrentArea = false;
            area.UpdateVisual();
        }

        AssignRandomMissionNames();

        currentArea = allAreas[0];
        currentArea.Unlock();
        currentArea.SetAsCurrent();

        UnlockAdjacent(currentArea);
        UpdatePlayerStatsUI();
    }


    void CloseConfirmation()
    {
        confirmationPanel.SetActive(false);
        areaInfoPanel.SetActive(false);
        briefText.text = "";
        briefText.color = Color.white;
        
    }

    void LockAllAreasExcept(AreaNode keepUnlocked)
    {
        foreach (var area in allAreas)
        {
            if (area != keepUnlocked && !area.isCurrentArea)
            {
                area.isUnlocked = false;
                area.UpdateVisual();
            }
        }
    }

    void UnlockAdjacent(AreaNode area)
    {
        if (area == null || area.connectedAreas == null) return;

        foreach (var adj in area.connectedAreas)
        {
            if (adj != null && !adj.isUnlocked)
                adj.Unlock();
        }
    }

    public void UpdatePlayerStatsUI()
    {
        int woundedCount = InventoryManager.Instance.GetWoundedCharacterCountFromInventory();
        int availableCount = InventoryManager.Instance.GetAvailableInventoryCount();

        squadCountText.text = $"{woundedCount}";
        inventoryCountText.text = $"{availableCount}";
        goldAmountText.text = $"{EconomyManager.Instance.GetGold()}";
        progressBar.value = (float)exploredNodeCount / totalNodeCount;
    }

    public void CheckGameOverCondition()
    {   
        if (!SquadTransferData.gameOverCheckEnabled)
        {
            return;
        }
        else if (InventoryManager.Instance.GetAvailableInventoryCount() <= 0)
        {
            Debug.Log("💀 Game Over: tidak ada karakter tersedia.");
            ConfirmationManager.Instance.ShowGameOverPanel();
        }
    }

    public void ClearAllCurrentAreas()
    {
        foreach (var area in allAreas)
        {
            area.isCurrentArea = false;
            area.UpdateVisual();
        }
    }
}
