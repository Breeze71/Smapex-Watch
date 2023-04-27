using UnityEngine;

public class BasicEnemy : MonoBehaviour, IDamagable
{
    public float maxHealth;
    private float health;
    
    public void TakeDamage(float damage, Vector3 hitPoint)
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
