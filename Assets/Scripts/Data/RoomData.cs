using System;
using System.Collections.Generic;

/// <summary>
/// Top-level response from GET /rooms/{code}
/// </summary>
[Serializable]
public class RoomApiResponse
{
    public string timestamp;
    public RoomDetail room;
}

[Serializable]
public class RoomDetail
{
    public string code;
    public string name;
    public int capacity;
    public string type;
    public string status;           // "occupied" | "free"
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

    public bool IsValid() => !string.IsNullOrEmpty(title);
}
