using UnityEngine;

public abstract class BulletEffect : ScriptableObject
{
    public abstract void Apply(in BulletHitContext ctx);
}
