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
