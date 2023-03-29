using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestSuite
{
    //This was all testing done before ExtenJect was introduced and needed
 /*   SceneManager scene;
    DataManager dataManger;

  [SetUp]
    public void Setup()
    {
        scene  = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/SceneManager")).GetComponent<SceneManager>();
        dataManger = scene.DataManager;

        //scene.Player.Manager = scene;
    }

    [UnityTest]
    public IEnumerator SceneManagerIsInScene()
    {
        yield return null;

        Assert.IsNotNull(scene);
    }



    [UnityTest]
    public IEnumerator ClosestGetsUpdatedOnPlayerMovement()
    {
        float lastCheckTime = scene.lastCheckTime;

        Player player = scene.Player;

        player.transform.position += Vector3.forward;

        yield return new WaitForSeconds( 0.1f);

        Assert.IsTrue(scene.lastCheckTime > lastCheckTime);
    }

    [UnityTest]
    public IEnumerator ClosestGetsUpdatedOnObjectMovement()
    {
        
        float lastCheckTime = scene.lastCheckTime;

        scene.Player.currentClosest.transform.position += Vector3.forward;

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.lastCheckTime > lastCheckTime);
    }

    [UnityTest]
    public IEnumerator ClosestDetectedNewObject()
    {
       
        SceneObject oldClosest = scene.Player.currentClosest;

        scene.Player.currentClosest.transform.position = Vector3.one * 1000f;

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(oldClosest != scene.Player.currentClosest);
    }


    [UnityTest]
    public IEnumerator LoadSceneDataIfItExists()
    {

        yield return new WaitForSeconds(0.1f);
        if (File.Exists(dataManger.path))
            Assert.IsTrue(scene.saveData != null);
        else
            Assert.IsTrue(!File.Exists(dataManger.path));
    }


    [UnityTest]
    public IEnumerator ClearSceneDataWhenCalled()
    {
        if (!File.Exists(dataManger.path))
            dataManger.SaveData(new SaveData());

        yield return new WaitForSeconds(0.1f);

        dataManger.ClearData();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(!File.Exists(dataManger.path));
    }


     [UnityTest]
     public IEnumerator AllObjectsRemovedForClearAll()
     {
         

         scene.ClearAll();

         yield return new WaitForSeconds(0.1f);

         Assert.IsTrue(scene.SceneObjects.Count == 0 && GameObject.FindObjectsOfType<SceneObject>().Length == 0);
     }

    [UnityTest]
    public IEnumerator ItemAddedOnButtonOrKey()
    {
        //SceneManager scene = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/SceneManager")).GetComponent<SceneManager>();
        yield return new WaitForSeconds(0.1f);
        int count = scene.SceneObjects.Count;
        scene.SpawnNewItem();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.SceneObjects.Count > count);
    }

    [UnityTest]
    public IEnumerator BotAddedOnButtonOrKey()
    {
       // SceneManager scene = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/SceneManager")).GetComponent<SceneManager>();
        yield return new WaitForSeconds(0.1f);
        int count = scene.SceneObjects.Count;
        scene.SpawnNewBot();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.SceneObjects.Count > count);
    }


    [TearDown]
    public void Teardown()
    {
        scene.ClearAll();
        Object.Destroy(scene.gameObject);
    }*/
}
