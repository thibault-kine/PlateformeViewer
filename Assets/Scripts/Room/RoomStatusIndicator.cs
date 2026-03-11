using UnityEngine;

/// <summary>
/// Updates a room mesh's material color to reflect its booking status.
/// Attach to the room GameObject and assign the Renderer in the Inspector.
/// If a VisualConfig is assigned, colors come from there — otherwise falls back to hardcoded defaults.
/// </summary>
public class RoomStatusIndicator : MonoBehaviour
{
    [SerializeField] Renderer targetRenderer;
    [SerializeField] int materialIndex = 0;
    [SerializeField] VisualConfig visualConfig;

    // Hardcoded fallbacks (match DESIGN_DECISIONS.md) used when no VisualConfig is assigned
    static readonly Color ColorFree     = new(0.18f, 0.80f, 0.44f); // #2ECC71
    static readonly Color ColorOccupied = new(0.91f, 0.30f, 0.24f); // #E74C3C
    static readonly Color ColorUpcoming = new(0.95f, 0.61f, 0.07f); // #F39C12
    static readonly Color ColorUnknown  = new(0.50f, 0.55f, 0.55f); // #7F8C8D

    Material _runtimeMat;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
        {
            // Clone the material so we don't modify the shared asset
            var mats = targetRenderer.materials;
            _runtimeMat = new Material(mats[materialIndex]);
            mats[materialIndex] = _runtimeMat;
            targetRenderer.materials = mats;
        }
    }

    public void SetStatus(string status)
    {
        if (_runtimeMat == null) return;

        Color c = visualConfig != null
            ? visualConfig.GetStatusColor(status)
            : status switch
            {
                "free"     => ColorFree,
                "occupied" => ColorOccupied,
                "upcoming" => ColorUpcoming,
                _          => ColorUnknown
            };

        _runtimeMat.color = c;

        // Soft emission so the color reads on the dark scene background
        if (_runtimeMat.IsKeywordEnabled("_EMISSION") ||
            _runtimeMat.HasProperty("_EmissionColor"))
        {
            _runtimeMat.EnableKeyword("_EMISSION");
            _runtimeMat.SetColor("_EmissionColor", c * 0.25f);
        }
    }

    void OnDestroy()
    {
        if (_runtimeMat != null) Destroy(_runtimeMat);
    }


    public void SetRenderer(Renderer renderer)
    {
        targetRenderer = renderer;
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
