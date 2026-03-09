using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class RoomsFile
{
    public string exportDate;
    public int totalRooms;
    public List<Room> rooms;


    public Room GetByID(string id)
    {
        return rooms.Find(room => room.id == id);
    }

    public Room GetByName(string name)
    {
        return rooms.Find(room => room.name == name);
    }


    public static RoomsFile FromJson(string json)
    {
        return JsonUtility.FromJson<RoomsFile>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }
}
