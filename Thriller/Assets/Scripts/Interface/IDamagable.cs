using UnityEngine;
public interface IDamagable
{
    //public bool hitTarget{get;set};
    public void TakeDamage(float damage, Vector3 hitPoint);
}



