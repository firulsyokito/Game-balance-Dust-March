using UnityEngine;
using System.Linq;

public class UnitShooting : MonoBehaviour
{
    UnitStats unitStats;
    
    [Header("Targeting")]
    public string enemyTag = "EnemyUnit";
    public string targetPointName = "HitPoint";

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public AudioClip shootSound;

    private float shootTimer;
    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool enemyIsInRange;
    [HideInInspector] public bool isCrit;
    [HideInInspector] public Transform currentTarget;

    void Awake()
    {
        unitStats = GetComponent<UnitStats>();
    }

    void Start()
    {
        shootTimer = 0f;
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        FindTarget();

        if (currentTarget)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            enemyIsInRange = dist < unitStats.shootRange;
            if (enemyIsInRange)
            {
                TryShoot(currentTarget);
            }
        }
    }

    void FindTarget()
    {
        var enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        currentTarget = enemies
            .Select(e =>
            {
                // Try to find the named child (e.g., "HitPoint")
                var hitPoint = e.transform.Find(targetPointName);
                if (hitPoint == null)
                {
                    // If not found, use the root transform
                    return e.transform;
                }
                else
                {
                    return hitPoint;
                }
            })
            .OrderBy(t => Vector2.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void TryShoot(Transform target)
    {
        //timer check
        if (shootTimer < unitStats.fireRate)
        {
            return;
        }
        else
        {
            isShooting = false;
        }

        isShooting = true;

        //sound
        PlayGunshot(shootSound, firePoint.position);

        //shoot bullet
        Vector3 dir = (target.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(dir));

        if (bullet.TryGetComponent(out Bullet bulletComp))
        {
            bulletComp.Initialize(gameObject);
            bulletComp.shooterTeamID = unitStats.teamID;

            isCrit = Random.value < unitStats.critChance;
            if (!isCrit)
            {
                bulletComp.damage = unitStats.damage;
            }
            else
            {
                bulletComp.isCrit = true;
                bulletComp.damage = unitStats.damage * unitStats.critMultiplier;
            }
        }

        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            float speed = bulletComp?.bulletSpeed ?? 10f;
            rb.linearVelocity = dir * speed;
        }
        else if (bulletComp != null)
        {
            bulletComp.SetShootDirection(dir);
            bullet.transform.up = dir;
        }

        shootTimer = 0f;
    }

    public static void PlayGunshot(AudioClip clip, Vector3 position)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = position;

        AudioSource src = tempAudio.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;
        src.spread = 120f;
        src.minDistance = 10f;
        src.maxDistance = 500f;
        src.rolloffMode = AudioRolloffMode.Linear;
        src.volume = 0.5f;
        src.pitch = Random.Range(0.8f, 1.2f);

        src.Play();
        Destroy(tempAudio, clip.length + 0.1f); // cleanup after playback
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnitStats unitStats = GetComponent<UnitStats>();
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, unitStats.shootRange);
    }
#endif
}
