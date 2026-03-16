#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class ResetRoomStatus
{
    [MenuItem("Tools/Reset All Room Status to Unknown")]
    public static void ResetAll()
    {
        var guids = AssetDatabase.FindAssets("t:RoomDataSO");
        int count = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var so = AssetDatabase.LoadAssetAtPath<RoomDataSO>(path);
            if (so == null) continue;
            so.status = "unknown";
            EditorUtility.SetDirty(so);
            count++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"[ResetRoomStatus] Reset {count} RoomDataSO assets.");
    }
}
#endif
