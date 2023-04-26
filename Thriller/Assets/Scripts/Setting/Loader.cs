using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class Loader
{
    private static Action onLoaderCallback;

    public enum Scene
    {
        MainMenu,
        LoadingScene,
        GameScene
    }

    // 每次換場景都 Loading 
    public static void LoadScene(Scene scene)
    {
        // recieved callback then Load to target scene
        onLoaderCallback = () =>
        {
            SceneManager.LoadScene(scene.ToString());
        };

        // loadong scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static void LoaderCallback()
    {
        // trigger after the first update
        if(onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
