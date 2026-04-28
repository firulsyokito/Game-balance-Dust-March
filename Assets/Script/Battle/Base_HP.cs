using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class BaseHealth : MonoBehaviour
{
    [Header("UI Element")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject damagePopUpPrefab;

    [Header("Settings")]
    [SerializeField] private int teamID = 0;
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        else
        {
            Debug.LogWarning("HealthBar slider is not assigned in the Inspector.", this);
        }
    }

    public void TakeDamage(float amount, bool isCrit = false)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0f);

        ShowDamagePopUp(amount, isCrit);

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
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

    private void Die()
    {
        Debug.Log(gameObject.name + " has been Destroyed!");

        if (CompareTag("PlayerUnit"))
        {
            GameOverManager.Instance.TriggerLose();
        }
        else if (CompareTag("EnemyUnit"))
        {
            GameOverManager.Instance.TriggerWin();
        }

        gameObject.SetActive(false);
    }

}
