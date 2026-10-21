using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : PausableMonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyShooter shooter;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float preferredDistance = 7f;
    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float eyeHeight = 0.5f;
    [SerializeField] private float patrolRadius = 6f;
    [SerializeField] private float patrolWaitTime = 2f;

    private Transform _player;
    private Vector3 _personalSpaceDirection;
    private Vector3 _startPosition;
    private bool _hasDetectedPlayer;
    private float _patrolWaitTimer;

    private void Start()
    {
        _player = FindFirstObjectByType<FirstPersonMovement>().transform;
        _startPosition = transform.position;

        enemyHealth.Damaged += _ => DetectPlayer();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (!_hasDetectedPlayer)
        {
            if (distanceToPlayer <= detectionRange && CanSeePlayer()) DetectPlayer();
            else Patrol();
            return;
        }

        agent.SetDestination(_player.position);
        LookAtPlayer();

        if (Vector3.Distance(transform.position, _player.position) <= attackRange)
            shooter.TryShoot(_player);
    }

    private void DetectPlayer()
    {
        if (_hasDetectedPlayer) return;

        _hasDetectedPlayer = true;
        agent.updateRotation = false;

        Vector3 directionFromPlayer = transform.position - _player.position;
        directionFromPlayer.y = 0f;

        float spreadAngle = Random.Range(-45f, 45f);
        _personalSpaceDirection = Quaternion.Euler(0f, spreadAngle, 0f) * directionFromPlayer.normalized;
    }

    private void LookAtPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private bool CanSeePlayer()
    {
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 directionToPlayer = _player.position - eyePosition;

        return Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, detectionRange, ~0, QueryTriggerInteraction.Ignore)
            && hit.transform == _player;
    }

    private void Patrol()
    {
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) return;

        _patrolWaitTimer -= Time.deltaTime;
        if (_patrolWaitTimer > 0f) return;

        Vector3 randomPoint = _startPosition + Random.insideUnitSphere * patrolRadius;
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        _patrolWaitTimer = patrolWaitTime;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // Draw patrol range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}