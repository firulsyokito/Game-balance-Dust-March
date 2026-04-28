using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
[ExecuteAlways]
public class RectMask2DForceEnabler : MonoBehaviour
{
    private MaskableGraphic graphic;

    private MaskableGraphic Graphic =>
        graphic ??= GetComponent<MaskableGraphic>();

    private void Update()
    {
        var mask = transform.parent?.GetComponentInParent<RectMask2D>();
        if (mask != null)
        {
            mask.AddClippable(Graphic);
        }
    }
}
