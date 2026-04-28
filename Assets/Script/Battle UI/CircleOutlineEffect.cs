using UnityEngine;

public class CircleOutlineEffect : MonoBehaviour
{
    public float targetScale = 0.2f;
    public float duration = 0.1f;

    private float elapsed = 0f;
    private Vector3 initialScale;

    private void OnEnable()
    {
        elapsed = 0f;
        initialScale = Vector3.zero;
        transform.localScale = initialScale;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        transform.localScale = Vector3.Lerp(initialScale, Vector3.one * targetScale, t);

        if (t >= 1f)
        {
            gameObject.SetActive(false);
        }
    }
}
