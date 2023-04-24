using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    private Rigidbody rb;
    private bool hitTarget;

    // IShootable
    /*
    public void IShootable(float damage)
    {
        damage = this.damage;
    }*/

    private void Start() 
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter(Collision collider)
    {
        // interface - IDamagable
        IDamagable damagable = collider.gameObject.GetComponent<IDamagable>();
        if(damagable != null)
        {
            damagable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
