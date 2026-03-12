#if UNITY_EDITOR
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
        // Find VisualConfig asset anywhere in the project (avoids hardcoding path)
        VisualConfig visualConfig = null;
        var vcGuids = AssetDatabase.FindAssets("t:VisualConfig");
        if (vcGuids.Length > 0)
            visualConfig = AssetDatabase.LoadAssetAtPath<VisualConfig>(AssetDatabase.GUIDToAssetPath(vcGuids[0]));
        else
            Debug.LogWarning("[RoomLoader] No VisualConfig asset found in project.");

        // Ensure RoomData folder exists
        if (!AssetDatabase.IsValidFolder("Assets/Resources/RoomData"))
            AssetDatabase.CreateFolder("Assets/Resources", "RoomData");

        foreach (var (res, child) in dict)
        {
            // Sets the layer to "Room", or else it won't do anything when clicked
            // A bit of dark magic there. Because apparently layers in Unity were too simple for the devs' taste :^)
            child.gameObject.layer = Mathf.RoundToInt(Mathf.Log(roomLayer.value, 2));

            // Collect only MeshRenderers on children whose name starts with "Cube"
            var meshRenderers = new List<MeshRenderer>();
            foreach (var mr in child.GetComponentsInChildren<MeshRenderer>()) 
            {
                if (mr.gameObject.name.StartsWith("Cube", System.StringComparison.OrdinalIgnoreCase) && 
                    !mr.gameObject.name.EndsWith("OpenSpace", System.StringComparison.OrdinalIgnoreCase))
                {
                    meshRenderers.Add(mr);
                }
            }

            if (meshRenderers.Count == 0)
            {
                Debug.LogWarning($"[RoomLoader] No MeshRenderer found in '{child.name}' or its children — skipping.");
                continue;
            }

            // Apply material, collider, and Room layer to every Cube child
            int roomLayerIndex = Mathf.RoundToInt(Mathf.Log(roomLayer.value, 2));
            foreach (var mr in meshRenderers)
            {
                mr.material = roomMaterial;
                mr.gameObject.GetOrAddComponent<MeshCollider>();
                Debug.Log($"Mesh Collider added to gameobject {mr.gameObject.name}");
                mr.gameObject.layer = roomLayerIndex; // raycast must hit this layer
            }

            // Add the RoomStatusIndicator script
            var _roomStatusIndicator = child.GetOrAddComponent<RoomStatusIndicator>();
            _roomStatusIndicator.SetRenderers(new List<Renderer>(meshRenderers));
            _roomStatusIndicator.SetMaterialIndex(0);
            _roomStatusIndicator.SetVisualConfig(visualConfig);
            EditorUtility.SetDirty(_roomStatusIndicator);

            // Create RoomDataSO if it doesn't exist yet, then save & refresh so Resources.Load can find it
            string soPath = $"Assets/Resources/RoomData/Room_{res.room.name}.asset";
            if (!File.Exists(soPath))
            {
                var _so = ScriptableObject.CreateInstance<RoomDataSO>();
                _so.UpdateFromResponse(res);
                AssetDatabase.CreateAsset(_so, soPath);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Add the RoomController script
            var _roomController = child.GetOrAddComponent<RoomController>();
            var roomDataSO = AssetDatabase.LoadAssetAtPath<RoomDataSO>(soPath);
            _roomController.roomData = roomDataSO;
            _roomController.SetStatusIndicator(child.GetComponent<RoomStatusIndicator>());
            EditorUtility.SetDirty(_roomController);

            // Wire RoomDataSO into every RoomSign found in children (e.g. DoorSign)
            foreach (var sign in child.GetComponentsInChildren<RoomSign>())
            {
                var so = new SerializedObject(sign);
                so.FindProperty("roomData").objectReferenceValue = roomDataSO;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(sign);
            }

            Debug.Log($"Setup for {res.room.name} done!");
        }
    }
}
#endif