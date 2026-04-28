using UnityEngine;
using UnityEngine.UI;

public class ToggleUnitMode : MonoBehaviour
{
    public Image buttonImage;
    public Sprite passiveSprite;
    public Sprite aggressiveSprite;
    private bool isAggressive = false; // Start in passive mode

    private void Start()
    {
        if (buttonImage == null)
        {
            Debug.LogError("ToggleUnitMode script must be attached to a GameObject with an Image component.");
            return;
        }

        // Set initial sprite to passive
        buttonImage.sprite = passiveSprite;
        SetAllUnitsMode(UnitController.ControlMode.Passive);
    }

    public void OnModeButtonClicked()
    {
        isAggressive = !isAggressive;

        // Toggle sprite
        buttonImage.sprite = isAggressive ? aggressiveSprite : passiveSprite;

        // Set all units to selected mode
        var mode = isAggressive ? UnitController.ControlMode.Aggressive : UnitController.ControlMode.Passive;
        SetAllUnitsMode(mode);
    }

    private void SetAllUnitsMode(UnitController.ControlMode mode)
    {
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("PlayerUnit");

        foreach (GameObject unit in playerUnits)
        {
            UnitController controller = unit.GetComponent<UnitController>();
            if (controller != null)
            {
                controller.controlMode = mode;
            }
        }
    }
}
