
using UnityEngine;
using System;
using Zenject;
using UnityEngine.SceneManagement;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SceneManager>().FromComponentInHierarchy().AsSingle().NonLazy();
         Container.Bind<DataManager>().AsCached().NonLazy();
        Container.Bind<Player>().FromComponentInHierarchy().AsSingle().NonLazy();

    }
}

