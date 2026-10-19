using UnityEngine;
using System;
using System.Collections.Generic;

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
    [Tooltip("Number of slots (different bullet types) in the cylinder")]
    [SerializeField] private int cylinderSize = 6;


    private float                       _nextFireTime;
    private List<BulletData>            _slots = new List<BulletData>();
    private Dictionary<BulletData, int> _ammo = new Dictionary<BulletData, int>();

    public event Action OnCylinderChanged;

    private void Awake()
    {
        if (shotPoint == null) shotPoint = transform;
        if (aimCamera == null) aimCamera = Camera.main;

        _slots = new List<BulletData>();
        _ammo = new Dictionary<BulletData, int>();
    }

    public BulletData GetBulletAt(int slotIndex) => slotIndex < _slots.Count ? _slots[slotIndex] : null;

    public int GetAmmoAt(int slotIndex)
    {
        BulletData data = GetBulletAt(slotIndex);
        return data != null && _ammo.TryGetValue(data, out int amount) ? amount : 0;
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
        BulletData bulletData = _slots[0];
        // the slot stays in the cylinder even at 0
        _ammo[bulletData]--;
        // Instantiate and store bullet game object
        GameObject go = Instantiate(bulletData.bulletPrefab);
        // Setup the bullet
        go.GetComponent<Bullet>().SetupBullet(bulletData, shotPoint.position, GetShotRotation(), gameObject);

        OnCylinderChanged?.Invoke();
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
            if (GetAmmoAt(0) == 0)
                PlayActionFailedAudio();
            
            Debug.Log("Cant shot!");
        }
    }

    protected override bool CanAttack()
    {
        if (GetBulletAt(0) == null) return false;
        // Player cannot shot if has no ammo or the time for next shot is not full yet
        if (Time.time < _nextFireTime || GetAmmoAt(0) < 1) return false;
        else return true; 
    }

    public int AddBullet(BulletData type, int amount = 1)
    {
        if (!_ammo.ContainsKey(type))
        {
            // no free hole
            if (_slots.Count >= cylinderSize) return 0; 
            _slots.Add(type);
            _ammo[type] = 0;
        }

        int added = Mathf.Min(amount, type.maxAmmo - _ammo[type]);
        if (added <= 0) return 0;

        _ammo[type] += added;
        OnCylinderChanged?.Invoke();
        return added;
    }

    public void RotateCylinder(int direction)
    {
        if (_slots.Count < 2) return;

        if (direction > 0)
        {
            BulletData first = _slots[0];
            _slots.RemoveAt(0);
            _slots.Add(first);
        }
        else
        {
            BulletData last = _slots[_slots.Count - 1];
            _slots.RemoveAt(_slots.Count - 1);
            _slots.Insert(0, last);
        }
        OnCylinderChanged?.Invoke();
    }
}