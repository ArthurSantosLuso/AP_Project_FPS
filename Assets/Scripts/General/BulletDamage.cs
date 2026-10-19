using UnityEngine;

public static class BulletDamage
{
    public static float GetMultiplier(Component target, BulletData bullet)
    {
        IBulletResistance res = target.GetComponentInParent<IBulletResistance>();
        return res != null ? res.GetMultiplier(bullet) : 1f;
    }

    public static bool IsImmune(Component target, BulletData bullet)
    {
        return GetMultiplier(target, bullet) <= 0f;
    }

    /// <summary>
    /// Damages the target, applying its resistance to this bullet.
    /// </summary>
    /// <returns>Returns true if damage was dealt.</returns>
    public static bool TryDamage(Component target, float baseDamage, BulletData bullet)
    {
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable == null) return false;

        float mult = GetMultiplier(target, bullet);
        if (mult <= 0f) return false;

        damageable.Damage(baseDamage *  mult);
        return true;
    }
}
