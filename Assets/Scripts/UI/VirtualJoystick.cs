using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single virtual joystick. Programmatically creates its own circle visuals.
/// Positioned at touch origin; knob moves within MaxDrag pixels.
/// </summary>
public class VirtualJoystick : MonoBehaviour
{
    // ── Config ────────────────────────────────────────────────────────────────
    const float BgSize   = 130f;  // background circle diameter (canvas pixels)
    const float KnobSize = 56f;   // knob circle diameter
    const float MaxDrag  = 55f;   // maximum knob displacement from center

    // ── Runtime ───────────────────────────────────────────────────────────────
    Canvas          _canvas;
    RectTransform   _root;        // anchored at touch position
    RectTransform   _bg;
    RectTransform   _knob;

    public Vector2 Direction { get; private set; }
    public bool    IsActive  { get; private set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _root   = (RectTransform)transform;
        BuildVisuals();
        _bg.gameObject.SetActive(false);
    }

    void BuildVisuals()
    {
        Sprite bgSprite   = CreateCircleSprite(128, new Color(0.01f, 0.05f, 0.14f, 0.80f),
                                               new Color(0f, 0.70f, 1f, 0.70f), 8f);
        Sprite knobSprite = CreateCircleSprite(64,  new Color(0f, 0.75f, 1f, 0.85f),
                                               new Color(0f, 0.90f, 1f, 1f), 4f);

        // Background
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(_root, false);
        _bg = bgGO.AddComponent<RectTransform>();
        _bg.sizeDelta = Vector2.one * BgSize;
        _bg.anchoredPosition = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.sprite = bgSprite;
        bgImg.raycastTarget = false;

        // Knob
        var knobGO = new GameObject("Knob");
        knobGO.transform.SetParent(_root, false);
        _knob = knobGO.AddComponent<RectTransform>();
        _knob.sizeDelta = Vector2.one * KnobSize;
        _knob.anchoredPosition = Vector2.zero;
        var knobImg = knobGO.AddComponent<Image>();
        knobImg.sprite = knobSprite;
        knobImg.raycastTarget = false;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Activate(Vector2 screenPos)
    {
        _root.anchoredPosition = ScreenToCanvas(screenPos);
        _knob.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
        IsActive  = true;
        _bg.gameObject.SetActive(true);
        _knob.gameObject.SetActive(true);
    }

    public void UpdateInput(Vector2 screenPos)
    {
        if (!IsActive) return;

        Vector2 localPos  = ScreenToCanvas(screenPos);
        Vector2 delta     = localPos - _root.anchoredPosition;
        Vector2 clamped   = Vector2.ClampMagnitude(delta, MaxDrag);
        _knob.anchoredPosition = clamped;
        Direction = clamped / MaxDrag;  // [-1 .. 1] on each axis
    }

    public void Deactivate()
    {
        IsActive  = false;
        Direction = Vector2.zero;
        _bg.gameObject.SetActive(false);
        _knob.gameObject.SetActive(false);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    Vector2 ScreenToCanvas(Vector2 screenPos)
    {
        Camera cam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_canvas.transform, screenPos, cam, out Vector2 local);
        return local;
    }

    /// <summary>Procedurally generates a filled circle sprite with a solid border.</summary>
    static Sprite CreateCircleSprite(int res, Color fill, Color border, float borderPx)
    {
        var tex    = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float c = (res - 1) * 0.5f;
        float r = c;
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++)
        {
            float dist = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
            Color col;
            if      (dist > r)           col = Color.clear;
            else if (dist > r - borderPx) col = border;
            else                          col = fill;
            tex.SetPixel(x, y, col);
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, res, res), new Vector2(0.5f, 0.5f), res);
    }
}
