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

    private int                 _currentBulletCount = 0;
    private float               _nextFireTime;
    private Queue<BulletData>   revolverCylinder;

    private void Awake()
    {
        if (shotPoint == null) shotPoint = transform;
        if (aimCamera == null) aimCamera = Camera.main;

        revolverCylinder = new Queue<BulletData>();
    }

    /// <summary>
    /// Get the rotation of the shot.
    /// If it identifies a target, put the bullet rotation to the target direction.
    /// </summary>
    /// <returns></returns>
    private Quaternion GetShotRotation()
    {
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 target = Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore)
            ? hit.point
            : ray.GetPoint(maxAimDistance);

        Vector3 dir = target - shotPoint.position;
        return dir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(dir) : shotPoint.rotation;
    }

    /// <summary>
    /// Instantiate and setup the bullet.
    /// </summary>
    private void SpawnBullet()
    {
        // Call the method to trigger VFX
        shootVFX.TriggerVFX();
        // Get the bullet data
        BulletData bulletData = revolverCylinder.Dequeue();
        // Instantiate and store bullet game object
        GameObject go = Instantiate(bulletData.bulletPrefab);
        // Setup the bullet
        go.GetComponent<Bullet>().SetupBullet(bulletData, shotPoint.position, GetShotRotation(), gameObject);
        // Update the aux bullet count variable
        _currentBulletCount = revolverCylinder.Count;
    }

    public override void Perform()
    {
        // Verify if can shot
        if (CanAttack())
        {
            // Set next shot time
            _nextFireTime = Time.time + 1f / fireRate;
            // Spawn bullet call
            SpawnBullet();
            // Play the shot audio
            PlayActionAudio();
        }
        else
        {
            // Just play audio when out of ammo
            if (_currentBulletCount == 0)
                PlayActionFailedAudio();
            
            Debug.Log("Cant shot!");
        }
    }

    protected override bool CanAttack()
    {
        // Player cannot shot if has no ammo or the time for next shot is not full yet
        if (Time.time < _nextFireTime || _currentBulletCount < 1) return false;
        else return true; 
    }

    public void AddBullet(BulletData bulletToAdd)
    {
        // Verify if there is free space on revolver cylinder
        if (_currentBulletCount < cylinderSize)
        {
            // Add bullet to revolver
            revolverCylinder.Enqueue(bulletToAdd);
            // Add to the count aux variable
            _currentBulletCount = revolverCylinder.Count;
        }
    }
}