using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Handles all HTTP GET requests to room API endpoints.
/// Singleton — add to a Manager GameObject in the scene.
/// </summary>
public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance { get; private set; }

    const int TimeoutSeconds = 10;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Fetches room data from baseUrl/rooms/{roomCode}.
    /// Calls onSuccess with parsed response, or onError with the error message.
    /// </summary>
    public void FetchRoom(string baseUrl, string roomCode,
        Action<RoomApiResponse> onSuccess, Action<string> onError)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            onError?.Invoke("API base URL is not configured.");
            return;
        }
        StartCoroutine(GetRoom(baseUrl.TrimEnd('/'), roomCode, onSuccess, onError));
    }

    IEnumerator GetRoom(string baseUrl, string code,
        Action<RoomApiResponse> onSuccess, Action<string> onError)
    {
        string url = $"{baseUrl}/rooms/{code}";
        using var request = UnityWebRequest.Get(url);
        request.timeout = TimeoutSeconds;

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke($"[{request.responseCode}] {request.error}");
            yield break;
        }

        string json = request.downloadHandler.text;
        RoomApiResponse response = null;
        try
        {
            response = JsonUtility.FromJson<RoomApiResponse>(json);
        }
        catch (Exception e)
        {
            onError?.Invoke($"JSON parse error: {e.Message}");
            yield break;
        }

        if (response?.room == null)
        {
            onError?.Invoke("Response missing 'room' field.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}
