using System;
using System.Collections.Generic;

/// <summary>
/// Matches GET /api/v1/rooms/{code} — room data is nested under "room".
/// </summary>
[Serializable]
public class RoomApiWrapper
{
    public string timestamp;
    public RoomApiResponse room;
}

[Serializable]
public class RoomApiResponse
{
    public string code;
    public string name;
    public int capacity;
    public string type;
    public string status;           // "available" | "occupied" | "upcoming" | "maintenance"
    public EventData current_event;
    public EventData next_event;
    public List<EventData> schedule_today;
}

[Serializable]
public class EventData
{
    public string title;
    public string start;    // ISO 8601 e.g. "2025-01-23T14:00:00Z"
    public string end;
    public string organizer;

    public bool IsValid() => !string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(organizer);
}
