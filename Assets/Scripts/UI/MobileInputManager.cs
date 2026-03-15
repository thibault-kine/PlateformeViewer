using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

/// <summary>
/// Manages dual virtual joysticks + altitude buttons for mobile WebGL.
///
/// - Left-half touches  → movement joystick  → CameraController.JoystickMove
/// - Right-half touches → look joystick      → CameraController.JoystickLook
/// - ▲ / ▼ hold-buttons (bottom-right)       → CameraController.MobileAscend/Descend
/// - Short taps (< 0.18s, < 18px)            → BuildingManager.HandleTap()
///
/// Attach to any persistent GameObject. All UI is created from code.
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

    const float TapMaxTime  = 0.18f;
    const float TapMaxMove  = 18f;
    const float JoyHoldTime = 0.12f;

    readonly Dictionary<int, PendingTouch> _pending = new();

    int _moveFingerIdx = -1;
    int _lookFingerIdx = -1;

    // ── References ────────────────────────────────────────────────────────────

    VirtualJoystick  _moveJoy;
    VirtualJoystick  _lookJoy;
    HoldButton       _ascendBtn;
    HoldButton       _descendBtn;
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
        if (Touchscreen.current == null)
        {
            enabled = false;
            return;
        }

        _cam = FindFirstObjectByType<CameraController>();
        _bm  = FindFirstObjectByType<BuildingManager>();

        CreateMobileCanvas();
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

        if (_cam != null)
        {
            _cam.JoystickMove  = _moveJoy.IsActive  ? _moveJoy.Direction  : Vector2.zero;
            _cam.JoystickLook  = _lookJoy.IsActive  ? _lookJoy.Direction  : Vector2.zero;
            _cam.MobileAscend  = _ascendBtn.IsHeld;
            _cam.MobileDescend = _descendBtn.IsHeld;
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
                // If this touch started on a UI element (altitude buttons), let
                // EventSystem handle it exclusively — don't activate a joystick.
                if (IsTouchOverUI(touch.screenPosition)) return;

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

                float moved      = Vector2.Distance(touch.screenPosition, pt.startPos);
                float held       = Time.unscaledTime - pt.startTime;
                bool  shouldClaim = !pt.claimedByJoystick &&
                                    (moved > TapMaxMove || held > JoyHoldTime);

                if (shouldClaim)
                {
                    if (isLeft && _moveFingerIdx == -1)
                    {
                        _moveFingerIdx       = idx;
                        pt.claimedByJoystick = true;
                        _pending[idx]        = pt;
                        _moveJoy.Activate(pt.startPos);
                    }
                    else if (!isLeft && _lookFingerIdx == -1)
                    {
                        _lookFingerIdx       = idx;
                        pt.claimedByJoystick = true;
                        _pending[idx]        = pt;
                        _lookJoy.Activate(pt.startPos);
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
        if (EventSystem.current != null)
        {
            var ped     = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            if (results.Count > 0) return;
        }
        _bm.HandleTap(screenPos);
    }

    // ── UI touch check ────────────────────────────────────────────────────────

    bool IsTouchOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;
        var ped     = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);
        return results.Count > 0;
    }

    // ── Canvas / UI creation ──────────────────────────────────────────────────

    void CreateMobileCanvas()
    {
        var go = new GameObject("MobileJoystickCanvas");
        DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        float dpi = Screen.dpi > 10f ? Screen.dpi : 160f;
        scaler.scaleFactor = Mathf.Clamp(dpi / 160f, 1f, 3f);

        // GraphicRaycaster required so altitude buttons receive EventSystem pointer events
        go.AddComponent<GraphicRaycaster>();

        _moveJoy    = CreateJoystick(go.transform, "MoveJoystick");
        _lookJoy    = CreateJoystick(go.transform, "LookJoystick");
        _ascendBtn  = CreateAltitudeButton(go.transform, "▲", 1);
        _descendBtn = CreateAltitudeButton(go.transform, "▼", 0);
    }

    // ── Joystick factory ──────────────────────────────────────────────────────

    static VirtualJoystick CreateJoystick(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = Vector2.zero;
        return go.AddComponent<VirtualJoystick>();
    }

    // ── Altitude button factory ───────────────────────────────────────────────

    /// <param name="stackIndex">0 = bottom button (▼), 1 = top button (▲)</param>
    static HoldButton CreateAltitudeButton(Transform parent, string arrow, int stackIndex)
    {
        const float size    = 64f;
        const float margin  = 24f;  // from screen edge
        const float spacing = 10f;

        var go = new GameObject("AltBtn_" + arrow);
        go.transform.SetParent(parent, false);

        var rt = go.AddComponent<RectTransform>();
        // Anchor: bottom-right corner
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot     = new Vector2(1f, 0f);
        rt.sizeDelta = new Vector2(size, size);
        float yOffset = margin + stackIndex * (size + spacing);
        rt.anchoredPosition = new Vector2(-margin, yOffset);

        // Background circle
        var img = go.AddComponent<Image>();
        img.sprite        = CreateCircleSprite(96,
            new Color(0.01f, 0.05f, 0.14f, 0.82f),
            new Color(0f, 0.70f, 1f, 0.65f), 6f);
        img.raycastTarget = true;

        // Arrow label
        var textGO = new GameObject("Label");
        textGO.transform.SetParent(go.transform, false);
        var textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;

        var text = textGO.AddComponent<Text>();
        text.text           = arrow;
        text.font           = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize       = 28;
        text.fontStyle      = FontStyle.Bold;
        text.alignment      = TextAnchor.MiddleCenter;
        text.color          = new Color(0f, 0.80f, 1f, 0.90f);
        text.raycastTarget  = false;

        return go.AddComponent<HoldButton>();
    }

    // ── Circle sprite (shared with VirtualJoystick) ───────────────────────────

    static Sprite CreateCircleSprite(int res, Color fill, Color border, float borderPx)
    {
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float c = (res - 1) * 0.5f;
        float r = c;
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            float dist = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
            Color col;
            if      (dist > r)            col = Color.clear;
            else if (dist > r - borderPx) col = border;
            else                          col = fill;
            tex.SetPixel(x, y, col);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }
}
