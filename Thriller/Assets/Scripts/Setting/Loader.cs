using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class Loader
{
    private static Action onLoaderCallback;
    private static AsyncOperation ansyLoad;

    public enum Scene
    {
        MainMenu,
        LoadingScene,
        GameScene
    }

    private static void Update() 
    {
        if(ansyLoad.isDone) //>= 0.9f)
            ansyLoad.allowSceneActivation = true;
    }

    // 每次換場景都 Loading 
    public static void LoadScene(Scene scene)
    {
        // recieved callback then Load to target scene
        onLoaderCallback = () =>
        {
            // 可以在背景加載場景
            ansyLoad = SceneManager.LoadSceneAsync(scene.ToString());
        };

        // loadong scene
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static float GetLoadingProgress()
    {
        if(ansyLoad != null)
            return ansyLoad.progress;
        else
            return 1f;
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
