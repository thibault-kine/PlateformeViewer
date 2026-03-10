using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject holding live data for one room.
/// One asset per room. Updated by ApiClient at runtime.
/// </summary>
[CreateAssetMenu(menuName = "PlateformeViewer/Room Data", fileName = "Room_")]
public class RoomDataSO : ScriptableObject
{
    [Header("Room Identity — set once")]
    public string roomCode;       // e.g. "Vert-01"  — must match API code
    public string roomName;       // e.g. "Vert-01"
    public int capacity;
    public string roomType;       // e.g. "Salle de travail"

    [Header("Live Data — updated at runtime")]
    public string status = "unknown";
    public string lastUpdated;
    public EventData currentEvent;
    public EventData nextEvent;
    public List<EventData> scheduleToday = new();

    public event Action OnDataUpdated;

    /// <summary>Called by ApiClient after a successful fetch.</summary>
    public void UpdateFromResponse(RoomApiResponse response)
    {
        if (response?.room == null)
        {
            SetError();
            return;
        }

        var r = response.room;
        status       = string.IsNullOrEmpty(r.status) ? "unknown" : r.status;
        lastUpdated  = response.timestamp;
        currentEvent = r.current_event;
        nextEvent    = r.next_event;
        scheduleToday = r.schedule_today ?? new List<EventData>();

        // Derive "upcoming" if next event starts within 30 minutes
        if (status == "free" && nextEvent != null && nextEvent.IsValid())
        {
            if (DateTime.TryParse(nextEvent.start, out DateTime nextStart))
            {
                var diff = nextStart - DateTime.UtcNow;
                if (diff.TotalMinutes is > 0 and <= 30)
                    status = "upcoming";
            }
        }

        OnDataUpdated?.Invoke();
    }

    /// <summary>Called on fetch error — keeps last data but marks unknown.</summary>
    public void SetError()
    {
        status = "unknown";
        OnDataUpdated?.Invoke();
    }
}
