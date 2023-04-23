using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerWave : MonoBehaviour
{
    public float waveSpeed;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 0.1f;
    public float destoryDelay = 5;

    private Rigidbody rb;
    private bool stopped;

    private void Start() 
    {
        transform.position = new Vector3(transform.position.x , 0 , 0);

        if(GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            StartCoroutine(slowDown());
        }
        else
        {
            Debug.Log("no rb");
        }

        Destroy(gameObject , destoryDelay);
    }

    private void FixedUpdate()
    {
        if(!stopped)
        {
            RaycastHit hit;
            Vector3 distance = new Vector3(transform.position.x , transform.position.y + 1 , transform.position.z);

            // 碰到東西時改變高度
            if(Physics.Raycast(distance , transform.TransformDirection(-Vector3.up) , out hit , detectingDistance))
            {
                transform.position = new Vector3(transform.position.x , hit.point.y , transform.position.z);
            }
            else
            {
                transform.position = new Vector3(transform.position.x , transform.position.y , transform.position.z);
            }
        }
    }

    IEnumerator slowDown()
    {
        float t = 1;

        while(t > 0)
        {
            rb.velocity = Vector3.Lerp(Vector3.zero , rb.velocity , t);
            t -= slowDownRate;

            yield return new WaitForSeconds(0.1f);
        }

        stopped = true;
    }
}
