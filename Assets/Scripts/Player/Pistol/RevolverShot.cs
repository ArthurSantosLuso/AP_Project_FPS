using System.Collections.Generic;
using UnityEngine;

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

    [Header("Revolver Configuration")]
    [Tooltip("Max number of bullets player can have at the same time")]
    [SerializeField] private int cylinderSize = 6;

    private int                 currentBulletCount = 0;
    private float               _nextFireTime;
    private Queue<BulletData>   revolverCylinder;

    private void Awake()
    {
        if (shotPoint == null) shotPoint = transform;
        if (aimCamera == null) aimCamera = Camera.main;

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
        shootVFX.TriggerVFX();
        BulletData bulletData = revolverCylinder.Dequeue();

        GameObject go = Instantiate(bulletData.bulletPrefab);
        go.GetComponent<Bullet>().SetupBullet(bulletData, shotPoint.position, GetShotRotation(), gameObject);

        currentBulletCount = revolverCylinder.Count;
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
            PlayActionFailedAudio();
            Debug.Log("Cant shot!");
        }
    }

    protected override bool CanAttack()
    {
        if (Time.time < _nextFireTime || currentBulletCount < 1) return false;
        else return true;
    }

    public void AddBullet(BulletData bulletToAdd)
    {
        if (currentBulletCount < cylinderSize)
        {
            revolverCylinder.Enqueue(bulletToAdd);

            currentBulletCount = revolverCylinder.Count;
        }
    }
}