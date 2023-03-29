using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// The player class is a monobehavior class used to track the player object and detect movement while storing the closest object 
/// </summary>
public class Player : MonoBehaviour
{

    /// <summary>
    /// Reference to the scene manager to receive updated information and call refresh when this object moves. Injected via SceneInstaller
    /// </summary>
    private SceneManager manager;

    public SceneManager Manager
    {
        set
        {
            manager = value;
        }
        get
        {

            //when the scene is not running the dependencies are not built and injected so we default to some dependencies
            if (!Application.isPlaying && manager == null) manager = FindObjectOfType<SceneManager>();

            return manager;
        }
    }

    [Inject]
    public void Construct(SceneManager _manager)
    {
        Manager = _manager;
    }
    /// <summary>
    /// A reference to this objects Mesh Renderer, used for changing materials upon highlighting 
    /// </summary>
    [HideInInspector]
    public MeshRenderer MeshRend
    {
        get
        {
            if (meshRend == null)
                meshRend = this.GetComponent<MeshRenderer>();
            return meshRend;
        }
    }
    private MeshRenderer meshRend;

    /// <summary>
    /// Current closest is the stored object that was found to be closest
    /// </summary>
    public SceneObject currentClosest = null;


    /// <summary>
    /// Previouse position is a vector 3 of where this object was located last, used to detect change in position between frames
    /// </summary>
    Vector3 previousePostion = Vector3.zero;//store previouse position so we can only send updates on change and not all the time


    void Update()
    {
        RunUpdate();
    }

    /// <summary>
    /// Run Update is basically an update method made public for additional use by inpector. 
    /// if the object has changed position, the manager needs to refresh its nearest item
    /// </summary>
    public void RunUpdate()//Pulling update out to its own method allows us to call it when in edit mode if wanted
    {
        //if we havent found closest yet or player moves, find new closest
        if (Manager != null &&(currentClosest == null || previousePostion != this.transform.position))
        {
            Manager.FindClosestItem();
            previousePostion = this.transform.position;
        }
    }

}
