using UnityEngine;
using System.Collections.Generic;

public class DataDispatcher : MonoBehaviour
{
    public GameObject parent;
    private List<Transform> children = new();

    private void Start()
    {
        GetChildren();
    }

    public void GetChildren()
    {
        foreach (Transform child in parent.transform.GetComponentsInChildren<Transform>())
        {
            var go = child.gameObject;
            RoomDetail _rd;
            DataLoader.LoadJson("Data/rooms.json", out _rd);
            if (_rd != null && _rd.name == child.name)
            {
                children.Add(child);
                Debug.Log($"Added child: {child.name}");
            }
        }
    }
}
