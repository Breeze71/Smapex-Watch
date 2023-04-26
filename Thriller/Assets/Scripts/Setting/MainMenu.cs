using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Loader.LoadScene(Loader.Scene.GameScene);
    }
}
