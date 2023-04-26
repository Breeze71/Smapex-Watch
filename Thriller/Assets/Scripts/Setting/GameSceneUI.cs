using UnityEngine;


public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    [SerializeField] private GameObject pausePannel;

    private void Start() 
    {
        pausePannel.SetActive(false);
    }
    private void Update() 
    {
        if(Input.GetKeyDown(pauseKey))
        {
            pausePannel.SetActive(true);
        }
    }
    public void Exit()
    {
        Loader.LoadScene(Loader.Scene.MainMenu);
    }
}
