using UnityEngine;

public class EnemyShooter : PausableMonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float fireRate = 0.5f;

    private float _nextFireTime;

    public void TryShoot(Transform target)
    {
        if (Time.time < _nextFireTime) return;

        _nextFireTime = Time.time + 1f / fireRate;
        shotPoint.LookAt(target);
        Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation);
    }
}