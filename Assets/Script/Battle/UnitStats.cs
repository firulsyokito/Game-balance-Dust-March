using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class UnitStats : MonoBehaviour
{
    public static List<UnitStats> allUnits = new List<UnitStats>();

    private AIPath aiPath;
    private OwnedCharacterData linkedCharacterData;
    public OwnedCharacterData LinkedCharacterData => linkedCharacterData;

    [Header("UI Elements")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject damagePopUpPrefab;

    [Header("Stats Settings")]
    public int teamID = 0;
    public float maxHealth = 100f;
    public float damage = 5f;
    public float speed = 2f;
    public float fireRate = 1f;
    public float shootRange = 10f;
    public float critChance = 0.1f;
    public float critMultiplier = 2f;

    private float currentHealth;

    void Awake()
    {
        allUnits.Add(this);

        aiPath = GetComponent<AIPath>();
        aiPath.maxSpeed = speed;

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    void OnDestroy()
    {
        allUnits.Remove(this); // hapus dari daftar saat destroy
    }

    public void SetStats(int hp, int atk, int spd, float range, float rate, OwnedCharacterData data)
    {
        maxHealth = hp;
        damage = atk;
        speed = spd;
        shootRange = range;
        fireRate = rate;

        linkedCharacterData = data;

        currentHealth = maxHealth;

        if (aiPath != null)
            aiPath.maxSpeed = speed;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    public void TakeDamage(float amount, bool isCrit = false)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        ShowDamagePopUp(amount, isCrit);

        if (healthBar != null)
            healthBar.value = currentHealth;

        if (currentHealth <= 0)
            Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Bullet bulletScript = collision.GetComponent<Bullet>();
            if (bulletScript != null && bulletScript.shooterTeamID != teamID)
            {
                TakeDamage(bulletScript.damage, bulletScript.isCrit);
                Destroy(collision.gameObject);
            }
        }
    }

    private void ShowDamagePopUp(float damageAmount, bool isCrit)
    {
        if (damagePopUpPrefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(0f, 1f, 0f);
        GameObject damagePopUp = Instantiate(damagePopUpPrefab, spawnPos, Quaternion.identity);

        if (damagePopUp.TryGetComponent(out DamagePopUp damagePopUpComp))
        {
            damagePopUpComp.textMesh.text = damageAmount.ToString();
            damagePopUpComp.isCrit = isCrit;
        }
    }

    public void ApplyAvailabilityPenalty()
    {
        if (linkedCharacterData == null) return;

        float hpPercent = currentHealth / maxHealth;

        if (hpPercent <= 0.25f)
            linkedCharacterData.availability = -3;
        else if (hpPercent <= 0.50f)
            linkedCharacterData.availability = -2;
        else if (hpPercent <= 0.75f)
            linkedCharacterData.availability = -1;
    }

    private void Die()
    {
        if (linkedCharacterData != null)
        {
            linkedCharacterData.isDead = true; // tandai sebagai mati
            if (!SquadTransferData.removedCharacters.Contains(linkedCharacterData))
            {
                SquadTransferData.removedCharacters.Add(linkedCharacterData);
            }
        }
        
        Destroy(gameObject);
    }
}
