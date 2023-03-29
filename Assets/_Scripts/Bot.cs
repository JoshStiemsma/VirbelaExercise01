using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// The Bot class is a child class of SceneObject used to differentiate the method and material used when highliting this scene object
/// </summary>
public class Bot : SceneObject
{
    

    /// <summary>
    /// Set highlight changes the objects mesh renderer material to its designated highlight or default material
    /// </summary>
    /// <param name="isHighlited"> Is Highlighted is the flag used to tell if the object should be set as highlighted or not</param>
    public override void SetHighlighted(bool isHighlited)
    {
        base.SetHighlighted(isHighlited);
        MeshRend.material = isHighlited ? Manager.BotMaterial : Manager.defaultMaterial;
    }
}
