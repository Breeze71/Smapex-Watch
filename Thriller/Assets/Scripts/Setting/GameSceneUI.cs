using UnityEngine;


public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private KeyCode pauseKey;
    [SerializeField] private GameObject pausePannel;
    public bool isPause;

    private void Start() 
    {
        pausePannel.SetActive(false);
        isPause = false;

    }

    private void Update() 
    {
        if(Input.GetKeyDown(pauseKey) && !isPause)
        {
            PressESC();

            Cursor.lockState = CursorLockMode.None;
        }
        else if(Input.GetKeyDown(pauseKey) && isPause)
        {
            PressESC();
            
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void PressESC()
    {
        isPause = !isPause;
        pausePannel.SetActive(isPause);

        Cursor.visible = isPause;
    }

    public void Exit()
    {
        Loader.LoadScene(Loader.Scene.MainMenu);
    }
}
