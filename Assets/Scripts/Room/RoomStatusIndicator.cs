using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Updates a room mesh's material color to reflect its booking status.
/// Supports multiple renderers (e.g. when the room has several Cube children).
/// If a VisualConfig is assigned, colors come from there — otherwise falls back to hardcoded defaults.
/// </summary>
public class RoomStatusIndicator : MonoBehaviour
{
    [SerializeField] List<Renderer> targetRenderers = new();
    [SerializeField] int materialIndex = 0;
    [SerializeField] VisualConfig visualConfig;

    // Hardcoded fallbacks (match DESIGN_DECISIONS.md) used when no VisualConfig is assigned
    static readonly Color ColorFree     = new(0.18f, 0.80f, 0.44f); // #2ECC71
    static readonly Color ColorOccupied = new(0.91f, 0.30f, 0.24f); // #E74C3C
    static readonly Color ColorUpcoming = new(0.95f, 0.61f, 0.07f); // #F39C12
    static readonly Color ColorUnknown  = new(0.50f, 0.55f, 0.55f); // #7F8C8D

    readonly List<Material> _runtimeMats  = new();
    readonly List<Color>   _originalColors = new();

    // Not serialized — always reads from code so stale scene values can't override it
    const float StatusBlend     = 0.5f;  // 75 % status color, 25 % original
    const float EmissionBoost   = 1f;   // HDR-range emission for URP / Linear colour space

    void Awake()
    {
        // Fallback: if no renderers assigned, try this GameObject itself
        if (targetRenderers.Count == 0)
        {
            var r = GetComponent<Renderer>();
            if (r != null) targetRenderers.Add(r);
        }

        // Clone one material per renderer so we don't modify shared assets
        foreach (var renderer in targetRenderers)
        {
            if (renderer == null) continue;
            var mats = renderer.materials;
            var cloned = new Material(mats[materialIndex]);
            mats[materialIndex] = cloned;
            renderer.materials = mats;
            _runtimeMats.Add(cloned);
            _originalColors.Add(cloned.color); // remember original albedo
        }
    }

    public void SetStatus(string status)
    {
        if (_runtimeMats.Count == 0) return;

        Color statusColor = visualConfig != null
            ? visualConfig.GetStatusColor(status)
            : status switch
            {
                "free"     => ColorFree,
                "occupied" => ColorOccupied,
                "upcoming" => ColorUpcoming,
                _          => ColorUnknown
            };

        for (int i = 0; i < _runtimeMats.Count; i++)
        {
            var mat = _runtimeMats[i];
            if (mat == null) continue;

            Color original = i < _originalColors.Count ? _originalColors[i] : Color.white;

            // Blend original material colour with status colour
            mat.color = Color.Lerp(original, statusColor, StatusBlend);

            // Emission — must set globalIlluminationFlags for URP / WebGL linear colour space
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", statusColor * EmissionBoost);
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
        }
    }

    void OnDestroy()
    {
        foreach (var mat in _runtimeMats)
            if (mat != null) Destroy(mat);
    }

    public void SetRenderers(List<Renderer> renderers)
    {
        targetRenderers = renderers;
    }

    public void SetMaterialIndex(int index)
    {
        materialIndex = index;
    }

    public void SetVisualConfig(VisualConfig config)
    {
        visualConfig = config;
    }
}
