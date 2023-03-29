using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;
using Zenject;
using System.ComponentModel;
using UnityEditorInternal;

/// <summary>
/// The SorthMethod enum sets the type of sorting the application uses
/// 
/// SortingMethod.Distance Using direct distance calculations on every object present to find the nearest. Works Great but not as efficient
/// 
/// SortingMethod.Nearby Attempts to do smaller math calculations to narrow down the list before doing distance calculation, not really more efficiant than Distance method cause the equations are slightly smaller but now there are even more
/// 
/// SortingMethod.DistanceBuckets Uses the sorted 2d(3d) position array to only check items in nearby quadrants of the players position
/// 
/// </summary>
public enum SortMethod
{
    Distance,
    Nearby,
    PositionalBuckets
}

/// <summary>
/// SceneManager coordinates the solving of nearest object to player as well as holds reference to all the objects int he scene for the checking
/// It also utilizes DataManager to load and save the scene
/// </summary>
public class SceneManager : MonoBehaviour
{
    public SortMethod sortingMethod = SortMethod.Distance;
    /// <summary>
    /// Default Material is the material for bots and items when they are not highlighted by being the closest
    /// </summary>
    public Material defaultMaterial;
    /// <summary>
    /// Bot Material is the material used for bots when they are highlighted by being closest
    /// </summary>
    public Material BotMaterial;
    /// <summary>
    /// Item Material is the material used for items when they are highlighted by being closest
    /// </summary>
    public Material ItemMaterial;
    /// <summary>
    /// Player Material is the material used on the player 
    /// </summary>
    public Material PlayerMaterial;


    [Inject]
    void Contruct( DiContainer diContainer, Player _player)
    {
        Player = _player;
        container = diContainer;
    }
    DiContainer container;



    /// <summary>
    /// Player is our single and main player object in the scene that is used when finding the nearest object to the player
    /// Set Get stores the private player class object and finds the object in the scene if it is null
    /// </summary>
   [HideInInspector]
    public Player Player
    {
        set { player = value; }
        get {

            //when the scene is not running the dependencies are not built and injected so we default to some dependencies
            if (!Application.isPlaying && player == null) player = FindObjectOfType<Player>();


            return player; }
    }

    private Player player;


    /// <summary>
    /// Scene Objects is a List of the parent class for all objects in the scene. This is used when sorting for nearest object and saving data
    /// </summary>
    [HideInInspector]
    public List<SceneObject> SceneObjects = new List<SceneObject>();


    [HideInInspector]
    public List<SceneObject>[,] SceneObjectsPositionBuckets = new List<SceneObject>[spawnRange *2 +1, spawnRange *2 +1 ];




    /// <summary>
    /// Bot prefab is the saved prefab the gets spawned for each bot in the scene
    /// </summary>
    [HideInInspector]
    public GameObject BotPrefab;

    /// <summary>
    /// Item prefab is the saved prefab the gets spawned for each item in the scene
    /// </summary>
   [HideInInspector]
    public GameObject ItemPrefab;

    
    /// <summary>
    /// Spawn range is the float distance used when spawning in objects to dictate the max range they can randomly be in
    /// </summary>
    public static int spawnRange = 50;

    /// <summary>
    /// Default Color is the color used for objects materials when they are not highlighted by being closest
    /// </summary>
    public Color defaultColor = Color.white;

    /// <summary>
    /// Bot Color is the color used for bots materials when they are highlighted by being closest
    /// </summary>
    public Color botColor = Color.blue;

    /// <summary>
    /// Item Color is the color used for items materials when they are highlighted by being closest
    /// </summary>
    public Color itemColor = Color.red;

    /// <summary>
    /// Player Color is the color used for the player material 
    /// </summary>
    public Color playerColor = Color.green;


    /// <summary>
    /// Previouse Default Color is the stored color we used to reference if the Default color has been updated
    /// </summary>
    [HideInInspector]
    public Color prevDefaultColor = Color.white;

