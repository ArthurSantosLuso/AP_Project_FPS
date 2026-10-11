using UnityEngine;

public class DummyTarget : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100f;

    public bool CanDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Damage(float damageValue)
    {
        health -= damageValue;
        if (health <= 0f) Destroy(gameObject);
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
