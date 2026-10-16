using UnityEngine;

[CreateAssetMenu(fileName = "IceEffect", menuName = "Combat/Effects/IceEffect")]
public class IceEffect : BulletEffect
{
    [SerializeField] private float freezeDuration = 3f;

    public override void Apply(in BulletHitContext ctx)
    {
        var target = ctx.Hit.collider.GetComponentInParent<IFreezable>();
        target?.Freeze(freezeDuration);
    }
}
