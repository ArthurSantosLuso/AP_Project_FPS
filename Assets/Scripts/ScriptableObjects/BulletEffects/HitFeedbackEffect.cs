using UnityEngine;

// Bullet on-hit effect: shows the hit marker UI when the bullet hits something damageable.
[CreateAssetMenu(fileName = "HitFeedbackEffect", menuName = "Scriptable Objects/HitFeedbackEffect")]
public class HitFeedbackEffect : BulletEffect
{
    public override void Apply(in BulletHitContext ctx)
    {
        // Verify if bullet hit a Damageable GameObject
        Collider target = ctx.Hit.collider;
        if (target.GetComponentInParent<IDamageable>() == null) return;

        // Trigger the hit marker UI
        HitMarker.Instance?.ShowHit(target);
    }
}
