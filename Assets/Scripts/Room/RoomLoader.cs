using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class RoomLoader : MonoBehaviour
{
    [SerializeField] GameObject buildingManager;
    Dictionary<RoomApiResponse, Transform> dict = new();

    public void Setup()
    {
        List<Transform> children = new();
        List<RoomApiResponse> responses = new();

        // Get all the children of the building
        foreach (Transform child in buildingManager.transform) 
            children.Add(child);

        // Get all the data in MockData/ and parse them into a RoomApiResponse object
        var allMock = Resources.LoadAll("MockData");
        foreach (var mock in allMock)
        {
            RoomApiResponse res;
            DataLoader.LoadJson($"MockData/{mock.name}", out res);
            responses.Add(res);
        }

        // Cycle through children and set them up accordingly if their ID correspond
        foreach (var child in children)
        {
            foreach (var res in responses)
            {
                if (child.name != res.room.name)
                    continue;

                dict.Add(res, child);
            }
        }

        SetupRooms();
    }

    public void SetupRooms()
    {
        foreach (var (res, child) in dict)
        {
            // Set the layer to "Room", or else it won't do anything when clicked
            child.gameObject.layer = LayerMask.GetMask("Room");

            // Add MeshCollider for more accuracy (and also setting the bounds is a pain in the ass)
            child.AddComponent<MeshCollider>();

            // Add the RoomStatusIndicator script
            var _rsi = child.AddComponent<RoomStatusIndicator>();
            _rsi.SetRenderer(child.GetComponent<MeshRenderer>());
            _rsi.SetMaterialIndex(0);
            _rsi.SetVisualConfig(Resources.Load<VisualConfig>("Config/VisualConfig_"));

            // Add the RoomController script
            var _rc = child.AddComponent<RoomController>();
            _rc.roomData = Resources.Load<RoomDataSO>($"RoomData/Room_{res.room.name}");
            _rc.SetStatusIndicator(child.GetComponent<RoomStatusIndicator>());

            Debug.Log($"Setup for {res.room.name} done!");
        }
    }
}