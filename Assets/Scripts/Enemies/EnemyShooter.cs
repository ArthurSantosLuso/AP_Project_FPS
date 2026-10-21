using UnityEditor.Rendering;
using UnityEngine;

public class EnemyShooter : PausableMonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float fireInterval = 3f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject effect;

    private float _nextFireTime;


    private float _timer;
    private bool _targetInRange;
    private bool _canShot;

    public void SetTargetStatus(bool isInRange)
    {
        _targetInRange = isInRange;
        if (!isInRange)
        {
            _timer = 0f;
            effect.SetActive(false);
        }
    }

    private void Update()
    {
        if (_targetInRange) _timer += Time.deltaTime;

        if (_timer >= fireInterval) _canShot = true;

        if (!_canShot)
        {
            float n = fireInterval - _timer;

            if (n <= 1.1f)
            {
                effect.SetActive(true);
            }
        }

    }

    public void TryShoot(Transform target)
    {
        if (!_canShot) return;
        //if (Time.time < _nextFireTime) return;

        //_nextFireTime = Time.time + 1f / fireRate;
        shotPoint.LookAt(target);
        Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation);
        _canShot = false;
        _timer = 0f;
        effect.SetActive(false);
    }
}

