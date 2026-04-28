using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public bool isCrit;
    public float moveYSpeed = 1f;
    public float disappearSpeed = 2f;
    private float DISAPPEAR_TIMER_MAX = 0.5f;
    private float disappearTimer;
    private static int sortingOder;

    [HideInInspector] public TextMeshPro textMesh;

    [Header("Popup Colors")]
    [Tooltip("Color to use for normal damage text")]
    public Color normalColor = Color.yellowNice;
    public Color critColor = Color.red;

    [Tooltip("Font size for critical hits")]
    public float critFontSize = 4f;

    [Tooltip("Font size for normal hits")]
    public float normalFontSize = 3f;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        sortingOder++;
        textMesh.sortingOrder = sortingOder;
    }

    private void Start()
    {
        if (!isCrit)
        {
            textMesh.fontSize = normalFontSize;
            textMesh.color = normalColor;
        }
        else
        {
            textMesh.fontSize = critFontSize;
            textMesh.color = critColor;
        }

        disappearTimer = DISAPPEAR_TIMER_MAX;

        Destroy(gameObject, 1f);
    }
    void Update()
    {
        disappearTimer -= Time.deltaTime;

        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            transform.localScale += Vector3.one * moveYSpeed * Time.deltaTime;
            transform.position += new Vector3(0.5f, moveYSpeed) * Time.deltaTime;
        }
        else
        {
            transform.localScale -= Vector3.one * moveYSpeed * Time.deltaTime;
            transform.position -= new Vector3(0, moveYSpeed) * Time.deltaTime;
        }

        textMesh.alpha -= disappearSpeed * Time.deltaTime;
    }
}