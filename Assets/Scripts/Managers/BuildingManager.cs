using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Scene-level manager. Handles:
///   - Room click detection via raycast
///   - Auto-refresh coroutine
///   - Mock data loading
///
/// Attach to a Manager GameObject in the scene.
/// Assign BuildingConfigSO and set the Room layer mask.
/// </summary>
public class BuildingManager : MonoBehaviour
{
    [SerializeField] BuildingConfigSO config;
    [SerializeField] LayerMask roomLayerMask;

    Camera _cam;

    /// <summary>Exposes config to UnityBridge for runtime tuning.</summary>
    public BuildingConfigSO GetConfig() => config;

    void Start()
    {
        _cam = Camera.main;

        if (config == null)
        {
            Debug.LogError("[BuildingManager] BuildingConfigSO not assigned.");
            return;
        }

        if (config.useMockData)
            LoadMockData();

        if (config.refreshInterval > 0f)
            StartCoroutine(AutoRefresh());
    }

    void Update()
    {
        HandleClick();
    }

    // ── Click detection ──────────────────────────────────────────────────────

    void HandleClick()
    {
        // On mobile, MobileInputManager intercepts all touches and calls HandleTap() directly.
        // Skip mouse-based detection to avoid double-firing on touch-simulated mouse events.
        if (MobileInputManager.Instance != null) return;

        var mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

        // Ignore clicks on UI elements
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()) return;

        RaycastRoom(_cam.ScreenPointToRay(mouse.position.ReadValue()));
    }

    /// <summary>Called by MobileInputManager when a short tap is detected (not a joystick drag).</summary>
    public void HandleTap(Vector2 screenPos)
    {
        // Check if tap hit any Unity UI canvas element
        if (EventSystem.current != null)
        {
            var ped     = new PointerEventData(EventSystem.current) { position = screenPos };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            if (results.Count > 0) return;
        }

        RaycastRoom(_cam.ScreenPointToRay(screenPos));
    }

    void RaycastRoom(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, roomLayerMask))
        {
            var room = hit.collider.GetComponentInParent<RoomController>()
                    ?? hit.collider.GetComponent<RoomController>();
            room?.OnRoomClicked();
        }
        else
        {
            RoomInfoPanel.Instance?.Hide();
        }
    }

    // ── Auto-refresh ─────────────────────────────────────────────────────────

    IEnumerator AutoRefresh()
    {
        while (true)
        {
            yield return new WaitForSeconds(config.refreshInterval);
            RefreshAll();
        }
    }

    public void RefreshAll()
    {
        if (config.useMockData) return;
        foreach (var room in config.rooms)
            FetchRoom(room);
    }

    void FetchRoom(RoomDataSO room)
    {
        if (ApiClient.Instance == null) return;

        ApiClient.Instance.FetchRoom(
            config.apiBaseUrl,
            room.roomCode,
            resp => room.UpdateFromResponse(resp),
            err =>
            {
                // API 1 failed — try API 2
                if (!string.IsNullOrEmpty(config.apiBaseUrl2))
                {
                    ApiClient.Instance.FetchRoom(
                        config.apiBaseUrl2,
                        room.roomCode,
                        resp2 => room.UpdateFromResponse(resp2),
                        err2 =>
                        {
                            Debug.LogWarning($"[BuildingManager] Both APIs failed for {room.roomCode}: {err2}");
                            room.SetError();
                        });
                }
                else
                {
                    room.SetError();
                }
            });
    }

    // ── Mock data ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Loads mock JSON from Resources/MockData/{roomCode}.json for each room.
    /// Falls back gracefully if the file doesn't exist.
    /// </summary>
    void LoadMockData()
    {
        foreach (var room in config.rooms)
        {
            var asset = Resources.Load<TextAsset>($"MockData/{room.roomCode}");
            if (asset == null)
            {
                room.SetError();
                continue;
            }

            try
            {
                var response = JsonUtility.FromJson<RoomApiResponse>(asset.text);
                room.UpdateFromResponse(response);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[BuildingManager] Mock data parse error for {room.roomCode}: {e.Message}");
                room.SetError();
            }
        }
    }
}
