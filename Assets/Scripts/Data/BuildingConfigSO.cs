using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Building-level configuration ScriptableObject.
/// Swap this asset to use the system for a different building — no code changes needed.
/// </summary>
[CreateAssetMenu(menuName = "PlateformeViewer/Building Config", fileName = "BuildingConfig_")]
public class BuildingConfigSO : ScriptableObject
{
    [Header("Building Info")]
    public string buildingName = "La Plateforme";

    [Header("API")]
    [Tooltip("Base URL for team API 1 — e.g. https://team1.com/api/v1")]
    public string apiBaseUrl;
    [Tooltip("Base URL for team API 2 — fallback if API 1 fails")]
    public string apiBaseUrl2;

    [Header("Data")]
    [Tooltip("Use local JSON files in Resources/MockData/ instead of live API")]
    public bool useMockData = true;
    [Tooltip("Auto-refresh interval in seconds. 0 = manual only.")]
    public float refreshInterval = 60f;

    [Header("Camera Defaults")]
    public float cameraSpeed = 10f;
    public float cameraBoostMultiplier = 3f;
    public float mouseSensitivity = 1.5f;
    public float scrollSpeed = 20f;
    public Vector3 cameraStartPosition = new(0f, 30f, -20f);
    public Vector3 cameraStartRotation = new(45f, 0f, 0f);

    [Header("Visual Style")]
    public VisualConfig visualConfig;

    [Header("Rooms")]
    [Tooltip("All RoomDataSO assets for this building")]
    public List<RoomDataSO> rooms = new();
}
