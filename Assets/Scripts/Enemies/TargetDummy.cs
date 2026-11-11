using UnityEngine;

public class TargetDummy : MonoBehaviour, IDamageable
{
    public bool CanDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Damage(float damageValue)
    {
        Destroy(gameObject);
    }

    public void DamageNoStagger(float damageValue)
    {
        throw new System.NotImplementedException();
    }

    public bool HasBlood()
    {
        throw new System.NotImplementedException();
    }
}
