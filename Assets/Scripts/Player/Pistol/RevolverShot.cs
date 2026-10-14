using System.Collections.Generic;
using UnityEditor;
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

    [Header("Aiming")]
    [SerializeField] private Camera aimCamera;
    [SerializeField] private LayerMask aimMask = -0;
    [SerializeField] private float maxAimDistance = 100f;

    [Header("Pool")]
    [SerializeField] private int defaultCapacity = 6;
    [SerializeField] private int maxSize = 6;

    private int                 currentBulletCount = 6;
    private float               _nextFireTime;
    private Queue<BulletData>   revolverCylinder;
    private IObjectPool<Bullet> _pool;

    private void Awake()
    {
        if (shotPoint == null) shotPoint = transform;
        if (aimCamera == null) aimCamera = Camera.main;

        _pool = new ObjectPool<Bullet>(
            createFunc: () => Instantiate(bulletPrefab),
            actionOnGet: b => b.gameObject.SetActive(true),
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize);

        revolverCylinder = new Queue<BulletData>();
    }

    private Quaternion GetShotRotation()
    {
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 target = Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore)
            ? hit.point
            : ray.GetPoint(maxAimDistance);

        Vector3 dir = target - shotPoint.position;
        return dir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(dir) : shotPoint.rotation;
    }

    private void SpawnBullet()
    {
        Bullet bullet = _pool.Get();

        shootVFX.TriggerVFX();
        bullet.Launch(revolverCylinder.Dequeue(), shotPoint.position, GetShotRotation(), gameObject, _pool.Release);
    }

    public override void Perform()
    {
        if (CanAttack())
        {
            _nextFireTime = Time.time + 1f / fireRate;
            SpawnBullet();
            PlayActionAudio();
        }
        else
        {
            Debug.Log("Cant shot!");
        }
    }

    protected override bool CanAttack()
    {
        if (Time.time < _nextFireTime || revolverCylinder.Count < 1) return false;
        else return true;
    }

    public void AddBullet(BulletData bulletToAdd)
    {
        if (currentBulletCount < maxSize)
        {
            revolverCylinder.Enqueue(bulletToAdd);
        }
    }
}