#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Menu: PlateformeViewer > Revert Non-Cube Materials
///
/// Scans the selected GameObject (or entire scene if nothing selected)
/// and resets every MeshRenderer whose name does NOT start with "Cube"
/// back to the material asset that matches its GameObject name.
///
/// Looks for materials in the project using AssetDatabase — finds the
/// first material asset whose name matches (case-insensitive) the
/// GameObject name or the start of it (e.g. "Frame" matches "Frame1.001").
/// </summary>
public static class RevertNonCubeMaterials
{
    [MenuItem("PlateformeViewer/Revert Non-Cube Materials")]
    static void Revert()
    {
        GameObject root = Selection.activeGameObject;
        if (root == null)
        {
            EditorUtility.DisplayDialog("Revert Non-Cube Materials",
                "Select the building root GameObject first, then run this.", "OK");
            return;
        }

        int reverted = 0;
        int skipped  = 0;

        foreach (var mr in root.GetComponentsInChildren<MeshRenderer>(true))
        {
            string goName = mr.gameObject.name;

            // Skip Cube objects — those are intentionally managed by RoomLoader
            if (goName.StartsWith("Cube", System.StringComparison.OrdinalIgnoreCase))
                continue;

            // Find a material asset whose name matches this object's name prefix
            // e.g. "Frame1.001" → looks for a material named "Frame" or "Frame1.001"
            Material found = FindMaterialByName(goName);
            if (found == null)
            {
                Debug.LogWarning($"[RevertNonCubeMaterials] No matching material found for '{goName}' — skipped.");
                skipped++;
                continue;
            }

            Undo.RecordObject(mr, "Revert Non-Cube Material");
            mr.sharedMaterial = found;
            EditorUtility.SetDirty(mr);
            reverted++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[RevertNonCubeMaterials] Reverted {reverted} renderer(s), skipped {skipped}.");
        EditorUtility.DisplayDialog("Revert Non-Cube Materials",
            $"Done.\nReverted: {reverted}\nSkipped (no matching material): {skipped}", "OK");
    }

    /// <summary>
    /// Searches all material assets in the project for one whose name
    /// matches the given object name or its base name (before the dot).
    /// Priority: exact match > base name match > prefix match.
    /// </summary>
    static Material FindMaterialByName(string goName)
    {
        // Base name: "Frame1.001" → "Frame1", "Glass.003" → "Glass"
        string baseName = goName.Contains('.')
            ? goName[..goName.IndexOf('.')]
            : goName;

        string[] guids = AssetDatabase.FindAssets("t:Material");
        Material exactMatch  = null;
        Material baseMatch   = null;
        Material prefixMatch = null;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            string matName = mat.name;

            if (string.Equals(matName, goName, System.StringComparison.OrdinalIgnoreCase))
            { exactMatch = mat; break; }

            if (string.Equals(matName, baseName, System.StringComparison.OrdinalIgnoreCase))
                baseMatch = mat;
            else if (matName.StartsWith(baseName, System.StringComparison.OrdinalIgnoreCase) && prefixMatch == null)
                prefixMatch = mat;
        }

        return exactMatch ?? baseMatch ?? prefixMatch;
    }
}
#endif
