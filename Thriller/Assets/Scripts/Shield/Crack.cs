using UnityEngine;
using UnityEngine.InputSystem;

public class Crack : MonoBehaviour
{
    public float minForce;
    public float maxForce;
    public float radius;


    private void Start() 
    {
        CrackExplosion();
    }
    public void CrackExplosion()
    {
        foreach(Transform t in transform)
        {
            var rb = t.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(Random.Range (minForce , maxForce) , transform.position , radius);
            }
        }
    }

}
