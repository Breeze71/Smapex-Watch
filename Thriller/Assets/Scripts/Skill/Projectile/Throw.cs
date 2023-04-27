using UnityEngine;

public class Throw : MonoBehaviour
{
    [Header("References")]
    public GameObject throwObj;
    public KeyCode throwKey;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform firePoint;

    [Header("Throwing")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwordForce;
    [SerializeField] private int totalAmmo;
    [SerializeField] private float throwCD;
    [SerializeField] private float fireRange;
    private bool readyToThrow;

    private void Start()
    {
        readyToThrow = true;
    }
    private void Update() 
    {
        if(Input.GetKeyDown(throwKey) && readyToThrow && totalAmmo > 0)
            Threw();    
    }

    private void Threw()
    {
        readyToThrow = false;

        // projectile angle
        Quaternion q = Quaternion.FromToRotation(Vector3.up, transform.forward);
        throwObj.transform.rotation = q * firePoint.transform.rotation;
        // instantiate
        GameObject projectile = Instantiate(throwObj, firePoint.position, q);
        Rigidbody objToThrowRB = projectile.GetComponent<Rigidbody>();
        
        /* 避免 firePoint 跟準心不再同一直線
            不過用了反而出 bug 可能是 rayCast 打到自己?
            嘗試調整 firePoint 沒用

        Vector3 forceDirection = cam.transform.forward;
        RaycastHit hitable;
        if(Physics.Raycast(cam.position, cam.forward, out hitable, fireRange))
        {
            forceDirection = (hitable.point - firePoint.position).normalized;
        }
        */

        // add force
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwordForce;
        objToThrowRB.AddForce(forceToAdd, ForceMode.Impulse);

        totalAmmo--;

        // throw cd
        Invoke(nameof(ResetThrow), throwCD);
    }

    private void ResetThrow()
    {
        readyToThrow = true;
    }
}
