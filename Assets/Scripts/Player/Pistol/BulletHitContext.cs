using UnityEngine;

public readonly struct BulletHitContext
{
    public readonly RaycastHit Hit;
    public readonly GameObject Owner;
    public readonly float Damage;

    public BulletHitContext(RaycastHit hit, GameObject owner, float damage)
    {
        Hit = hit;
        Owner = owner;
        Damage = damage;
    }

}
