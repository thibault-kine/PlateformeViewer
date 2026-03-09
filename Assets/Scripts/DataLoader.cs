using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataLoader : MonoBehaviour
{
    [SerializeField] string _path = "Data/rooms.json";
    
    [Header("UI Fields")]
    [SerializeField] TMP_InputField idInput;
    [SerializeField] TMP_Text dataText;
    [SerializeField] Button searchButton;

    RoomsFile roomsFile;


    private void Awake()
    {
        searchButton.onClick.AddListener(OnRoomSearched);

        LoadJson(_path);
    }

    public void LoadJson(string path)
    {
        try
        {
            string content = Resources.Load<TextAsset>(path).text;
            roomsFile = RoomsFile.FromJson(content);
        }
        catch (IOException e)
        {
            Debug.LogException(e);
        }
    }

    public void OnRoomSearched()
    {
        if (roomsFile == null)
        {
            Debug.LogError("ERROR: 'roomsFile' is empty!");
            return;
        }

        if (string.IsNullOrEmpty(idInput.text))
        {
            Debug.LogError("ERROR: Invalid input!");
            return;
        }

        string data = roomsFile.GetByID(idInput.text).ToJson();

        if (string.IsNullOrEmpty(data)) 
            data = "No data found :(";

        dataText.text = data;
    }
}
