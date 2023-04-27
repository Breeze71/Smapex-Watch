using UnityEngine;

public class DestructableObject : MonoBehaviour, IDamagable
{
    private Vector3 lastHitPoint;
    private float currentHealth;

    [Header("References")]    
    [SerializeField] private float maxHealth;
    [SerializeField] private Transform crackPrefabs;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float radius;

    private void Awake() 
    {
        currentHealth = maxHealth;    
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        lastHitPoint = hitPoint;
        currentHealth -= damage;

        if(currentHealth <= 0)
            Desturct();
    }

    private void Desturct()
    {
        //Instantiate(ExplosionEffect, lastHitPoint, Quaternion.identity);

        Transform crackObj = Instantiate(crackPrefabs, transform.position, transform.rotation);

        foreach(Transform child in crackObj)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRb))
                childRb.AddExplosionForce(Random.Range(minForce, maxForce), transform.position, radius);
        }

        Destroy(this.gameObject);
    }
}
