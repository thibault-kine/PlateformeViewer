#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu: PlateformeViewer > Setup Room Signs
///
/// Finds every RoomController in the scene and adds a "DoorSign" child
/// with RoomSign component if one doesn't already exist.
/// Position the DoorSign child manually in the scene to match the door face.
/// </summary>
public static class RoomSignSetup
{
    [MenuItem("PlateformeViewer/Setup Room Signs")]
    static void Setup()
    {
        var rooms = Object.FindObjectsByType<RoomController>(FindObjectsSortMode.None);
        int added = 0;

        foreach (var room in rooms)
        {
            // Skip if already has a DoorSign child
            Transform existing = room.transform.Find("DoorSign");
            if (existing != null) continue;

            GameObject sign = new("DoorSign");
            sign.transform.SetParent(room.transform, false);
            // Default: centered on the room origin, slightly in front
            sign.transform.localPosition = new Vector3(0f, 0f, -0.5f);

            RoomSign rs = sign.AddComponent<RoomSign>();
            // roomData field is auto-resolved at runtime via GetComponentInParent

            Undo.RegisterCreatedObjectUndo(sign, "Add DoorSign");
            added++;
        }

        Debug.Log($"[RoomSignSetup] Added {added} DoorSign(s) to {rooms.Length} room(s).");
        if (added > 0)
            EditorUtility.DisplayDialog("Room Signs", $"Added {added} DoorSign child(ren).\n\nNow position each DoorSign on its door face in the Scene view.", "OK");
        else
            EditorUtility.DisplayDialog("Room Signs", "All rooms already have a DoorSign child.", "OK");
    }
}
#endif
