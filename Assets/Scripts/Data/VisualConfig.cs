using UnityEngine;

/// <summary>
/// ScriptableObject holding all visual/style settings for the scene.
/// Assign to SceneStyler and BuildingConfigSO.
/// Swap this asset to completely restyle for a different building.
/// </summary>
[CreateAssetMenu(menuName = "PlateformeViewer/Visual Config", fileName = "VisualConfig_")]
public class VisualConfig : ScriptableObject
{
    [Header("Scene Background")]
    [Tooltip("Camera clear color — deep navy for blueprint style")]
    public Color backgroundColor = new(0.051f, 0.106f, 0.165f); // #0D1B2A

    [Header("Ambient Lighting")]
    public Color ambientColor = new(0.04f, 0.07f, 0.11f);       // very dim cool blue
    [Range(0f, 2f)] public float ambientIntensity = 0.3f;

    [Header("Building Materials")]
    [Tooltip("Walls / container exteriors — cool white-blue")]
    public Material matWall;
    [Tooltip("Hangar exterior shell — dark slate")]
    public Material matShell;
    [Tooltip("Corridors and floor planes")]
    public Material matCorridor;
    [Tooltip("Default room body material (overridden at runtime by status color)")]
    public Material matRoomDefault;

    [Header("Room Status Colors")]
    public Color colorFree     = new(0.18f, 0.80f, 0.44f); // #2ECC71
    public Color colorOccupied = new(0.91f, 0.30f, 0.24f); // #E74C3C
    public Color colorUpcoming = new(0.95f, 0.61f, 0.07f); // #F39C12
    public Color colorUnknown  = new(0.50f, 0.55f, 0.55f); // #7F8C8D

    [Header("UI Colors")]
    public Color uiBackground      = new(0.051f, 0.106f, 0.165f); // panel bg
    public Color uiTextPrimary     = Color.white;
    public Color uiTextMuted       = new(0.56f, 0.64f, 0.69f);   // #8FA3B1
    public Color uiDivider         = new(0.118f, 0.176f, 0.239f); // #1E2D3D

    /// <summary>Returns the status color for a given status string.</summary>
    public Color GetStatusColor(string status) => status switch
    {
        "free"     => colorFree,
        "occupied" => colorOccupied,
        "upcoming" => colorUpcoming,
        _          => colorUnknown
    };
}
