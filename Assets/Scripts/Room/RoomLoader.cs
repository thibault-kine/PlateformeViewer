using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RoomLoader : MonoBehaviour
{
    [Tooltip("The building containing the rooms you want to setup")]
    [SerializeField] GameObject building;

    [SerializeField] LayerMask roomLayer;
    [SerializeField] Material roomMaterial;

    // A simple dictionnary:
    //      Key: RoomApiResponses relative to the fetched json data
    //      Val: The actual Transform of the room
    Dictionary<RoomApiResponse, Transform> dict = new();

    public void Setup()
    {
        List<Transform> children = new();
        List<RoomApiResponse> responses = new();

        // Get all the children of the building
        foreach (Transform child in building.transform) 
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
            // Sets the layer to "Room", or else it won't do anything when clicked
            // A bit of dark magic there. Because apparently layers in Unity were too simple for the devs' taste :^)
            child.gameObject.layer = Mathf.RoundToInt(Mathf.Log(roomLayer.value, 2));

            var meshRenderer = child.GetComponent<MeshRenderer>();
            meshRenderer.material = roomMaterial;

            // Add MeshCollider for more accuracy (and also setting the bounds is a pain in the ass)
            child.GetOrAddComponent<MeshCollider>();

            // Add the RoomStatusIndicator script
            var _roomStatusIndicator = child.GetOrAddComponent<RoomStatusIndicator>();
            _roomStatusIndicator.SetRenderer(meshRenderer);
            _roomStatusIndicator.SetMaterialIndex(0);
            _roomStatusIndicator.SetVisualConfig(Resources.Load<VisualConfig>("Config/VisualConfig_"));

            // Add the RoomController script
            var _roomController = child.GetOrAddComponent<RoomController>();
            if (!File.Exists($"Assets/Resources/RoomData/Room_{res.room.name}"))
            {
                var _so = ScriptableObject.CreateInstance<RoomDataSO>();
                _so.UpdateFromResponse(res);
                AssetDatabase.CreateAsset(_so, $"Assets/Resources/RoomData/Room_{res.room.name}.asset");
            }
            _roomController.roomData = Resources.Load<RoomDataSO>($"RoomData/Room_{res.room.name}");
            _roomController.SetStatusIndicator(child.GetComponent<RoomStatusIndicator>());

            Debug.Log($"Setup for {res.room.name} done!");
        }
    }
}