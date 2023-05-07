using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhyllotaxisTunnel : MonoBehaviour
{
    [SerializeField] Transform tunnel;
    [SerializeField] private float tunnelSpeed, cameraDistance;
    [SerializeField] private int band;

    private void Update() 
    {
        tunnel.position = new Vector3(tunnel.position.x, tunnel.position.y, 
            tunnel.position.z + (AudioSample.ControlBandBuffer[band] * tunnelSpeed));    

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, tunnel.position.z + cameraDistance);
    }
}
