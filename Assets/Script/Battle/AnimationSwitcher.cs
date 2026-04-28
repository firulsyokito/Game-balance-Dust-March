using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSwitcher : MonoBehaviour
{
    private Animator animator;

    string currentState;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Smoothly crossfade to the given animation state.
    /// StateName should include layer prefix if needed, e.g. "Base Layer.Run".
    /// </summary>
    public void PlayAnimation(string stateName, float normalizedBlend = 0.2f, int layer = 0)
    {
        if (currentState == stateName) return;

        currentState = stateName;

        animator.CrossFade(stateName, normalizedBlend, layer);
    }
}
