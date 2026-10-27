using UnityEngine;

public class EnemyShooter : PausableMonoBehaviour
{
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float minFireInterval = 3f;
    [SerializeField] private float maxFireInterval = 3f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private GameObject effect;

    private float _nextFireTime;
    private float _fireInterval;


    private float _timer;
    private bool _targetInRange;
    private bool _canShot;

    private void Start()
    {
        _fireInterval = Random.Range(minFireInterval, maxFireInterval);
    }

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

        if (_timer >= _fireInterval) _canShot = true;

        if (!_canShot)
        {
            float n = _fireInterval - _timer;

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

