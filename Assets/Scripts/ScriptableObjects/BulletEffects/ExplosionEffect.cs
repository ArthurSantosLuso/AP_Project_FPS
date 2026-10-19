using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "Combat/Effects/ExplosionEffect")]
public class ExplosionEffect : BulletEffect
{
    private static readonly HashSet<IDamageable> _seen = new HashSet<IDamageable>();

    public float radius = 4f;
    public float damageMultiplier = 0.5f;
    public LayerMask mask = ~0;

    public override void Apply(in BulletHitContext ctx)
    {
        _seen.Clear();
        var hits = Physics.OverlapSphere(ctx.Hit.point, radius, mask, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
        {
            IDamageable d = c.GetComponentInParent<IDamageable>();
            // One hit per entity
            if (d == null || !_seen.Add(d)) continue;

            BulletDamage.TryDamage(c, ctx.Damage * damageMultiplier, ctx.Bullet);
        }
    }
}
