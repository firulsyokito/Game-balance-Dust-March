using UnityEngine;
using Pathfinding; // Untuk AIPath, AIDestinationSetter, Seeker

public static class CharacterPreviewUtil
{
    public static void DisableGameplayComponents(GameObject character)
    {
        // Matikan semua komponen yang tidak dibutuhkan untuk preview
        var aiPath = character.GetComponent<AIPath>();
        var seeker = character.GetComponent<Seeker>();
        var controller = character.GetComponent<UnitController>();
        var shooting = character.GetComponent<UnitShooting>();
        var aiDest = character.GetComponent<AIDestinationSetter>();
        var rigid = character.GetComponent<Rigidbody2D>();
        var col1 = character.GetComponent<CircleCollider2D>();
        var col2 = character.GetComponent<CapsuleCollider2D>();

        if (aiPath) aiPath.enabled = false;
        if (seeker) seeker.enabled = false;
        if (controller) controller.enabled = false;
        if (shooting) shooting.enabled = false;
        if (aiDest) aiDest.enabled = false;
        if (rigid) rigid.simulated = false;
        if (col1) col1.enabled = false;
        if (col2) col2.enabled = false;
    }
}
