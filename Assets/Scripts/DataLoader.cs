using System.IO;
using UnityEngine;

public static class DataLoader
{
    /// <summary>
    /// Loads a JSON file from Resources and parses it into a variable.
    /// </summary>
    /// <typeparam name="T">A serializable type</typeparam>
    /// <param name="path">Path inside the Resources folder</param>
    /// <param name="variable">Parsed output object</param>
    /// <returns>The raw JSON string</returns>
    public static string LoadJson<T>(string path, out T variable)
    {
        variable = default(T);
        try
        {
            string content = Resources.Load<TextAsset>(path).text;
            variable = JsonUtility.FromJson<T>(content);
            return content;
        }
        catch (IOException e)
        {
            Debug.LogException(e);
            return null;
        }
    }
}
