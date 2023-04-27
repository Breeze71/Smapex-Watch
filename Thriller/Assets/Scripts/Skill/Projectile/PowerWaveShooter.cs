using UnityEngine;

public class PowerWaveShooter : MonoBehaviour
{
    public Camera cam;
    public GameObject projectile;
    public Transform firePoint;
    public float fireRate = 4;

    private Vector3 destination;
    private float timeToFire;
    private PowerWave powerWaveScript;
    public KeyCode QSkillKey;

    private void Update() 
    {
        if(Input.GetKeyDown(QSkillKey) && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1/fireRate;
            ShootProjectile();
        }
    }

    private void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f , 0 , 0));

        destination = ray.GetPoint(1000);

        InstantiateProjectile();
    }

    private void InstantiateProjectile()
    {
        var projectileObj = Instantiate(projectile , firePoint.position , Quaternion.identity) as GameObject;

        powerWaveScript = projectileObj.GetComponent<PowerWave>();
        RotateToDestination(projectileObj , destination , true);
        projectileObj.GetComponent<Rigidbody>().velocity = firePoint.transform.right * powerWaveScript.waveSpeed;
    }

    private void RotateToDestination(GameObject obj , Vector3 destination , bool onlyX)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if(onlyX)
        {
            rotation.y = 0;
            rotation.z = 0;
        }

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation , rotation ,1);
    }
}
