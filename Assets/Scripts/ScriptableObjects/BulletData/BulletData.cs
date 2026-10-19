using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    public GameObject bulletPrefab;
    public float speed = 40f;
    public float damage = 10f;
    public float lifetime = 3f;
    public float radius = 0.05f;
    public int maxAmmo = 0;
    public Sprite bulletIcon;
    public LayerMask hitMask = 0;
    public GameObject hitEffectPrefab;
    public BulletEffect[] onHitEffects; // Empty = regular bullet
}
