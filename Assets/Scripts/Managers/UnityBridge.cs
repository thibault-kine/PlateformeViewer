using System.Globalization;
using UnityEngine;

/// <summary>
/// React → Unity message bridge.
///
/// Attach to a GameObject named EXACTLY "UnityBridge" in the scene.
/// React calls: sendMessage("UnityBridge", "MethodName", "value")
///
/// All public methods accept a single string parameter (react-unity-webgl
/// only supports string | number across all bridge versions).
/// float.TryParse uses InvariantCulture so "1.5" works regardless of
/// the browser's locale (French browsers use "1,5" with default culture).
/// </summary>
public class UnityBridge : MonoBehaviour
{
    BuildingConfigSO       _config;
    SceneStyler            _styler;
    RoomStatusIndicator[]  _indicators;

    void Awake()
    {
        // Resolve BuildingConfigSO through BuildingManager
        var manager = FindFirstObjectByType<BuildingManager>();
        if (manager != null)
            _config = manager.GetConfig();
        else
            Debug.LogWarning("[UnityBridge] BuildingManager not found — camera settings unavailable.");

        _styler = FindFirstObjectByType<SceneStyler>();
        if (_styler == null)
            Debug.LogWarning("[UnityBridge] SceneStyler not found — lighting settings unavailable.");

        _indicators = FindObjectsByType<RoomStatusIndicator>(FindObjectsSortMode.None);
        Debug.Log($"[UnityBridge] Ready. {_indicators.Length} room indicators found.");
    }

    // ── Camera ───────────────────────────────────────────────────────────────

    public void SetMouseSensitivity(string value)
    {
        if (_config == null) return;
        if (TryParseFloat(value, out float f)) _config.mouseSensitivity = f;
    }

    public void SetCameraSpeed(string value)
    {
        if (_config == null) return;
        if (TryParseFloat(value, out float f)) _config.cameraSpeed = f;
    }

    public void SetScrollSpeed(string value)
    {
        if (_config == null) return;
        if (TryParseFloat(value, out float f)) _config.scrollSpeed = f;
    }

    // ── Lighting ─────────────────────────────────────────────────────────────

    public void SetAmbientIntensity(string value)
    {
        if (_styler == null) return;
        if (TryParseFloat(value, out float f)) _styler.SetAmbientIntensity(f);
    }

    // ── Room visuals ──────────────────────────────────────────────────────────

    public void SetStatusVisible(string value)
    {
        bool visible = value.ToLowerInvariant() == "true";
        foreach (var indicator in _indicators)
            indicator?.SetStatusVisible(visible);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static bool TryParseFloat(string value, out float result) =>
        float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
}
