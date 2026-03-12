using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Applies the VisualConfig to the scene at startup:
/// camera background, ambient light, fog.
///
/// Attach to any Manager GameObject. Assign the VisualConfig asset.
/// </summary>
public class SceneStyler : MonoBehaviour
{
    [SerializeField] VisualConfig config;

    void Awake()
    {
        if (config == null)
        {
            Debug.LogWarning("[SceneStyler] No VisualConfig assigned.");
            return;
        }

        ApplyToCamera();
        ApplyLighting();
    }

    void ApplyToCamera()
    {
        var cam = Camera.main;
        if (cam == null) return;

        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = config.backgroundColor;
    }

    void ApplyLighting()
    {
        RenderSettings.ambientMode      = AmbientMode.Flat;
        RenderSettings.ambientLight     = config.ambientColor * config.ambientIntensity;
        RenderSettings.fog              = false;

        var sun = RenderSettings.sun;
        if (sun != null)
        {
            sun.intensity = config.sunIntensity;
            sun.color     = config.sunColor;
        }
    }
}