    /// <summary>
    /// Previouse Bot Color is the stored color we used to reference if the bot color has been updated
    /// </summary>
    [HideInInspector]
    public Color prevBotColor = Color.blue;

    /// <summary>
    /// Previouse Item Color is the stored color we used to reference if the item color has been updated
    /// </summary>
    [HideInInspector]
    public Color prevItemColor = Color.red;

    /// <summary>
    /// Previouse Player Color is the stored color we used to reference if the player color has been updated
    /// </summary>
    [HideInInspector]
    public Color prevPlayerColor = Color.green;

    [Space(20)]

    /// <summary>
    /// Run in Editor toggles the scene into a mode where updates run even in Edit mode
    /// </summary>
    public bool runInEditor = false;




    /// <summary>
    /// Data Manager Set; Get; is the public class that points to the private datamanager or created one if it doesnt exist yet 
    /// </summary>
    [HideInInspector]
    public DataManager DataManager
    {
        set { dataManager = value; }
        get
        {
            dataManager ??= new DataManager();
            return dataManager;
        }
    }
    /// <summary>
    /// Data Manager is the class that reads/writes to files with saveData, which we can rebuild the scene using. 
    /// </summary>
    private DataManager dataManager;

    /// <summary>
    /// Save Data is the object that can hold the critical data for parsing the scene into JSON as well as back out
    /// </summary>
   [HideInInspector]
    public SaveData saveData;

    /// <summary>
    /// Last check time is used for unit testing to store the last time we ran our check method
    /// </summary>
    [HideInInspector]
    public float lastCheckTime;


    private float CheckFrequency = .1f;
    private void Start()
    {
        if (BotPrefab == null) BotPrefab = Resources.Load<GameObject>("Prefabs/Bot");
        if (ItemPrefab == null) ItemPrefab = Resources.Load<GameObject>("Prefabs/Item");
         

        LoadData(true);
    }

    /// <summary>
    /// Save data when application is closed
    /// </summary>
    private void OnApplicationQuit()
    {
        SaveData();
    }

    void Update()
    {
        HandleInput();
    }





    #region Data Handling
    /// <summary>
    /// Clear save data on disk via DataManager
    /// </summary>
    public void ClearSaveData()
    {
        DataManager.ClearData();
    }

    /// <summary>
    /// Save Data collects all the critical information about the scene and its objects used for saving via the DataManager
    /// </summary>
    public void SaveData()
    { 
        //attempt save
        //Build Data to save
        saveData = new SaveData();
        saveData.Player = Player.transform.position;
        foreach (SceneObject obj in SceneObjects)
        {
            if (obj is Item)
            {
                saveData.Items.Add(obj.transform.position);
            }
            else
            {
                saveData.Bots.Add(obj.transform.position);
            }
        }

        DataManager.SaveData(saveData);
    }
   
    /// <summary>
    /// Load data and generate scene if data is present
    /// </summary>
    /// <param name="SpawnOnEmpty">Flag to indicate spawningin initial objects if there is no save or objs in scene</param>
    public void LoadData(bool SpawnOnEmpty = false)
    {
        saveData = DataManager.LoadData();

        if (saveData != null)
        {

            ClearAll();
            foreach (Vector3 v in saveData.Items)
            {
                
                SceneObject obj = SpawnPrefab(v, ItemPrefab);
                 SceneObjects.Add(obj);
                AddObjectToPositionalArray(obj);

            }
            foreach (Vector3 v in saveData.Bots)
            {
                SceneObject obj = SpawnPrefab(v, BotPrefab);

                SceneObjects.Add(obj);
                AddObjectToPositionalArray(obj);

            }
            Player.transform.position = saveData.Player;

        }
        else if(SpawnOnEmpty)
        {

            ClearAll();

            if (SceneObjects.Count == 0)
            {
                //if save does not exists
                //spawn 5 of each when we run the application
                SpawnFiveNewBot();
                SpawnFiveNewItem();
            }
        }
        FindClosestItem(false);

    }


    #endregion




