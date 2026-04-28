using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public int shooterTeamID = 0; // Match with UnitStats.teamID
    public float bulletSpeed = 40f;
    public float damage = 10;
    public float lifespan = 2f;
    [HideInInspector] public bool isCrit = false; 

    public GameObject Owner { get; private set; }
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

     public void Initialize(GameObject owner)
    {
        Owner = owner;
    }

    public void SetShootDirection(Vector2 direction)
    {
        direction = direction.normalized;
        rb.linearVelocity = direction * bulletSpeed;

        // Rotate bullet to face movement direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }

    void Start()
    {
        Destroy(gameObject, lifespan); // Auto-destroy
    }
}
