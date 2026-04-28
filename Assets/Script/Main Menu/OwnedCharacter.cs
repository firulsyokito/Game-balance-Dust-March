[System.Serializable]
public class OwnedCharacterData
{
    public string characterName;
    public string characterClass;
    public int level;
    public int attack;
    public int hp;
    public int spd;
    public float range;
    public float fireRate;
    public string traits;
    public int availability;
    public int maxAvailability;
    
    public bool isDead;
    public int experience; // EXP saat ini
    public int expToNextLevel = 100; // kebutuhan EXP

    public string hairStyle;
    public string bodyStyle;
    public string armStyle;
    public string legStyle;
    public string headStyle;
    public string weaponStyle;
}
