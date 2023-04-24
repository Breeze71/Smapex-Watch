using UnityEngine;

public class FrameController : MonoBehaviour
{
    private void Awake() 
    {
        Application.targetFrameRate = 60;    
    }
}
