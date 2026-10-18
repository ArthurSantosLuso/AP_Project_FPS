using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : ValueBase, IDamageable, IDamageNotifier
{
    [SerializeField] private Slider uiHealthBar;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float targetFlashDuration = 0.06f;

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

        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) StartCoroutine(FlashTarget(renderer));
    }

    public void DamageNoStagger(float damageValue)
    {
        ApplyDamage(damageValue);

        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) StartCoroutine(FlashTarget(renderer));
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

    private IEnumerator FlashTarget(Renderer targetRenderer)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_BaseColor", flashColor);
        propertyBlock.SetColor("_EmissionColor", flashColor * 2f);
        targetRenderer.SetPropertyBlock(propertyBlock);

        yield return new WaitForSecondsRealtime(targetFlashDuration);

        if (targetRenderer != null) targetRenderer.SetPropertyBlock(null);
    }

    public bool CanDamage() => !isDead && currentValue > 0f;
    public bool HasBlood() => true;
}