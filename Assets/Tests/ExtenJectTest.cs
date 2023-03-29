using Zenject;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.IO;

[TestFixture]
public class ExtenJectTest : ZenjectUnitTestFixture
{

    SceneManager scene;
    Player player;
    DataManager dataManager;

    [SetUp]
    public void BindInterfaces()
    {
        TestInstaller.Install(Container);
         scene = Container.Resolve<SceneManager>();
        player = Container.Resolve<Player>();

    }

    [Test]
    public void SceneManagerIsInScene()
    {
        Assert.NotNull(scene);
    }
    [UnityTest]
    public IEnumerator PlayerPresent()
    {

        yield return null;
        Assert.IsNotNull(player);
    }


    [UnityTest]
    public IEnumerator ClosestGetsUpdatedOnPlayerMovement()
    {

        scene.IgnoreDelay = true;
        float lastCheckTime = scene.lastCheckTime;

        player.transform.position += Vector3.forward;
        yield return new WaitForSeconds(0.2f);

        Assert.IsTrue(scene.lastCheckTime > lastCheckTime);
    }

    [UnityTest]
    public IEnumerator ClosestGetsUpdatedOnObjectMovement()
    {
        scene.IgnoreDelay = true;

        float lastCheckTime = scene.lastCheckTime;

        player.currentClosest.transform.position += Vector3.forward;

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.lastCheckTime > lastCheckTime);
    }

    [UnityTest]
    public IEnumerator ClosestDetectedNewObject()
    {
        scene.IgnoreDelay = true;

        SceneObject oldClosest = player.currentClosest;

        player.currentClosest.transform.position = Vector3.one * 1000f;

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(oldClosest != player.currentClosest);
    }


    [UnityTest]
    public IEnumerator LoadSceneDataIfItExists()
    {

        dataManager = Container.Resolve<DataManager>();

        yield return new WaitForSeconds(0.1f);
        if (File.Exists(dataManager.path))
            Assert.IsTrue(scene.saveData != null);
        else
            Assert.IsTrue(!File.Exists(dataManager.path));
    }


    [UnityTest]
    public IEnumerator ClearSceneDataWhenCalled()
    {
        dataManager = Container.Resolve<DataManager>();

        if (!File.Exists(dataManager.path))
            dataManager.SaveData(new SaveData());

        yield return new WaitForSeconds(0.1f);

        dataManager.ClearData();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(!File.Exists(dataManager.path));
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
        
        int count = scene.SceneObjects.Count;
        scene.SpawnNewItem();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.SceneObjects.Count > count);
    }

    [UnityTest]
    public IEnumerator BotAddedOnButtonOrKey()
    {
        int count = scene.SceneObjects.Count;
        scene.SpawnNewBot();

        yield return new WaitForSeconds(0.1f);

        Assert.IsTrue(scene.SceneObjects.Count > count);
    }


    [TearDown]
    public override void Teardown()
    {
        scene.ClearAll();
        Object.Destroy(scene.gameObject);
        Object.Destroy(player.gameObject);
    }
}