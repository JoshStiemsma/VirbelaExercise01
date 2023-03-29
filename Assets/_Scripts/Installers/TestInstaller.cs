using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TestInstaller : Installer<TestInstaller>
{

    public override void InstallBindings()
    {
        Container.Bind<SceneManager>().FromComponentInNewPrefabResource("Prefabs/SceneManager").AsSingle().NonLazy();
        Container.Bind<Player>().FromComponentInChildren().AsSingle().NonLazy();
        Container.Bind<DataManager>().AsCached().NonLazy();

    }
}
