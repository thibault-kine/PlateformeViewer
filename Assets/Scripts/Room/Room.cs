using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Room
{
    public string id;

    public string name;
    public string email;
    public string type;

    public int capacity;
    public string building;
    public string floor;
    public string category;

    public List<string> features;

    public string generatedResourceName;


    public static Room FromJson(string json)
    {
        return JsonUtility.FromJson<Room>(json);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }
}

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