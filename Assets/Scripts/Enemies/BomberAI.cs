using UnityEngine;

public class BomberAI : PausableMonoBehaviour
{
    private enum BomberState
    {
        Scan,
        Chase,
        Hide
    }

    [SerializeField] private float fieldOfView = 100f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float eyeHeight = 0.5f;
    [SerializeField] private float scanTurnSpeed = 90f;
    [SerializeField] private float obstacleDistance = 2f;
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private float lifeTime = 6f;
    [SerializeField] private float explodeDistance = 1f;
    [SerializeField] private float explosionDamage = 20f;
    [SerializeField] private float chestHeight = 1f;
    [SerializeField] private Transform[] hidePoints;
    [SerializeField] private float arriveDistance = 0.3f;


    private BomberState _state = BomberState.Scan;
    private Transform _player;
    private float _turnDirection = 1f;
    private float _lifeTimer;
    private bool _wasBlocked;
    private Transform _hidePoint;

    private void Start()
    {
        _player = FindFirstObjectByType<FirstPersonMovement>().transform;
    }

    private void Update()
    {
        if (GameState.IsPaused) return;

        switch (_state)
        {
            case BomberState.Scan:
                UpdateScan();
                break;
            case BomberState.Chase:
                UpdateChase();
                break;
            case BomberState.Hide:
                UpdateHide();
                break;
        }
    }

    private void UpdateScan()
    {
        transform.Rotate(0f, scanTurnSpeed * _turnDirection * Time.deltaTime, 0f);

        bool isBlocked = Physics.Raycast(transform.position, transform.forward, obstacleDistance, ~0, QueryTriggerInteraction.Ignore);
        if (isBlocked && !_wasBlocked)
            _turnDirection *= -1f;
        _wasBlocked = isBlocked;

        if (CanSeePlayer())
            EnterChase();
    }

    private void EnterChase()
    {
        _state = BomberState.Chase;
        _lifeTimer = lifeTime;
        //Debug.Log($"Detetou | timer: {_lifeTimer} | distância: {Vector3.Distance(transform.position, _player.position)}");
    }

    private void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            EnterHide();
            return;
        }

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0f)
        {
            Destroy(gameObject);
            return;
            //Debug.Log("Destruiu por tempo");
        }

        Vector3 targetPoint = _player.position + Vector3.up * chestHeight;
        Vector3 toTarget = targetPoint - transform.position;

        if (toTarget.magnitude <= explodeDistance)
        {
            _player.GetComponent<IDamageable>().Damage(explosionDamage);
            Destroy(gameObject);
            return;
            //Debug.Log($"Destruiu por contacto | distância: {toTarget.magnitude}");
        }

        Vector3 direction = toTarget.normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
        transform.forward = direction;
    
    }

    private void EnterHide()
    {
        _state = BomberState.Hide;
        _hidePoint = GetNearestHidePoint();
    }

    private Transform GetNearestHidePoint()
    {
        Transform nearest = hidePoints[0];

        foreach (Transform point in hidePoints)
        {
            if (Vector3.Distance(transform.position, point.position) < Vector3.Distance(transform.position, nearest.position))
                nearest = point;
        }

        return nearest;
    }

    private void UpdateHide()
    {
        if (CanSeePlayer())
        {
            EnterChase();
            return;
        }

        Vector3 toHide = _hidePoint.position - transform.position;
        if (toHide.magnitude <= arriveDistance)
        {
            _state = BomberState.Scan;
            return;
        }

        Vector3 direction = toHide.normalized;
        transform.position += direction * chaseSpeed * Time.deltaTime;
        transform.forward = direction;
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 directionToPlayer = _player.position - eyePosition;

        if (directionToPlayer.magnitude > detectionRange) return false;
        if (Vector3.Angle(transform.forward, directionToPlayer) > fieldOfView / 2f) return false;

        return Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, detectionRange, ~0, QueryTriggerInteraction.Ignore)
            && hit.transform == _player;
    }

    //To visualize the detection range and field of view in the editor
    private void OnDrawGizmosSelected()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // Draw field of view
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eyePosition, Quaternion.Euler(0f, -fieldOfView / 2f, 0f) * transform.forward * detectionRange);
        Gizmos.DrawRay(eyePosition, Quaternion.Euler(0f, fieldOfView / 2f, 0f) * transform.forward * detectionRange);
        // Draw obstacle detection ray
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleDistance);
        // Draw explosion range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
    }
}