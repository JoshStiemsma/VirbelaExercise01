
using System.IO;
using UnityEngine;

/// <summary>
/// Data Manager is the class responsible for reading and writing to disk with JSON parsing
/// </summary>
public class DataManager
{
    /// <summary>
    /// The path location used for saving data
    /// </summary>
    public string path = Application.persistentDataPath + "/savedata.json";

    /// <summary>
    /// Save Data converts the scene data intoa json string and writes to path
    /// </summary>
    /// <param name="data"> The scene data that is being saved in our SaveData format</param>
    public void SaveData(SaveData data)
    {
        Debug.Log("Save Data");

        string jsonString = JsonUtility.ToJson(data);
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Load Data checks if there is any data saved at the path and if so, reads the file into a string that is then parsed out of Json and into our SaveData type then returned
    /// </summary>
    /// <returns>Returns a copy of SaveData</returns>
    public SaveData LoadData()
    {
        SaveData data = new SaveData();
        if (File.Exists(path))
        {
            string stringData = File.ReadAllText(path);
            data = JsonUtility.FromJson<SaveData>(stringData);
            Debug.Log("Load Data");
            return data;
        }
        return null;
    }
    /// <summary>
    /// Clear Data Deletes the file at the path if it exists
    /// </summary>
    public void ClearData()
    {
        if (File.Exists(path))
        {
            Debug.Log("Save Deleted");
            File.Delete(path);
        }
    }
}
