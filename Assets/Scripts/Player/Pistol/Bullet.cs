using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A pooled projectile. Moves itself each frame and raycasts along its path,
/// so it never tunnels through thin colliders at high speed.
/// No Rigidbody required.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Bullet Info")]
    [SerializeField] private BulletData data;

    private BulletData _data;

    private float timeAlive;
    private GameObject owner;
    private Action<Bullet> releaseToPool;

    private void Start()
    {
        if (_data == null) _data = data;
    }

    /// <summary>
    /// Called by the shooter every time the bullet is (re)used.
    /// </summary>
    public void Launch(BulletData bulletData, Vector3 position, Quaternion rotation, GameObject shooter, Action<Bullet> release)
    {
        _data = bulletData;
        transform.SetPositionAndRotation(position, rotation);
        owner = shooter;
        releaseToPool = release;
        timeAlive = 0f;
    }

    public void SetupBullet(BulletData bulletData, Vector3 position, Quaternion rotation, GameObject shooter)
    {
        _data = bulletData;
        transform.SetPositionAndRotation(position, rotation);
        owner = shooter;
        timeAlive = 0f;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= _data.lifetime)
        {
            Despawn();
            return;
        }

        float stepDistance = _data.speed * Time.deltaTime;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (TryCast(origin, direction, stepDistance, out RaycastHit hit))
        {
            transform.position = hit.point;
            OnHit(hit);
            return;
        }

        transform.position = origin + direction * stepDistance;
    }

    private bool TryCast(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        bool found = _data.radius > 0f
            ? Physics.SphereCast(origin, _data.radius, direction, out hit, distance, _data.hitMask, QueryTriggerInteraction.Ignore)
            : Physics.Raycast(origin, direction, out hit, distance, _data.hitMask, QueryTriggerInteraction.Ignore);

        // Ignore the shooter's own colliders.
        if (found && owner != null && hit.collider.transform.root == owner.transform.root)
            return false;

        return found;
    }

    private void OnHit(RaycastHit hit)
    {
        var ctx = new BulletHitContext(hit, owner, _data.damage, _data);
        foreach(var effect in _data.onHitEffects)
            effect.Apply(ctx);

        if (_data.hitEffectPrefab != null)
            Instantiate(_data.hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

        Despawn();
    }

    private void Despawn()
    {
        if (releaseToPool != null) releaseToPool(this);
        else Destroy(gameObject);
    }
}
