using UnityEngine;

public readonly struct BulletHitContext
{
    public readonly RaycastHit Hit;
    public readonly GameObject Owner;
    public readonly float Damage;
    public readonly BulletData Bullet;

    public BulletHitContext(RaycastHit hit, GameObject owner, float damage, BulletData bullet)
    {
        Hit = hit;
        Owner = owner;
        Damage = damage;
        Bullet = bullet;
    }
}
