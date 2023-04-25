using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform  camPosition;

    private void Update() 
    {
        this.transform.position = camPosition.position;
    }
}
