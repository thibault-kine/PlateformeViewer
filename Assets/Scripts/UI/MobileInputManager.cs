using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Manages dual virtual joysticks for mobile WebGL.
///
/// - Left-half touches  → movement joystick  → CameraController.JoystickMove
/// - Right-half touches → look joystick      → CameraController.JoystickLook
/// - Short taps (< 0.18s, < 18px) that don't turn into joysticks
///   → forwarded to BuildingManager.HandleTap() for room selection
///
/// Attach to any persistent GameObject in the scene.
/// The entire Canvas + joystick UI is created from code — no prefab needed.
/// </summary>
public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }

    // ── Touch tracking ────────────────────────────────────────────────────────

    struct PendingTouch
    {
        public int     fingerIndex;
        public Vector2 startPos;
        public float   startTime;
        public bool    claimedByJoystick;
    }

    const float TapMaxTime  = 0.18f;  // seconds
    const float TapMaxMove  = 18f;    // pixels
    const float JoyHoldTime = 0.12f;  // seconds before activating joystick on held touch

    readonly Dictionary<int, PendingTouch> _pending = new();

    int _moveFingerIdx = -1;
    int _lookFingerIdx = -1;

    // ── References ────────────────────────────────────────────────────────────

    VirtualJoystick  _moveJoy;
    VirtualJoystick  _lookJoy;
    CameraController _cam;
    BuildingManager  _bm;

    bool _initialized;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        Instance = this;
        EnhancedTouchSupport.Enable();
    }

    void Start()
    {
        // Only activate on touch-capable devices
        if (Touchscreen.current == null)
        {
            enabled = false;
            return;
        }

        _cam = FindFirstObjectByType<CameraController>();
        _bm  = FindFirstObjectByType<BuildingManager>();

        CreateJoystickCanvas();
        _initialized = true;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        EnhancedTouchSupport.Disable();
    }

    // ── Update ────────────────────────────────────────────────────────────────

    void Update()
    {
        if (!_initialized) return;

        foreach (var touch in Touch.activeTouches)
            ProcessTouch(touch);

        // Feed directions to camera
        if (_cam != null)
        {
            _cam.JoystickMove = _moveJoy.IsActive ? _moveJoy.Direction : Vector2.zero;
            _cam.JoystickLook = _lookJoy.IsActive ? _lookJoy.Direction : Vector2.zero;
        }
    }

    // ── Touch routing ─────────────────────────────────────────────────────────

    void ProcessTouch(Touch touch)
    {
        int   idx    = touch.finger.index;
        float halfW  = Screen.width * 0.5f;
        bool  isLeft = touch.screenPosition.x < halfW;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                _pending[idx] = new PendingTouch
                {
                    fingerIndex       = idx,
                    startPos          = touch.screenPosition,
                    startTime         = Time.unscaledTime,
                    claimedByJoystick = false,
                };
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (!_pending.TryGetValue(idx, out var pt)) break;

                float moved   = Vector2.Distance(touch.screenPosition, pt.startPos);
                float held    = Time.unscaledTime - pt.startTime;
                bool  shouldClaim = !pt.claimedByJoystick &&
                                    (moved > TapMaxMove || held > JoyHoldTime);

                if (shouldClaim)
                {
                    if (isLeft && _moveFingerIdx == -1)
                    {
                        _moveFingerIdx = idx;
                        _moveJoy.Activate(pt.startPos);
                        pt.claimedByJoystick = true;
                        _pending[idx] = pt;
                    }
                    else if (!isLeft && _lookFingerIdx == -1)
                    {
                        _lookFingerIdx = idx;
                        _lookJoy.Activate(pt.startPos);
                        pt.claimedByJoystick = true;
                        _pending[idx] = pt;
                    }
                }

                if (idx == _moveFingerIdx) _moveJoy.UpdateInput(touch.screenPosition);
                if (idx == _lookFingerIdx) _lookJoy.UpdateInput(touch.screenPosition);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (_pending.TryGetValue(idx, out var endPt))
                {
                    if (!endPt.claimedByJoystick)
                        FireTap(endPt.startPos);

                    _pending.Remove(idx);
                }

                if (idx == _moveFingerIdx) { _moveFingerIdx = -1; _moveJoy.Deactivate(); }
                if (idx == _lookFingerIdx) { _lookFingerIdx = -1; _lookJoy.Deactivate(); }
                break;
        }
    }

    // ── Tap → room click ─────────────────────────────────────────────────────

    void FireTap(Vector2 screenPos)
    {
        if (_bm == null) return;

        // Check if tap landed on any Unity UI element
        if (EventSystem.current != null)
        {
            var ped     = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            if (results.Count > 0) return;
        }

        _bm.HandleTap(screenPos);
    }

    // ── Canvas / joystick creation ────────────────────────────────────────────

    void CreateJoystickCanvas()
    {
        var go     = new GameObject("MobileJoystickCanvas");
        DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;   // above Unity game UI but below RoomInfoPanel

        // DPI-aware scaling so joystick has consistent physical size
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        float dpi  = Screen.dpi > 10f ? Screen.dpi : 160f;
        scaler.scaleFactor = Mathf.Clamp(dpi / 160f, 1f, 3f);

        // No GraphicRaycaster — joystick canvas must not block EventSystem raycasts
        // (touches are handled via EnhancedTouch, not EventSystem)

        _moveJoy = CreateJoystick(go.transform, "MoveJoystick");
        _lookJoy = CreateJoystick(go.transform, "LookJoystick");
    }

    static VirtualJoystick CreateJoystick(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = Vector2.zero;
        return go.AddComponent<VirtualJoystick>();
    }
}
