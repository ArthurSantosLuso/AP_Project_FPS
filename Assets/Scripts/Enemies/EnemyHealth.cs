using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : ValueBase, IDamageable, IDamageNotifier
{
    [SerializeField] private Slider uiHealthBar;

    /// <summary>
    /// Raised after damage is applied. Use by anything that want to react to hits.
    /// </summary>
    public event Action<float> Damaged;

    /// <summary>
    /// Raised once when health reaches zero. If nobody subscribes, the enemy is destroyed.
    /// </summary>
    public Action<EnemyHealth> OnDeath;

    private bool isDead;

    public bool IsDead => isDead;

    public void Damage(float damageValue)
    {
        ApplyDamage(damageValue);
    }

    public void DamageNoStagger(float damageValue)
    {
        ApplyDamage(damageValue);
    }

    private void ApplyDamage(float damageValue)
    {
        if (isDead) return;

        ReduceValue(damageValue);

        // Notify listeners before death handling,
        // so a killing blow still reaches them while the object exists.
        Damaged?.Invoke(damageValue);

        uiHealthBar.value = currentValue;

        // Verify if entity died
        if (currentValue <= 0f)
            Kill();
    }

    private void Kill()
    {
        isDead = true;

        if (OnDeath != null)
            OnDeath.Invoke(this);   // Whoever listens decides what happens next
        else
            Destroy(gameObject);
    }

    public bool CanDamage() => !isDead && currentValue > 0f;
    public bool HasBlood() => true;
}