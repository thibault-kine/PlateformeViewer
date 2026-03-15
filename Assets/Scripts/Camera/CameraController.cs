using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Free-flight hover camera.
///
/// Controls:
///   WASD / Arrow keys   — move forward / back / strafe
///   Q / E               — move down / up
///   Right-click + drag  — look around
///   Scroll wheel        — dolly forward/back
///   Left Shift          — speed boost
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] BuildingConfigSO config;
    [SerializeField] BoxCollider boundsCollider;

    float _pitch, _yaw;

    // Set by MobileInputManager each frame ([-1..1] normalised axes)
    public Vector2 JoystickMove { get; set; }
    public Vector2 JoystickLook { get; set; }

    void Start()
    {
        if (config != null)
        {
            transform.position = config.cameraStartPosition;
            transform.eulerAngles = config.cameraStartRotation;
        }

        var euler = transform.eulerAngles;
        _pitch = euler.x;
        _yaw   = euler.y;
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;   
        Vector3 min = boundsCollider.bounds.min;
        Vector3 max = boundsCollider.bounds.max;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        pos.z = Mathf.Clamp(pos.z, min.z, max.z);

        transform.position = pos;
    }

    void HandleLook()
    {
        // ── Joystick look (mobile) ────────────────────────────────────────────
        if (JoystickLook.sqrMagnitude > 0.001f)
        {
            float sens      = config != null ? config.mouseSensitivity : 1.5f;
            float lookSpeed = 120f; // degrees per second at full deflection
            _yaw   += JoystickLook.x * lookSpeed * sens * Time.deltaTime;
            _pitch -= JoystickLook.y * lookSpeed * sens * Time.deltaTime;
            _pitch  = Mathf.Clamp(_pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        // ── Mouse look (desktop) ──────────────────────────────────────────────
        var mouse = Mouse.current;
        if (mouse == null) return;

        // Hide cursor while looking; avoid CursorLockMode in WebGL — the Pointer Lock
        // API takes 1-2 frames to acquire and causes delta spikes / stutter.
        if (mouse.rightButton.wasPressedThisFrame)
            Cursor.visible = false;
        if (mouse.rightButton.wasReleasedThisFrame)
            Cursor.visible = true;

        if (mouse.rightButton.isPressed && !mouse.rightButton.wasPressedThisFrame)
        {
            // Skip delta on the press frame: the browser accumulates movement
            // since the click, which would produce a large unwanted jump.
            var delta = mouse.delta.ReadValue();
            float sens = config != null ? config.mouseSensitivity : 1.5f;
            _yaw   += delta.x * sens;
            _pitch -= delta.y * sens;
            _pitch  = Mathf.Clamp(_pitch, -89f, 89f);
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        // Scroll to dolly
        float scroll = mouse.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float speed = config != null ? config.scrollSpeed : 20f;
            transform.position += transform.forward * (scroll * speed * Time.deltaTime);
        }
    }

    void HandleMovement()
    {
        float baseSpeed = config != null ? config.cameraSpeed : 10f;

        // ── Joystick movement (mobile) ────────────────────────────────────────
        if (JoystickMove.sqrMagnitude > 0.001f)
        {
            Vector3 moveDir = transform.forward * JoystickMove.y
                            + transform.right   * JoystickMove.x;
            // Keep movement horizontal (no altitude drift from camera tilt)
            moveDir.y = 0f;
            if (moveDir.sqrMagnitude > 0.001f)
                transform.position += moveDir.normalized
                                    * (JoystickMove.magnitude * baseSpeed * Time.deltaTime);
        }

        // ── Keyboard movement (desktop) ───────────────────────────────────────
        var kb = Keyboard.current;
        if (kb == null) return;

        float boost = config != null ? config.cameraBoostMultiplier : 3f;
        float speed = baseSpeed * (kb.shiftKey.isPressed ? boost : 1f);

        Vector3 dir = Vector3.zero;

        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    dir += transform.forward;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  dir -= transform.forward;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) dir += transform.right;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  dir -= transform.right;
        if (kb.eKey.isPressed)                               dir += Vector3.up;
        if (kb.qKey.isPressed)                               dir -= Vector3.up;

        if (dir.sqrMagnitude > 0.001f)
            transform.position += dir.normalized * (speed * Time.deltaTime);
    }
}
