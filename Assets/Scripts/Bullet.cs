using System;
using UnityEngine;

/// <summary>
/// A pooled projectile. Moves itself each frame and raycasts along its path,
/// so it never tunnels through thin colliders at high speed.
/// No Rigidbody required.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 40f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 3f;

    [Header("Collision")]
    [SerializeField] private LayerMask hitMask = ~0;      // layers the bullet can hit
    [SerializeField] private float radius = 0.05f;        // 0 = thin raycast, >0 = spherecast

    [Header("Optional")]
    [SerializeField] private GameObject hitEffectPrefab;  // Any blood effect or impact on collision

    private float timeAlive;
    private GameObject owner;
    private Action<Bullet> releaseToPool;

    /// <summary>
    /// Called by the shooter every time the bullet is (re)used.
    /// </summary>
    public void Launch(Vector3 position, Quaternion rotation, GameObject shooter, Action<Bullet> release)
    {
        transform.SetPositionAndRotation(position, rotation);
        owner = shooter;
        releaseToPool = release;
        timeAlive = 0f;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifetime)
        {
            Despawn();
            return;
        }

        float stepDistance = speed * Time.deltaTime;
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
        bool found = radius > 0f
            ? Physics.SphereCast(origin, radius, direction, out hit, distance, hitMask, QueryTriggerInteraction.Ignore)
            : Physics.Raycast(origin, direction, out hit, distance, hitMask, QueryTriggerInteraction.Ignore);

        // Ignore the shooter's own colliders.
        if (found && owner != null && hit.collider.transform.root == owner.transform.root)
            return false;

        return found;
    }

    private void OnHit(RaycastHit hit)
    {
        var damageable = hit.collider.GetComponentInParent<IDamageable>();
        damageable?.Damage(damage);

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));

        Despawn();
    }

    private void Despawn()
    {
        if (releaseToPool != null) releaseToPool(this);
    }
}
