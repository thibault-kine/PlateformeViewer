using UnityEngine;

/// <summary>
/// Main component on each room GameObject.
/// Links a RoomDataSO to the visual indicator and handles click events.
///
/// Setup per room:
///   1. Attach this component to the room mesh GameObject
///   2. Assign the matching RoomDataSO in the Inspector
///   3. Assign the RoomStatusIndicator (can be on the same or child object)
///   4. Ensure the GameObject has a Collider (MeshCollider or BoxCollider)
///   5. Set the layer to "Room" (create this layer in Unity)
/// </summary>
public class RoomController : MonoBehaviour
{
    [SerializeField] public RoomDataSO roomData;
    [SerializeField] RoomStatusIndicator statusIndicator;

    void OnEnable()
    {
        if (roomData != null)
            roomData.OnDataUpdated += OnDataUpdated;
    }

    void OnDisable()
    {
        if (roomData != null)
            roomData.OnDataUpdated -= OnDataUpdated;
    }

    void Start()
    {
        // Apply initial status from whatever is in the SO (mock or last known)
        if (roomData != null) OnDataUpdated();
    }

    void OnDataUpdated()
    {
        statusIndicator?.SetStatus(roomData.status);
    }

    /// <summary>Called by BuildingManager when this room is clicked.</summary>
    public void OnRoomClicked()
    {
        RoomInfoPanel.Instance?.Show(roomData);
    }
}
