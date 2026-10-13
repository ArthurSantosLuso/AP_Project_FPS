using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    public float speed = 40f;
    public float damage = 10f;
    public float lifetimer = 3f;
    public float radius = 0.05f;
    public LayerMask hitMask = 0;
    public GameObject hitEffectPrefab;
    public BulletEffect[] onHitEffects; // Empty = regular bullet
}
