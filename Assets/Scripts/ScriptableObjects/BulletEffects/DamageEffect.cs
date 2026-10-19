using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Combat/Effects/DamageEffect")]
public class DamageEffect : BulletEffect
{
    public override void Apply(in BulletHitContext ctx)
    {
        BulletDamage.TryDamage(ctx.Hit.collider, ctx.Damage, ctx.Bullet);
    }
}
