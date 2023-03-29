using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Save Data is the custom data type used for parsing the scene critical data into Json to be Read/Written to file
/// </summary>
[System.Serializable]
public class SaveData
{
    /// <summary>
    /// Bots is a list of positions of all bots in the scene
    /// </summary>
    public List<Vector3> Bots = new List<Vector3>();

    /// <summary>
    /// Items is a list of positions of all items in the scene
    /// </summary>
    public List<Vector3> Items = new List<Vector3>();

    /// <summary>
    /// Player is the players position in the scene
    /// </summary>
    public Vector3 Player;

}
