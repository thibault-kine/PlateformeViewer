using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Sidebar panel that slides in from the right when a room is clicked.
/// </summary>
public class RoomInfoPanel : MonoBehaviour
{
    public static RoomInfoPanel Instance { get; private set; }

    [SerializeField] RectTransform panel;
    [SerializeField] BuildingConfigSO config;

    [Header("Status")]
    [SerializeField] Image statusBadge;
    [SerializeField] TMP_Text statusLabel;

    [Header("Room Info")]
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomSubtitleText;

    [Header("Current Event")]
    [SerializeField] GameObject currentEventSection;
    [SerializeField] TMP_Text currentEventTitle;
    [SerializeField] TMP_Text currentEventTime;
    [SerializeField] TMP_Text currentEventOrganizer;

    [Header("Next Event")]
    [SerializeField] GameObject nextEventSection;
    [SerializeField] TMP_Text nextEventTitle;
    [SerializeField] TMP_Text nextEventTime;
    [SerializeField] TMP_Text nextEventOrganizer;

    [Header("Schedule")]
    [SerializeField] Transform scheduleContainer;
    [SerializeField] GameObject scheduleItemPrefab;

    [Header("Footer")]
    [SerializeField] TMP_Text lastUpdatedText;

    // Status colors — must match RoomStatusIndicator and DESIGN_DECISIONS.md
    static readonly Color ColorFree     = new(0.18f, 0.80f, 0.44f);
    static readonly Color ColorOccupied = new(0.91f, 0.30f, 0.24f);
    static readonly Color ColorUpcoming = new(0.95f, 0.61f, 0.07f);
    static readonly Color ColorUnknown  = new(0.50f, 0.55f, 0.55f);

    RoomDataSO _currentRoom;
    bool _isOpen;
    float _panelWidth;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _panelWidth = panel.rect.width;
        // Start off-screen to the right
        panel.anchoredPosition = new Vector2(_panelWidth, 0f);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Show(RoomDataSO roomData)
    {
        if (roomData == null) return;

        // Unsubscribe from previous room
        if (_currentRoom != null)
            _currentRoom.OnDataUpdated -= Refresh;

        _currentRoom = roomData;
        _currentRoom.OnDataUpdated += Refresh;

        Refresh();
        SlideIn();

        // Fetch fresh data immediately (unless in mock mode)
        if (config != null && !config.useMockData)
            FetchFreshData();
    }

    public void Hide()
    {
        if (!_isOpen) return;

        if (_currentRoom != null)
        {
            _currentRoom.OnDataUpdated -= Refresh;
            _currentRoom = null;
        }
        SlideOut();
    }

    /// <summary>Wired to the manual Refresh button in the panel UI.</summary>
    public void OnRefreshButtonClicked()
    {
        if (_currentRoom == null) return;
        if (config != null && config.useMockData) return;
        FetchFreshData();
    }

    // ── Display ───────────────────────────────────────────────────────────────

    void Refresh()
    {
        if (_currentRoom == null) return;

        UpdateStatusBadge(_currentRoom.status);
        roomNameText.text     = _currentRoom.roomName;
        roomSubtitleText.text = $"{_currentRoom.roomType} · {_currentRoom.capacity} places";

        PopulateEvent(currentEventSection, currentEventTitle,
            currentEventTime, currentEventOrganizer, _currentRoom.currentEvent);

        PopulateEvent(nextEventSection, nextEventTitle,
            nextEventTime, nextEventOrganizer, _currentRoom.nextEvent);

        PopulateSchedule(_currentRoom.scheduleToday);

        if (!string.IsNullOrEmpty(_currentRoom.lastUpdated))
            lastUpdatedText.text = $"Mis à jour : {FormatTime(_currentRoom.lastUpdated)}";
        else
            lastUpdatedText.text = string.Empty;
    }

    void UpdateStatusBadge(string status)
    {
        (Color color, string label) = status switch
        {
            "occupied" => (ColorOccupied, "OCCUPÉE"),
            "free"     => (ColorFree,     "LIBRE"),
            "upcoming" => (ColorUpcoming, "BIENTÔT"),
            _          => (ColorUnknown,  "INCONNU")
        };
        statusBadge.color = color;
        statusLabel.text  = label;
    }

    void PopulateEvent(GameObject section, TMP_Text titleText,
        TMP_Text timeText, TMP_Text organizerText, EventData evt)
    {
        bool hasEvent = evt != null && evt.IsValid();
        section.SetActive(hasEvent);
        if (!hasEvent) return;

        titleText.text     = string.IsNullOrEmpty(evt.title) ? "Réservé" : evt.title;
        timeText.text      = FormatTimeRange(evt.start, evt.end);
        organizerText.text = evt.organizer;
    }

    void PopulateSchedule(List<EventData> schedule)
    {
        foreach (Transform child in scheduleContainer)
            Destroy(child.gameObject);

        if (schedule == null) return;

        foreach (var evt in schedule)
        {
            if (!evt.IsValid()) continue;
            var item  = Instantiate(scheduleItemPrefab, scheduleContainer);
            var texts = item.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = FormatTime(evt.start);
                texts[1].text = string.IsNullOrEmpty(evt.title) ? "Réservé" : evt.title;
            }
        }
    }

    // ── API fetch ─────────────────────────────────────────────────────────────

    void FetchFreshData()
    {
        if (ApiClient.Instance == null || _currentRoom == null) return;

        string code = !string.IsNullOrEmpty(_currentRoom.apiCode) ? _currentRoom.apiCode : _currentRoom.roomCode;
        ApiClient.Instance.FetchRoom(
            config.apiBaseUrl,
            code,
            resp => _currentRoom.UpdateFromResponse(resp),
            err =>
            {
                if (!string.IsNullOrEmpty(config.apiBaseUrl2))
                {
                    ApiClient.Instance.FetchRoom(
                        config.apiBaseUrl2,
                        _currentRoom.roomCode,
                        resp2 => _currentRoom.UpdateFromResponse(resp2),
                        err2 => _currentRoom.SetError());
                }
                else
                {
                    _currentRoom.SetError();
                }
            });
    }

    // ── Animation ────────────────────────────────────────────────────────────

    void SlideIn()
    {
        _isOpen = true;
        StopAllCoroutines();
        StartCoroutine(AnimatePanel(panel.anchoredPosition, Vector2.zero));
    }

    void SlideOut()
    {
        _isOpen = false;
        StopAllCoroutines();
        StartCoroutine(AnimatePanel(panel.anchoredPosition,
            new Vector2(_panelWidth, 0f)));
    }

    IEnumerator AnimatePanel(Vector2 from, Vector2 to)
    {
        float t = 0f, duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float ease = Mathf.SmoothStep(0f, 1f, t / duration);
            panel.anchoredPosition = Vector2.Lerp(from, to, ease);
            yield return null;
        }
        panel.anchoredPosition = to;
    }

    // ── Close on Escape ───────────────────────────────────────────────────────

    void Update()
    {
        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame && _isOpen)
            Hide();
    }

    // ── Time helpers ──────────────────────────────────────────────────────────

    static string FormatTimeRange(string start, string end) =>
        $"{FormatTime(start)} → {FormatTime(end)}";

    static string FormatTime(string iso)
    {
        if (string.IsNullOrEmpty(iso)) return "--:--";
        return DateTime.TryParse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind,
            out DateTime dt)
            ? dt.ToLocalTime().ToString("HH:mm")
            : iso;
    }
}
