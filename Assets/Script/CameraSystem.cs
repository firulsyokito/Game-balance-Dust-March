using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraSystem : MonoBehaviour
{
    [Header("Movement Settings")]
    public int touchesNumber = 2;
    public float moveSpeed = 15f;
    public float followSpeed = 10f;
    public float edgeThreshold = 10f;

    [Header("Bounds")]
    public float startPosition = 0f;
    public float endPosition = 36f;

    private Vector3 targetPosition;
    private Vector2 lastTouchPosition;
    private Vector2 lastMousePosition;

    private bool isTouchDragging;
    private bool isMouseDragging;

    private void Awake() => EnhancedTouchSupport.Enable();
    private void OnDestroy() => EnhancedTouchSupport.Disable();

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!Application.isFocused) return;

        Vector3 moveDir = Vector3.zero;

        bool touchUsed = ProcessTouchInput();
        bool mouseUsed = ProcessMouseDrag();

        ProcessKeyboardOrEdgeInput(ref moveDir);

        targetPosition += moveDir * moveSpeed * Time.deltaTime;
        targetPosition.x = Mathf.Clamp(targetPosition.x, startPosition, endPosition);

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private bool ProcessTouchInput()
    {
        var touches = Touch.activeTouches;
        if (touches.Count != touchesNumber)
        {
            isTouchDragging = false;
            return false;
        }

        var touch = touches[0];
        Vector2 currentPos = touch.screenPosition;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                lastTouchPosition = currentPos;
                isTouchDragging = true;
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                Vector2 delta = touch.delta;
                float normalizedDeltaX = delta.x / Screen.width;
                float moveAmount = -normalizedDeltaX * moveSpeed;
                targetPosition.x += moveAmount;
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isTouchDragging = false;
                break;
        }

        return true;
    }

    private bool ProcessMouseDrag()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.rightButton.isPressed)
        {
            isMouseDragging = false;
            return false;
        }

        Vector2 currentPos = mouse.position.ReadValue();
        if (!isMouseDragging)
        {
            lastMousePosition = currentPos;
            isMouseDragging = true;
            return true;
        }

        Vector2 delta = currentPos - lastMousePosition;
        float normalizedDeltaX = delta.x / Screen.width;
        float moveAmount = -normalizedDeltaX * moveSpeed;

        targetPosition.x += moveAmount;
        lastMousePosition = currentPos;

        return true;
    }

    private void ProcessKeyboardOrEdgeInput(ref Vector3 moveDir)
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (kb != null)
        {
            if (kb.aKey.isPressed) moveDir.x -= 1;
            if (kb.dKey.isPressed) moveDir.x += 1;
        }

        if (mouse != null)
        {
            float x = mouse.position.ReadValue().x;
            if (x <= edgeThreshold) moveDir.x -= 1;
            else if (x >= Screen.width - edgeThreshold) moveDir.x += 1;
        }
    }
}
