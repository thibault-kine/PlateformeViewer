using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Procedurally generates a door sign (colored background + room number)
/// as a World Space Canvas child of this GameObject.
///
/// Setup:
///   1. Create an empty child of the room mesh named "DoorSign"
///   2. Attach this component
///   3. Position/rotate "DoorSign" in the scene to sit on the door face
///   4. Optionally assign roomData directly; otherwise it searches up the
///      hierarchy for a RoomController
///
/// The sign reads the room code (e.g. "Vert-01", "Bleu-13") and:
///   - Picks the zone background color from the prefix
///   - Displays only the number suffix as large text
/// </summary>
public class RoomSign : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("Leave empty to auto-find RoomController in parent hierarchy")]
    [SerializeField] RoomDataSO roomData;

    [Header("Sign Dimensions (world units)")]
    [SerializeField] float signWidth  = 0.6f;
    [SerializeField] float signHeight = 0.9f;

    [Header("Text")]
    [SerializeField] float numberFontSize = 120f;
    [SerializeField] Color numberColor    = new(1f, 1f, 1f, 1f); // fully opaque white

    // Zone color table — extend if new zones are added
    static readonly (string prefix, Color color)[] ZoneColors =
    {
        ("vert",   new Color(0.11f, 0.70f, 0.43f)), // green  #1DB36E
        ("bleu",   new Color(0.11f, 0.37f, 0.65f)), // blue   #1B5EA6
        ("jaune",  new Color(0.94f, 0.65f, 0.00f)), // yellow #F0A500
        ("rouge",  new Color(0.91f, 0.30f, 0.24f)), // red    #E84C3C
        ("orange", new Color(0.95f, 0.50f, 0.10f)), // orange
    };
    static readonly Color FallbackColor = new(0.30f, 0.35f, 0.40f);

    void Start()
    {
        // Resolve room data
        if (roomData == null)
        {
            var controller = GetComponentInParent<RoomController>();
            if (controller != null) roomData = controller.roomData;
        }

        if (roomData == null)
        {
            Debug.LogWarning($"[RoomSign] No RoomDataSO found for {gameObject.name}");
            return;
        }

        BuildSign(roomData.roomCode);
    }

    void BuildSign(string roomCode)
    {
        (Color bgColor, string number) = ParseCode(roomCode);

        // ── Canvas ────────────────────────────────────────────────────────────
        GameObject canvasGO = new("SignCanvas");
        canvasGO.transform.SetParent(transform, false);
        canvasGO.transform.localPosition = Vector3.zero;
        canvasGO.transform.localRotation = Quaternion.identity;
        canvasGO.transform.localScale    = Vector3.one;

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        // 1 unit in world = 100 canvas pixels
        canvasRT.sizeDelta = new Vector2(signWidth * 100f, signHeight * 100f);
        canvasRT.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // ── Background ────────────────────────────────────────────────────────
        GameObject bgGO = new("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);

        Image bg = bgGO.AddComponent<Image>();
        bg.color = bgColor;

        RectTransform bgRT = bgGO.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;
        bgRT.anchoredPosition = Vector2.zero;

        // ── Number text ───────────────────────────────────────────────────────
        GameObject textGO = new("NumberText");
        textGO.transform.SetParent(canvasGO.transform, false);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = number;
        tmp.fontSize  = numberFontSize;
        tmp.color     = numberColor;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;

        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = Vector2.zero;
        textRT.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Parses "Vert-01" → (green, "01")  |  "Bleu-13" → (blue, "13")
    /// </summary>
    static (Color color, string number) ParseCode(string code)
    {
        if (string.IsNullOrEmpty(code)) return (FallbackColor, "?");

        int dashIndex = code.IndexOf('-');
        if (dashIndex < 0) return (FallbackColor, code);

        string prefix = code[..dashIndex].ToLowerInvariant();
        string number = code[(dashIndex + 1)..].TrimStart('0');
        if (string.IsNullOrEmpty(number)) number = "0";

        Color bg = FallbackColor;
        foreach (var (key, col) in ZoneColors)
        {
            if (prefix.StartsWith(key)) { bg = col; break; }
        }

        return (bg, number);
    }

#if UNITY_EDITOR
    // Preview in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(signWidth, signHeight, 0.01f));
    }
#endif
}
