using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomLoader))]
public class RoomLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomLoader loader = (RoomLoader)target;

        if (GUILayout.Button("Setup Rooms"))
        {
            loader.Setup();
            if (!Application.isPlaying)
                EditorUtility.SetDirty(loader);
        }   
    }
}
