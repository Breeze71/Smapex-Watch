using UnityEngine;

public class BasicEnemy : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth;
    private float health;

    // IDamagable
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Start() 
    {
        health = maxHealth;    
    }
    private void Update() 
    {
        Death();
    }

    private void Death()
    {
        if(health <= 0) 
        {
            Destroy(gameObject);
        }  
    }
}
