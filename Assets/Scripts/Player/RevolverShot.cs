using UnityEngine;
using UnityEngine.Pool;

public class RevolverShot : Ability
{
    [Header("References")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private PistolShootVFX shootVFX;

    [Header("Firing")]
    [SerializeField] private float fireRate = 2f;

    [Header("Pool")]
    [SerializeField] private int defaultCapacity = 6;
    [SerializeField] private int maxSize = 6;

    private IObjectPool<Bullet> _pool;
    private float _nextFireTime;

    private void Awake()
    {
        if (shotPoint == null) shotPoint = transform;

        _pool = new ObjectPool<Bullet>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: b => b.gameObject.SetActive(true),
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);
    }

    private void SpawnBullet()
    {
        Bullet bullet = _pool.Get();

        shootVFX.TriggerVFX();
        bullet.Launch(shotPoint.position, shotPoint.rotation, gameObject, _pool.Release);
    }

    public override void Perform()
    {
        if (CanAttack())
        {
            _nextFireTime = Time.time + 1f / fireRate;
            SpawnBullet();
        }
    }

    protected override bool CanAttack()
    {
        if (Time.time < _nextFireTime) return false;
        else return true;
    }
}