    #region Inputs
    /// <summary>
    /// Handle the input of button presses to spawn new bots or items with the "B" and "I" keys
    /// </summary>
    void HandleInput()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            SpawnNewBot();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            SpawnNewItem();
        }
    }
    #endregion



    //These are the handlers and methods for inspector buttons
    //We can add 1 or 5 of Items/Bots, we can remove a single Bot or Item, we can remove all Bots/Items, and we can remove All
    #region Button Control Handlers

    //basic instantiaion of prefabs and adding to SceneObjects List
    #region Spawn Methods

    /// <summary>
    /// Spawn Prefab handles the actual spawning of the prefab call via DiContainer InstantiatePrefab which is needed so the Insert is actually utilized
    /// </summary>
    /// <param name="pos">Position of new obj in world</param>
    /// <param name="obj">the prefab being spawned</param>
    /// <returns></returns>
    SceneObject SpawnPrefab(Vector3 pos, GameObject obj)
    {
        if(container != null)
            return container.InstantiatePrefab(obj, pos, Quaternion.identity, this.transform).GetComponent<SceneObject>();
        else //if scene is not running then the dependency container isnt set up yet
            return Instantiate(obj, pos, Quaternion.identity, this.transform).GetComponent<SceneObject>();


    }


    /// <summary>
    /// Spawn new bot and add to list, search for newest close obj if flag set
    /// </summary>
    /// <param name="CheckClosest"> Flag for rechecking closest object at end of method</param>
    public void SpawnNewBot(bool CheckClosest = true)
    {
        Vector3 pos = new Vector3(Random.Range((float)-spawnRange, (float)spawnRange), .5f, Random.Range((float)-spawnRange, (float)spawnRange));
        SceneObject obj = SpawnPrefab(pos, BotPrefab);
            

        // obj.Init(this);
        SceneObjects.Add(obj);
        AddObjectToPositionalArray(obj);

        if (CheckClosest)FindClosestItem(false);
    }

    /// <summary>
    /// Spawn new item and add to list, search for newest close obj if flag set
    /// </summary>
    /// <param name="CheckClosest"> Flag for rechecking closest object at end of method</param>
    public void SpawnNewItem(bool CheckClosest = true)
    {
        Vector3 pos = new Vector3(Random.Range((float)-spawnRange, (float)spawnRange), .5f, Random.Range((float)-spawnRange, (float)spawnRange));
        SceneObject obj = SpawnPrefab(pos, ItemPrefab);
      
        SceneObjects.Add(obj);
        AddObjectToPositionalArray(obj);
        if (CheckClosest) FindClosestItem(false);

    }

    /// <summary>
    /// Add Object to Positional Array placed each scene object into the 2d array where its position in the array relates to its x and z position
    /// </summary>
    /// <param name="obj"></param>
    void AddObjectToPositionalArray(SceneObject obj)
    {
        int x = (int)obj.transform.position.x + spawnRange;
        int z = (int)obj.transform.position.z + spawnRange;
        if (SceneObjectsPositionBuckets[x, z] == null) SceneObjectsPositionBuckets[x, z] = new List<SceneObject>();
        SceneObjectsPositionBuckets[x, z].Add(obj);

    }

    /// <summary>
    /// Button handler for spawning new bots and after all are spawned running a closest check
    /// </summary>
    public void HandleSpawnFiveNewBotsButton()
    {
        SpawnFiveNewBot();
        FindClosestItem(false);

    }
    /// <summary>
    /// Button handler for spawning new Items and after all are spawned running a closest check
    /// </summary>
    public void HandleSpawnFiveNewItemsButton()
    {
        SpawnFiveNewItem();
        FindClosestItem(false);

    }
    /// <summary>
    /// Spawn five new bots and add to list, search for closest at end of method
    /// </summary>
    public void SpawnFiveNewBot()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnNewBot(false);
        }
    }

    /// <summary>
    /// Spawn five new items and add to list, search for closest at end of method
    /// </summary>
    public void SpawnFiveNewItem()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnNewItem(false);
        }
    }
    #endregion

    //These methods simply seek through the Scene Objects for one, or all, of a class type and removes one or all of that type
    //removes from SceneObjects list and destroys gameobject
    #region Clear and remove Methods

    /// <summary>
    /// Clear all Items from scene
    /// </summary>
    public void ClearItems() {
        foreach (SceneObject i in SceneObjects.AsEnumerable().Reverse())
        {
            if (i is Item)
            {
                SceneObjects.Remove(i);
                RemoveObjectFromScenePositionArray(i);

                DestroyImmediate(i.gameObject);
            }
        }
        FindClosestItem(false);

    }
    /// <summary>
    /// Clear Only Bots from scene
    /// </summary>
    public void ClearBots() {
        foreach (SceneObject i in SceneObjects.AsEnumerable().Reverse())
        {
            if (i is Bot)
            {
                SceneObjects.Remove(i);
                RemoveObjectFromScenePositionArray(i);

                DestroyImmediate(i.gameObject);
            }
        }
        FindClosestItem(false);

    }
    /// <summary>
    /// Clear all items and bots from Scene
    /// </summary>
    public void ClearAll() {
        foreach (SceneObject i in SceneObjects.AsEnumerable().Reverse())
        {
           if(i !=null) DestroyImmediate(i.gameObject);
        }
        SceneObjects.Clear();
        SceneObjectsPositionBuckets = new List<SceneObject>[spawnRange * 2+1 , spawnRange *2+1 ];
        Player.currentClosest = null;
    }

    /// <summary>
    /// Remove one item from scene starting from last item added
    /// </summary>
    public void RemoveItem()
    {
        foreach(SceneObject i in SceneObjects.AsEnumerable().Reverse())
        {
            if(i is Item)
            {
               SceneObjects.Remove(i);
                RemoveObjectFromScenePositionArray(i);
                DestroyImmediate(i.gameObject);
                break;
            }
        }
        FindClosestItem(false);

    }
    /// <summary>
    /// Remove one bot from scene starting from last bot added
    /// </summary>
    public void RemoveBot()
    {
        foreach (SceneObject i in SceneObjects.AsEnumerable().Reverse())
        {
            if (i is Bot)
            {
                SceneObjects.Remove(i);
                RemoveObjectFromScenePositionArray(i);

                DestroyImmediate(i.gameObject);
                break;
            }
        }
        FindClosestItem(false);

    }
   
    /// <summary>
    /// Remove object from scene position array removes the passed in obj from the array when found
    /// </summary>
    /// <param name="obj">The objct to be removed</param>
    void RemoveObjectFromScenePositionArray(SceneObject obj)
    {
        //we could use the objects position to just know where its att in the array but just incase something changed, we can search the whole array
        //might optimize this som array.contains isnt getting called 1000 times and only once
        for (int x = 0; x < spawnRange * 2; x++)
        {
            for (int y = 0; y < spawnRange * 2; y++)
            {
                if (SceneObjectsPositionBuckets[x, y] != null && SceneObjectsPositionBuckets[x, y].Contains(obj))
                    SceneObjectsPositionBuckets[x, y].Remove(obj);
            }
        }
    }
    
    
    #endregion

    #endregion




    #region Color Updates

    /// <summary>
    /// Update the Color of the Default Material Shader with the given color
    /// </summary>
    /// <param name="newColor"> New color to be applied to material</param>
    public void UpdateDefaultColor(Color newColor)
    {
        defaultMaterial.SetColor("_Color", newColor);
    }
    /// <summary>
    /// Update the Color of the Default Material Shader with the given color
    /// </summary>
    /// <param name="newColor"> New color to be applied to material</param>
    public void UpdateItemColor(Color newColor)
    {
        ItemMaterial.SetColor("_Color", newColor);
    }
    /// <summary>
    /// Update the Color of the Default Material Shader with the given color
    /// </summary>
    /// <param name="newColor"> New color to be applied to material</param>
    public void UpdateBotColor(Color newColor)
    {
        BotMaterial.SetColor("_Color", newColor);
    }
    /// <summary>
    /// Update the Color of the Default Material Shader with the given color
    /// </summary>
    /// <param name="newColor"> New color to be applied to material</param>
    public void UpdatePlayerColor(Color newColor)
    {
        PlayerMaterial.SetColor("_Color", newColor);

    }
    #endregion


    /// <summary>
    /// Using a given list of objeccts, use the distance method to find the closest
    /// </summary>
    /// <param name="list">the list to search through</param>
    /// <param name="position">The position to check against</param>
    /// <returns></returns>
    SceneObject FindClosestInGivenList(List<SceneObject> list, Vector3 position)
    {
        float shortestDistance = float.MaxValue;
        Vector3 playerPosition = Player.transform.position;
        SceneObject closest = null;

        //using the SceneObjects List, compare distance and store the closest
        foreach (var obj in SceneObjects)
        {
            float distance = Mathf.Abs((obj.transform.position - playerPosition).sqrMagnitude);
            if (distance < shortestDistance || closest == null)
            {
                closest = obj;
                shortestDistance = distance;
                // Debug.Log($"Found closest item {obj.gameObject.name}  {distance}");
            }
        }

        return closest;
    }




    /// <summary>
    /// Ignore Delay removes the Closest Item method from skipping updates so that it updates everyframe
    /// </summary>
    public bool IgnoreDelay = false;


    /// <summary>
    /// Find Closest Item method uses the SceneObjects List to calculate the closest Item/Bot while at the same time updating the material Highlight state of every Item/Bot
    /// 
    /// SortingMethod.Distance Using direct distance calculations on every object present to find the nearest. Works Great but not as efficient
    /// 
    /// SortingMethod.Nearby Attempts to do smaller math calculations to narrow down the list before doing distance calculation, not really more efficiant than Distance method cause the equations are slightly smaller but now there are even more
    /// 
    /// SortingMethod.DistanceBuckets Uses the sorted 2d(3d) position array to only check items in nearby quadrants of the players position
    /// 
    /// </summary>
    /// <returns>
    /// Return the Closest SceneObjectFound
    /// </returns>
    /// 
    public SceneObject FindClosestItem(bool UseDelayedCheck = true)
    {
        //only run Find Closest Item every x amount of seconds, otherwise return current closest
        if (UseDelayedCheck && !IgnoreDelay)
        {
            if(Time.time - CheckFrequency < lastCheckTime)
            {
                return Player.currentClosest;
            }

        }

        lastCheckTime = Time.time;
        if (SceneObjects.Count == 0) return null;


        SceneObject closest = null;

        if (sortingMethod == SortMethod.Distance)
        {
            if (Player.currentClosest != null) Player.currentClosest.SetHighlighted(false);

            closest = FindClosestInGivenList(SceneObjects, Player.transform.position);
            
            //now that we have the closest, highlight it and store it
            closest.SetHighlighted(true);
            player.currentClosest = closest;

        }
        else if(sortingMethod == SortMethod.Nearby)
        {
            
            if (Player.currentClosest != null) Player.currentClosest.SetHighlighted(false);

            ChecWithinDistance(1, HandleFinish);

          void HandleFinish(SceneObject obj)
            {
                closest = obj;
                closest.SetHighlighted(true);
                Player.currentClosest = closest;
            }

        }else if (sortingMethod == SortMethod.PositionalBuckets)
        {
            if (Player.currentClosest != null) Player.currentClosest.SetHighlighted(false);
            List<SceneObject> nearbyObjs = new List<SceneObject>();
            
            GatherObjectsFromNearbyBuckets(1, ref nearbyObjs);
            closest = FindClosestInGivenList(nearbyObjs, Player.transform.position);

            //now that we have the closest, highlight it and store it
            closest.SetHighlighted(true);
            Player.currentClosest = closest;

        }
        return closest;

    }



    /// <summary>
    /// Gather objects from nearby buckets uses the 2d array of objects thats related to thier int position to only check nearby buckets 
    /// </summary>
    /// <param name="distance">The amount of buckets to check away from the center</param>
    /// <param name="list">A reference to an initialy empty list that gets filled and then use after this method is done</param>
    void GatherObjectsFromNearbyBuckets(int distance, ref List<SceneObject> list)
    {
        if (distance > spawnRange)
        {
            list = SceneObjects;
            return;
        }

        int x = (int)Player.transform.position.x + spawnRange;
        int y = (int)Player.transform.position.z +spawnRange;

        if (x > spawnRange * 2) x = spawnRange * 2;
        else if (x < 0) x = 0;
        if (y > spawnRange) y = spawnRange * 2;
        else if (y < 0) y = 0;

        if (SceneObjectsPositionBuckets[x, y] != null)
            list.AddRange(SceneObjectsPositionBuckets[x, y]);

        if (list.Count ==0)
        {
            if (x + distance < spawnRange && SceneObjectsPositionBuckets[x + distance, y] != null) list.AddRange(SceneObjectsPositionBuckets[x + distance, y]);//if not null 
            if (x - distance > 0 && SceneObjectsPositionBuckets[x - distance, y] != null) list.AddRange(SceneObjectsPositionBuckets[x - distance, y]);
            if (y + distance < spawnRange && SceneObjectsPositionBuckets[x, y + distance] != null) list.AddRange(SceneObjectsPositionBuckets[x , y + distance]);
            if (y - distance > 0 && SceneObjectsPositionBuckets[x, y - distance] != null) list.AddRange(SceneObjectsPositionBuckets[x , y - distance]);

            if (x + distance < spawnRange && y - distance > 0 && SceneObjectsPositionBuckets[x + distance, y - distance] != null) list.AddRange(SceneObjectsPositionBuckets[x + distance, y - distance]);
            if (x - distance > 0 && y - distance > 0 && SceneObjectsPositionBuckets[x - distance, y - distance] != null) list.AddRange(SceneObjectsPositionBuckets[x - distance, y - distance]);
            if (x + distance < spawnRange && y + distance < spawnRange && SceneObjectsPositionBuckets[x + distance, y + distance] != null) list.AddRange(SceneObjectsPositionBuckets[x + distance, y + distance]);
            if (x - distance > 0 && y + distance < spawnRange && SceneObjectsPositionBuckets[x - distance, y + distance] != null) list.AddRange(SceneObjectsPositionBuckets[x - distance, y + distance]);

            if (list.Count == 0)
                GatherObjectsFromNearbyBuckets(distance+=1, ref list);
           
        }
    }

    /// <summary>
    /// Check for nearest object but only within given distance
    /// </summary>
    /// <param name="d"> Distance to check within </param>
    /// <param name="OnComplete"> Action returning closest scene object when method is complete and obj is found</param>
    void ChecWithinDistance(int d, Action<SceneObject> OnComplete)
    {
        float shortestDistance = float.MaxValue;
        Vector3 playerPosition = Player.transform.position;
        List<SceneObject> nearbyObjs = new List<SceneObject>();
        SceneObject closest = null;

       // Debug.Log($"Check within distanc {d}");
        //using the SceneObjects List, find only nearby objs
        foreach (var obj in SceneObjects)
        {
            Vector3 pos = obj.transform.position;
            if (pos.x < playerPosition.x + d &&
                 pos.x > playerPosition.x - d &&
                 pos.z < playerPosition.z + d &&
                 pos.z > playerPosition.z - d
                 )
            {
                nearbyObjs.Add(obj);
            }
        }

        if (nearbyObjs.Count > 0)
        {
            foreach (var obj in nearbyObjs)
            {
                float distance = Mathf.Abs((obj.transform.position - playerPosition).sqrMagnitude);
                if (closest == null || distance < shortestDistance)
                {
                    closest = obj;
                    shortestDistance = distance;
                    // Debug.Log($"Found closest item {closest.gameObject.name}  {distance}  {nearbyObjs.Count}");
                }
            }

            OnComplete(closest);
        }
        else
        {
             ChecWithinDistance(d+=1,OnComplete);
        }


    }
}
