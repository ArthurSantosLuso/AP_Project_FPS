using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionEffect", menuName = "Combat/Effects/ExplosionEffect")]
public class ExplosionEffect : BulletEffect
{
    public float radius = 4f;
    public float damageMultiplier = 0.5f;
    public LayerMask mask = ~0;

    public override void Apply(in BulletHitContext ctx)
    {
        var hits = Physics.OverlapSphere(ctx.Hit.point, radius, mask, QueryTriggerInteraction.Ignore);
        foreach (var c in hits)
            c.GetComponentInParent<IDamageable>()?.Damage(ctx.Damage * damageMultiplier);
    }
}
