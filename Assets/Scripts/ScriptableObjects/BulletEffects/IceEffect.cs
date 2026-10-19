using UnityEngine;

[CreateAssetMenu(fileName = "IceEffect", menuName = "Combat/Effects/IceEffect")]
public class IceEffect : BulletEffect
{
    [SerializeField] private float freezeDuration = 3f;

    public override void Apply(in BulletHitContext ctx)
    {
        Collider col = ctx.Hit.collider;
        if (BulletDamage.IsImmune(col, ctx.Bullet)) return;

        col.GetComponentInParent<IFreezable>()?.Freeze(freezeDuration);
    }
}
