using UnityEngine;

[System.Serializable]
public struct BulletResistance
{
    public BulletData bullet;
    [Tooltip("1 = normal, 0.5 = takes half, 0 = immune, 2 = weak (double damage)")]
    public float multiplier;
}
