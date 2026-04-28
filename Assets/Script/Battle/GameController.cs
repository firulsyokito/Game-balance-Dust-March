using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform selectionAreaTransform;
    [SerializeField] private GameObject clickVisual;

    private Vector2 inputStartPos;
    private bool isSelecting;
    private bool didSelectUnit;

    private const float TwoTapMaxDelay = 0.3f;
    private const float MaxTapMovement = 20f;
    private const float dragStartDistance = 10f;

    private float firstTouchStartTime;

    private readonly List<UnitController> selectedUnits = new();
    private const string UnitTag = "PlayerUnit";

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        selectionAreaTransform.gameObject.SetActive(false);
        EnhancedTouchSupport.Enable();
    }

    private void OnDestroy() => EnhancedTouchSupport.Disable();

    private void Update()
    {
        HandleMouseInput();
        HandleTouchInput();
    }

    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (Mouse.current.leftButton.wasPressedThisFrame)
            OnInputStart(mousePos);
        else if (Mouse.current.leftButton.isPressed)
            OnInputHold(mousePos);  // <-- we now use a new method to process movement
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
            OnInputEnd(mousePos);
        else if (Mouse.current.rightButton.wasPressedThisFrame)
            DeselectAllUnits();
    }

    private void HandleTouchInput()
    {
        var touches = Touch.activeTouches;
        if (touches.Count == 1)
        {
            var touch = touches[0];
            Vector2 pos = touch.screenPosition;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    firstTouchStartTime = Time.time;
                    OnInputStart(pos);
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    OnInputHold(pos);
                    break;
                case TouchPhase.Ended:
                    OnInputEnd(pos);
                    break;
            }
        }
        else if (touches.Count == 2)
        {
            var touch1 = touches[0];
            var touch2 = touches[1];

            if (touch1.phase == TouchPhase.Began && touch2.phase == TouchPhase.Began)
                firstTouchStartTime = Time.time;

            if (touch1.phase == TouchPhase.Ended && touch2.phase == TouchPhase.Ended)
            {
                float elapsed = Time.time - firstTouchStartTime;
                if (elapsed <= TwoTapMaxDelay &&
                    Vector2.Distance(touch1.screenPosition, touch1.startScreenPosition) <= MaxTapMovement &&
                    Vector2.Distance(touch2.screenPosition, touch2.startScreenPosition) <= MaxTapMovement)
                {
                    DeselectAllUnits();
                }
            }
        }
    }

    private void OnInputStart(Vector2 screenPos)
    {
        didSelectUnit = false;
        inputStartPos = screenPos;
        isSelecting = false;  // don't start selecting yet

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(screenPos), Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag(UnitTag) && hit.collider.TryGetComponent(out UnitController unit))
        {
            DeselectAllUnits();
            unit.SetSelected(true);
            selectedUnits.Add(unit);
            didSelectUnit = true;
        }
    }

    private void OnInputHold(Vector2 screenPos)
    {
        // Check if we need to start drag-selection
        if (!isSelecting)
        {
            float dist = Vector2.Distance(screenPos, inputStartPos);
            if (dist >= dragStartDistance)
            {
                isSelecting = true;
                selectionAreaTransform.gameObject.SetActive(true);
            }
        }

        // If drag-selection has started, update selection box
        if (isSelecting)
        {
            UpdateSelection(screenPos);
        }
    }

    private void OnInputEnd(Vector2 screenPos)
    {
        if (!isSelecting)
        {
            if (!didSelectUnit && selectedUnits.Count > 0)
                HandleMoveCommand(screenPos);
            return;
        }

        selectionAreaTransform.gameObject.SetActive(false);
        isSelecting = false;

        Vector2 worldStart = mainCamera.ScreenToWorldPoint(inputStartPos);
        Vector2 worldEnd = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 min = Vector2.Min(worldStart, worldEnd);
        Vector2 max = Vector2.Max(worldStart, worldEnd);

        Physics2D.SyncTransforms();

        DeselectAllUnits();

        foreach (var hit in Physics2D.OverlapAreaAll(min, max))
        {
            if (hit.CompareTag(UnitTag) && hit.TryGetComponent(out UnitController unit))
            {
                unit.SetSelected(true);
                selectedUnits.Add(unit);
            }
        }
    }

    private void UpdateSelection(Vector2 screenPos)
    {
        Vector2 worldStart = mainCamera.ScreenToWorldPoint(inputStartPos);
        Vector2 worldEnd = mainCamera.ScreenToWorldPoint(screenPos);
        Vector2 min = Vector2.Min(worldStart, worldEnd);
        Vector2 max = Vector2.Max(worldStart, worldEnd);

        selectionAreaTransform.position = min;
        selectionAreaTransform.localScale = max - min;
    }

    private void DeselectAllUnits()
    {
        foreach (var unit in selectedUnits)
            unit?.SetSelected(false);
        selectedUnits.Clear();
    }

    private void HandleMoveCommand(Vector2 screenPos)
    {
        if (selectedUnits.Count == 0) return;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        Vector3 dest = new Vector3(worldPos.x, worldPos.y, 0f);

        // Configuration: define grid size
        int count = selectedUnits.Count;
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt((float)count / cols);
        float spacing = 0.5f; // Adjust spacing as needed

        // Compute the top-left origin so formation is centered on 'dest'
        Vector3 origin = dest - new Vector3((cols - 1) * spacing / 2f, (rows - 1) * spacing / 2f, 0f);

        clickVisual.transform.position = dest;
        clickVisual.SetActive(true);

        selectedUnits.RemoveAll(u => u == null);

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;

            Vector3 offset = new Vector3(col * spacing, row * spacing, 0f);

            float noiseRange = 0.2f; // You can tweak this
            Vector3 noise = new Vector3(Random.Range(-noiseRange, noiseRange), Random.Range(-noiseRange, noiseRange), 0f);

            Vector3 finalPos = origin + offset + noise;
            selectedUnits[i].MoveTo(finalPos);
        }

        DeselectAllUnits();
    }

}
