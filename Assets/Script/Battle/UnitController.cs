using UnityEngine;
using Pathfinding;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Seeker), typeof(AIPath))]
public class UnitController : MonoBehaviour
{
    // Core components
    private Seeker seeker;
    private AIPath aiPath;
    private UnitShooting unitShooting;
    private UnitStats unitStats;
    private SpriteRenderer spriteRenderer;
    private AnimationSwitcher upAnimation;
    private AnimationSwitcher lowAnimation;
    private Camera mainCamera;

    // Dependencies
    [HideInInspector] public CharacterBuilder upCharacterBuilder;
    [HideInInspector] public CharacterBuilder lowCharacterBuilder;
    public GameObject upBodyModel;
    public GameObject lowBodyModel;
    public GameObject selectedVisual;
    public bool selectable = true;

    public enum ControlMode { Passive, Aggressive }
    public ControlMode controlMode = ControlMode.Passive;

    // Internal state
    private bool isMoving;
    private bool isSelected = false;
    private bool wasSelectedLastFrame = false;
    private bool facingRight = true;
    private Vector3 lastLookDir = Vector3.right;
    private Vector3? mouseDestination = null;

    private string runAnim = "Base Layer.Run";
    private string idleAnim = "Base Layer.Idle";
    private string shootAnim = "Base Layer.ShootOnce";

    private const float distanceBuffer = 1f;
    private const float rayOriginOffset = 0.1f;

    [Header("Line of Sight")]
    [SerializeField] private LayerMask obstacleLayerMask = default;

    void Awake()
    {
        mainCamera = Camera.main;
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        unitShooting = GetComponent<UnitShooting>();
        unitStats = GetComponent<UnitStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (upBodyModel != null && lowBodyModel != null)
        {
            upCharacterBuilder = upBodyModel.GetComponent<CharacterBuilder>();
            lowCharacterBuilder = lowBodyModel.GetComponent<CharacterBuilder>();
            upAnimation = upBodyModel.GetComponent<AnimationSwitcher>();
            lowAnimation = lowBodyModel.GetComponent<AnimationSwitcher>();
        }

        if (selectedVisual) selectedVisual.SetActive(false);
    }

    void Update()
    {
        switch (controlMode)
        {
            case ControlMode.Aggressive:
                DoAggressiveBehavior();
                break;
        }

        UpdateFacingDirection();
        UpdateAnimation();
        wasSelectedLastFrame = isSelected;
    }

    private void DoAggressiveBehavior()
    {
        var target = unitShooting?.currentTarget;
        if (target == null) return;

        Vector2 origin = unitShooting.firePoint.position;
        Vector2 targetPos = target.position;
        float dist = Vector2.Distance(origin, targetPos);

        bool withinRange = dist + distanceBuffer < unitStats.shootRange;
        bool blocked = IsLineOfSightBlocked(origin, targetPos);

        if (!withinRange || blocked)
        {
            SetDestination(target.position);
        }
        else
        {
            aiPath.isStopped = true;
            aiPath.canSearch = false;
        }
    }

    private void SetDestination(Vector3 dest)
    {
        aiPath.destination = dest;
        aiPath.canSearch = true;
        aiPath.isStopped = false;

        if (seeker.IsDone())
        {
            aiPath.SearchPath();
        }
    }

    public void MoveTo(Vector3 dest)
    {
        isSelected = false;           // Optional: deselect on move
        wasSelectedLastFrame = false; // Reset previous selection state
        mouseDestination = dest;

        aiPath.isStopped = false;
        aiPath.canSearch = true;
        aiPath.destination = dest;

        if (seeker.IsDone())
            aiPath.SearchPath();
    }


    private bool IsLineOfSightBlocked(Vector2 origin, Vector2 targetPos)
    {
        Vector2 dir = (targetPos - origin).normalized;
        float maxDist = Vector2.Distance(origin, targetPos) - rayOriginOffset;
        if (maxDist <= 0) return false;

        return Physics2D.Raycast(origin, dir, maxDist, obstacleLayerMask).collider != null;
    }

    private void UpdateFacingDirection()
    {
        bool hasValidTarget = unitShooting?.currentTarget != null && unitShooting.enemyIsInRange;
        Vector3 targetDir;

        if (hasValidTarget) {
            targetDir = (unitShooting.currentTarget.position - transform.position).normalized;
        } else {
            Vector2 vel = aiPath.desiredVelocity;
            if (vel.sqrMagnitude < 0.01f) return;
            targetDir = vel.normalized;
        }

        bool shouldFaceRight = targetDir.x > 0f;
        if (shouldFaceRight != facingRight) {
            facingRight = shouldFaceRight;
            if (spriteRenderer != null) {
                spriteRenderer.flipX = !facingRight;
            } else {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (facingRight ? 1 : -1);
                transform.localScale = scale;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (upAnimation == null || lowAnimation == null || aiPath == null) return;
        
        isMoving = aiPath.desiredVelocity.sqrMagnitude > 0.01f && !aiPath.isStopped;

        lowAnimation.PlayAnimation(isMoving ? runAnim : idleAnim);

        if (unitShooting.isShooting && unitShooting.enemyIsInRange)
        {
            // Prioritize shooting animation if shooting
            upAnimation.PlayAnimation(shootAnim);
        }
        else
        {
            // Otherwise, play movement-based animation
            upAnimation.PlayAnimation(isMoving ? runAnim : idleAnim);
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selectable && selected;
        if (selectedVisual) selectedVisual.SetActive(isSelected);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (unitShooting == null || unitStats == null)
            return;

        Vector2 origin = unitShooting.firePoint != null ? unitShooting.firePoint.position : transform.position;
        Vector3 targetPos = unitShooting.currentTarget != null ? unitShooting.currentTarget.position : transform.position + lastLookDir * unitStats.shootRange;

        Vector2 dir = (targetPos - (Vector3)origin).normalized;
        float maxDist = Vector2.Distance(origin, targetPos) - rayOriginOffset;
        Vector2 end = origin + dir * maxDist;

        bool blocked = Physics2D.Raycast(origin, dir, maxDist, obstacleLayerMask);

        Color gizmoColor = blocked ? Color.red : Color.green;
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(origin, end);

        Handles.color = gizmoColor;
        Handles.ArrowHandleCap(0, end, Quaternion.LookRotation(Vector3.forward, dir), 0.2f, EventType.Repaint);
    }
#endif
}
