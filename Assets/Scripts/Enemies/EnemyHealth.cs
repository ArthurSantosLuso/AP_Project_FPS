using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : ValueBase, IDamageable, IDamageNotifier, IBulletResistance
{
    [SerializeField] private Slider uiHealthBar;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float targetFlashDuration = 0.06f;
    [SerializeField] private BulletResistance[] resistances;


    /// <summary>
    /// Raised after damage is applied. Use by anything that want to react to hits.
    /// </summary>
    public event Action<float> Damaged;

    /// <summary>
    /// Raised once when health reaches zero. If nobody subscribes, the enemy is destroyed.
    /// </summary>
    public Action<EnemyHealth> OnDeath;

    private bool _isDead;
    private Dictionary<BulletData, float> _resistanceMap;

    public bool IsDead => _isDead;

    protected override void Awake()
    {
        base.Awake();

        _resistanceMap = new Dictionary<BulletData, float>();
        foreach (var r in resistances)
        {
            if (r.bullet != null)
            {
                _resistanceMap[r.bullet] = r.multiplier;
            }
        }
    }

    public float GetMultiplier(BulletData bullet)
        => bullet != null && _resistanceMap.TryGetValue(bullet, out float m) ? m : 1f; 

    public void Damage(float damageValue)
    {
        ApplyDamage(damageValue);

        TryFlash();
    }

    public void DamageNoStagger(float damageValue)
    {
        ApplyDamage(damageValue);

        TryFlash();
    }

    private void ApplyDamage(float damageValue)
    {
        if (_isDead) return;

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
        _isDead = true;

        if (OnDeath != null)
            OnDeath.Invoke(this);   // Whoever listens decides what happens next
        else
            Destroy(gameObject);
    }

    private void TryFlash()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null) StartCoroutine(FlashTarget(renderer));
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

    public bool CanDamage() => !_isDead && currentValue > 0f;
    public bool HasBlood() => true;   
}