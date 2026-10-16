using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Combat/Effects/DamageEffect")]
public class DamageEffect : BulletEffect
{
    public override void Apply(in BulletHitContext ctx)
    {
        ctx.Hit.collider.GetComponentInParent<IDamageable>()?.Damage(ctx.Damage);
    }
}
