using UnityEngine;
using System.Linq;

public class Obstacle : MonoBehaviour
{
    [Tooltip("Radius within which shooters can protect their bullets")]
    public float unitRadius = 2f;

    [Range(0f, 1f)]
    public float destroyChance = 0.9f;

    [Tooltip("LayerMask for shooter units")]
    public LayerMask unitLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet == null) return;

        // If shooter who fired it is nearby, do nothing
        if (IsShooterNearby(bullet.Owner))
            return;

        // Otherwise normal destroy chance
        if (Random.value < destroyChance)
            Destroy(bullet.gameObject);
    }

    private bool IsShooterNearby(GameObject shooter)
    {
        if (shooter == null) return false;

        // Find all units nearby
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, unitRadius, unitLayer);
        return hits.Any(c => c.gameObject == shooter);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, unitRadius);
    }
#endif
}
