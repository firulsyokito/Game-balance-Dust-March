using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraSystem : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 15f;
    public float smoothTime = 0.1f;
    public float edgeThreshold = 10f;

    [Header("Bounds")]
    public float startPosition = 0f;
    public float endPosition = 36f;

    private Vector3 targetPosition;
    private Vector2 lastTouchPosition;
    private Vector2 lastMousePosition;

    private float velocityX;
    private bool isTouchDragging;
    private bool isMouseDragging;

    private enum InputMethod { None, Touch, MouseDrag, KeyboardOrMouseEdge }
    private InputMethod activeInput = InputMethod.None;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!Application.isFocused) return;

        Vector3 moveDir = Vector3.zero;
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        bool hasKeyboard = keyboard != null && keyboard.enabled;
        bool hasMouse = mouse != null && mouse.enabled;

        if (TryHandleTouchInput())
        {
            SetInputMethod(InputMethod.Touch);
        }
        else if (hasMouse && mouse.rightButton.isPressed)
        {
            SetInputMethod(InputMethod.MouseDrag);
        }
        else if (IsKeyboardOrEdgeInput(hasKeyboard, hasMouse, keyboard, mouse))
        {
            SetInputMethod(InputMethod.KeyboardOrMouseEdge);
        }
        else
        {
            SetInputMethod(InputMethod.None);
        }

        HandleInput(ref moveDir);

        if (activeInput == InputMethod.KeyboardOrMouseEdge)
        {
            targetPosition.x += moveDir.x * moveSpeed * Time.deltaTime;
        }
        else
        {
            targetPosition.x += velocityX * Time.deltaTime;
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, startPosition, endPosition);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.15f);
    }

    private void SetInputMethod(InputMethod method)
    {
        if (activeInput != method)
        {
            activeInput = method;
            velocityX = 0f;
            isTouchDragging = false;
            isMouseDragging = false;
        }
    }

    private void HandleInput(ref Vector3 moveDir)
    {
        switch (activeInput)
        {
            case InputMethod.Touch:
                ProcessTouchDrag();
                break;
            case InputMethod.MouseDrag:
                ProcessMouseDrag();
                break;
            case InputMethod.KeyboardOrMouseEdge:
                ProcessKeyboardOrMouseEdgeInput(ref moveDir);
                break;
            default:
                velocityX = Mathf.Lerp(velocityX, 0f, smoothTime * Time.deltaTime);
                break;
        }
    }

    private bool TryHandleTouchInput()
    {
        if (Touch.activeTouches.Count == 0) return false;
        var touch = Touch.activeTouches[0];
        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPosition = touch.screenPosition;
            isTouchDragging = true;
            velocityX = 0f;
            return false; // don't immediately short-circuit; wait for movement
        }
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isTouchDragging = false;
            return false;
        }
        return isTouchDragging;
    }

    private void ProcessTouchDrag()
    {
        if (!isTouchDragging || Touch.activeTouches.Count == 0) return;
        var touch = Touch.activeTouches[0];
        if (touch.phase == TouchPhase.Moved)
        {
            float dt = Mathf.Max(Time.deltaTime, 0.001f);
            velocityX = -touch.delta.x / dt;
            lastTouchPosition = touch.screenPosition;
        }
    }
    
    private void ProcessMouseDrag()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.rightButton.isPressed)
        {
            isMouseDragging = false;
            activeInput = InputMethod.None;
            return;
        }

        Vector2 currentPos = mouse.position.ReadValue();
        if (!isMouseDragging)
        {
            lastMousePosition = currentPos;
            velocityX = 0f;
            isMouseDragging = true;
        }

        Vector2 delta = currentPos - lastMousePosition;
        float dt = Mathf.Max(Time.deltaTime, 0.001f);
        velocityX = -delta.x / dt * 0.01f;
        lastMousePosition = currentPos;
    }

    private void ProcessKeyboardOrMouseEdgeInput(ref Vector3 moveDir)
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) moveDir.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveDir.x += 1;
        }

        if (Mouse.current != null)
        {
            float x = Mouse.current.position.ReadValue().x;
            if (x <= edgeThreshold) moveDir.x -= 1;
            else if (x >= Screen.width - edgeThreshold) moveDir.x += 1;
        }
    }

    private bool IsKeyboardOrEdgeInput(bool hasKeyboard, bool hasMouse, Keyboard keyboard, Mouse mouse)
    {
        return (hasKeyboard && (keyboard.aKey.isPressed || keyboard.dKey.isPressed)) ||
               (hasMouse && (mouse.position.ReadValue().x <= edgeThreshold ||
                             mouse.position.ReadValue().x >= Screen.width - edgeThreshold));
    }
}
