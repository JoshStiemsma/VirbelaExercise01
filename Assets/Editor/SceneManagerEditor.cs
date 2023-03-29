using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneManager))]
public class SceneManagerEditor : Editor
{
    SceneManager myScript;
  
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

         myScript = (SceneManager)target;


        #region Spawn Buttons

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Item"))
        {
            myScript.SpawnNewItem();
        }

        if (GUILayout.Button("Add Bot"))
        {
            myScript.SpawnNewBot();
        }
        if (GUILayout.Button("Remove Item"))
        {
            myScript.RemoveItem();
        }

        if (GUILayout.Button("Remove Bot"))
        {
            myScript.RemoveBot();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Five Item"))
        {
            myScript.HandleSpawnFiveNewItemsButton();
        }

        if (GUILayout.Button("Add Five Bot"))
        {
            myScript.HandleSpawnFiveNewBotsButton();
        }
        GUILayout.EndHorizontal();
        #endregion

        #region Clear and Remove Buttons
        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear All Items"))
        {
            myScript.ClearItems();
        }

        if (GUILayout.Button("Clear All Bot"))
        {
            myScript.ClearBots();
        }

        if (GUILayout.Button("Clear All "))
        {
            myScript.ClearAll();
        }
        GUILayout.EndHorizontal();
        #endregion


        #region Update Colors on Change 
        GUILayout.Space(20);
        if (myScript.prevDefaultColor != myScript.defaultColor)
        {
            myScript.prevDefaultColor = myScript.defaultColor;
            myScript.UpdateDefaultColor(myScript.defaultColor);
        }
        if (myScript.prevBotColor != myScript.botColor)
        {
            myScript.prevDefaultColor = myScript.botColor;
            myScript.UpdateBotColor(myScript.botColor);
        }
        if (myScript.prevItemColor != myScript.itemColor)
        {
            myScript.prevItemColor = myScript.itemColor;
            myScript.UpdateItemColor(myScript.itemColor);
        }
        if (myScript.prevPlayerColor != myScript.playerColor)
        {
            myScript.prevPlayerColor = myScript.playerColor;
            myScript.UpdatePlayerColor(myScript.playerColor);
        }
        #endregion

        #region Save Buttons
        //GUILayout.Space(20);
        if (GUILayout.Button("Save Data"))
        {
            myScript.SaveData();
        }
        if (GUILayout.Button("Load Data"))
        {
            myScript.LoadData();
        }
        if (GUILayout.Button("Clear Save Data"))
        {
            myScript.ClearSaveData();
        }


        #endregion

    }


    void OnSceneGUI()
    {
        if(myScript == null) myScript = (SceneManager)target;
        //this statement will execute our update codes even in Edit Mode if we set the Run In Editor toggle to true
        if (myScript.runInEditor && Event.current.type == EventType.Repaint)
        {
            myScript.Player.RunUpdate();
            foreach(SceneObject obj in myScript.SceneObjects)
            {
                obj.RunUpdate();
            }
            SceneView.RepaintAll();

        }
    }
}
