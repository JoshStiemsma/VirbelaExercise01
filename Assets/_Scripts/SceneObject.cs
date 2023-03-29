using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Zenject;

/// <summary>
/// Scene Object is a parent Monobehavior class that handles the movement detection and virtual methods for setting this objects materials when highlighted
/// </summary>
public class SceneObject : MonoBehaviour
{

    /// <summary>
    /// Previouse position is a vector 3 of where this object was located last, used to detect change in position between frames
    /// </summary>
    Vector3 previousePostion;//store previouse position so we can only send updates on change and not all the time

    /// <summary>
    /// Reference to the scene manager to receive updated information and call refresh when this object moves. 
    /// </summary>
    /// 
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
    public void Construct(SceneManager manager)
    {
        Manager = manager;
    }
    /// <summary>
    /// A reference to this objects Mesh Renderer, used for changing materials upon highlighting 
    /// </summary>
    [HideInInspector]
    public MeshRenderer MeshRend
    {
        get {
            if(meshRend == null)
                meshRend = this.GetComponent<MeshRenderer>();
            return meshRend;
        }
    }
    private MeshRenderer meshRend;


    private void Awake()
    {
        previousePostion = this.transform.position;//set previouse position so that at start we arent running a checck for everysingle object
      
        MeshRend.material = Manager.defaultMaterial;//already set in inspector but just incase set here too
      
    }
    private void Update()
    {
        RunUpdate();
    }

    /// <summary>
    /// Run Update is basically an update method made public for additional use by inpector. 
    /// if the object has changed position, the manager needs to refresh its nearest item
    /// </summary>
    public void RunUpdate() //Pulling update out to its own method allows us to call in when in edit mode if wanted
    {
        //if this item moves, refresh players closest item
        if (previousePostion != this.transform.position)
        {
            Manager.FindClosestItem();
            previousePostion = this.transform.position;

        }
    }


    /// <summary>
    /// Set Highlight is a virtual method used by child classes to handle the toggling of thier designated highlighted materials.
    /// </summary>
    /// <param name="isHighlited">The flag used to set this objects material to highlighted or not highlighted</param>
    public virtual void SetHighlighted(bool _isHighlited)
    {
    }
}
