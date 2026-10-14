using UnityEngine;

// Bullet on-hit effect: shows the hit marker when the bullet hits something damageable.
[CreateAssetMenu(fileName = "HitFeedbackEffect", menuName = "Scriptable Objects/HitFeedbackEffect")]
public class HitFeedbackEffect : BulletEffect
{
    public override void Apply(in BulletHitContext ctx)
    {
        Collider target = ctx.Hit.collider;
        if (target.GetComponentInParent<IDamageable>() == null) return;

        HitMarker.Instance?.ShowHit(target);
    }
}
