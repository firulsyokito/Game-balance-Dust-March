using UnityEngine;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Prefabs & Parents")]
    public GameObject characterItemPrefab;     
    public Transform contentParent;            

    [Tooltip("Optional: drag ModularPreview prefab. Jika null, akan dicari via Resources.")]
    public GameObject characterPreviewPrefab;  

    private List<OwnedCharacterData> currentShopCharacters = new List<OwnedCharacterData>();

    [Header("Modular Options")]
    public string[] hairStyles = { "1", "4", "5" };
    public string[] bodyStyles = { "1", "4", "5" };
    public string[] armStyles = { "1", "4", "5" };
    public string[] legStyles = { "1", "4", "5" };
    public string[] headStyles = { "1", "2", "3", "4" };

    private Dictionary<string, string[]> weaponByClass = new Dictionary<string, string[]>
    {
        { "Marksman", new[] { "Sniper" } },
        { "Rifleman", new[] { "Rifle" } },
        { "Frontliner", new[] { "Shotgun" } },
        { "Rusher", new[] { "SMG" } }
    };

    [Header("Character Info")]
    public string[] names = {
        "Lia", "Rey", "Jin", "Mira", "Kara", "Dex", "Zee", "Alex", "Rin", "Kai", "Noa", "Ash", "Eli", "Robin", "Sky", "Ari", "Sam","Ren", "Drew", "Rowan", "Taylor", "Sage", "Jamie", "Emery", "Morgan", "Adrian", "Quinn",
        "Jules", "River", "Shay", "Casey", "Micah", "Phoenix", "Harper", "Lane", "Blair", "Avery",
        "Jordan", "Tory", "Ezra", "Pax", "Gray", "Brook", "Indigo", "Lux", "Marley", "Finley",
        "Cameron", "Charlie", "Emerson", "Ocean", "Rory", "Skyler", "Easton", "Rowe", "Aspen", "Onyx",
        "Alya", "Nova", "Iris", "Selene", "Luna", "Freya", "Zara", "Nina", "Yara", "Aria",
        "Lyra", "Elara", "Sienna", "Isla", "Vera", "Alina", "Talia", "Seren", "Calla", "Cleo",
        "Mae", "Nora", "Faye", "Amara", "Dahlia", "Esme", "Opal", "Cira", "Elva", "Mina",
        "Aurora", "Evie", "Clara", "Sylvie", "Naomi", "Anya", "Keira", "Thea", "Ivy", "Ada",
        "Juniper", "Adele", "Belle", "Liora", "Elodie", "Zinnia", "Rosalie", "Marina", "Althea", "Giselle",
        "Celeste", "Amira", "Violetta", "Helena", "Lucia", "Odette", "Penelope", "Mirabel", "Bianca", "Florence",
        "Estelle", "Renee", "Tatiana", "Cassandra", "Lilith", "Odessa", "Sabina", "Jasmine", "Celine", "Rowena",
        "Evangeline", "Camellia", "Valeria", "Adelina", "Seraphine", "Cassia", "Felicity", "Rosalind", "Elspeth", "Isolde"
    };

    public string[] classes = { "Marksman", "Rifleman", "Frontliner", "Rusher" };

    // Efek trait terhadap stats
    private Dictionary<string, System.Action<OwnedCharacterData>> traitEffects = new Dictionary<string, System.Action<OwnedCharacterData>>
    {
        { "Strong", (c) => c.attack = Mathf.RoundToInt(c.attack * 1.2f) },
        { "Valiant", (c) => c.hp = Mathf.RoundToInt(c.hp * 1.15f) },
        { "Eagle Eyed", (c) => c.range *= 1.1f },
        { "Positive", (c) => c.fireRate *= 0.9f }
    };

    private Dictionary<string, int> classCosts = new Dictionary<string, int>
    {
        { "Marksman", 200 },
        { "Rifleman", 100 },
        { "Frontliner", 200 },
        { "Rusher", 200 }
    };

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        AddRandomCharactersToShop(4);
    }

    public void AddRandomCharactersToShop(int count)
    {
        // Bersihkan UI item sebelumnya
        foreach (Transform child in contentParent)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        currentShopCharacters.Clear();

        for (int i = 0; i < count; i++)
        {
            OwnedCharacterData character = GenerateRandomCharacter(out int cost);
            currentShopCharacters.Add(character);

            GameObject itemUI = Instantiate(characterItemPrefab, contentParent);
            CharacterItemUI ui = itemUI.GetComponent<CharacterItemUI>();

            // Pastikan prefab modular preview tersedia
            GameObject previewToUse = characterPreviewPrefab != null
                ? characterPreviewPrefab
                : Resources.Load<GameObject>("ModularPreview");

            ui.Setup(character, cost, this, previewToUse);
        }
    }

    public OwnedCharacterData GenerateRandomCharacter(out int cost)
    {
        string selectedClass = classes[Random.Range(0, classes.Length)];

        int spd = 0;
        float range = 0f;
        float fireRate = 0f;
        int attack = 0;
        int hp = 0;

        switch (selectedClass)
        {
            case "Marksman":
                spd = 2; range = 18f; fireRate = 1.2f;
                attack = Random.Range(40, 60); hp = Random.Range(50, 80);
                break;
            case "Rifleman":
                spd = 3; range = 10f; fireRate = 0.8f;
                attack = Random.Range(20, 30); hp = Random.Range(100, 150);
                break;
            case "Frontliner":
                spd = 3; range = 5f; fireRate = 1.1f;
                attack = Random.Range(20, 30); hp = Random.Range(275, 325);
                break;
            case "Rusher":
                spd = 4; range = 8f; fireRate = 0.2f;
                attack = Random.Range(8, 15); hp = Random.Range(125, 200);
                break;
        }

        string hair = hairStyles[Random.Range(0, hairStyles.Length)];
        string body = bodyStyles[Random.Range(0, bodyStyles.Length)];
        string arm = armStyles[Random.Range(0, armStyles.Length)];
        string leg = legStyles[Random.Range(0, legStyles.Length)];
        string head = headStyles[Random.Range(0, headStyles.Length)];
        string weapon = weaponByClass[selectedClass][Random.Range(0, weaponByClass[selectedClass].Length)];

        // Pilih trait random dari dictionary
        string[] traitKeys = new List<string>(traitEffects.Keys).ToArray();
        string selectedTrait = traitKeys[Random.Range(0, traitKeys.Length)];

        // Buat karakter dengan stat dasar
        OwnedCharacterData newChar = new OwnedCharacterData
        {
            characterName = names[Random.Range(0, names.Length)],
            characterClass = selectedClass,
            traits = selectedTrait,
            level = 1,
            attack = attack,
            hp = hp,
            spd = spd,
            range = range,
            fireRate = fireRate,
            availability = 3,
            maxAvailability = 3,

            hairStyle = hair,
            bodyStyle = body,
            armStyle = arm,
            legStyle = leg,
            headStyle = head,
            weaponStyle = weapon
        };

        // Terapkan efek trait
        traitEffects[selectedTrait](newChar);

        cost = classCosts[selectedClass];
        return newChar;
    }

    public GameObject CreateCharacterPrefabInstance(OwnedCharacterData data)
    {
        GameObject basePrefab = characterPreviewPrefab != null
            ? characterPreviewPrefab
            : Resources.Load<GameObject>("ModularPreview");

        if (basePrefab == null)
        {
            Debug.LogError("ModularPreview prefab tidak ditemukan.");
            return null;
        }

        GameObject instance = Instantiate(basePrefab);
        CharacterBuilder builder = instance.GetComponent<CharacterBuilder>();
        if (builder != null)
        {
            builder.ApplyCharacterData(data);
        }
        else
        {
            Debug.LogWarning("CharacterBuilder tidak ditemukan di prefab.");
        } 

        return instance;
    }

    public void PurchaseCharacter(OwnedCharacterData data)
    {
        InventoryManager.Instance?.AddCharacter(data);
        Debug.Log($"Purchased character: {data.characterName}");
    }

    public List<OwnedCharacterData> GetShopCharacters()
    {
        return currentShopCharacters;
    }
}
