using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : PausableMonoBehaviour
{
    private enum EnemyState
    {
        Patrol,
        Engaged
    }

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyShooter shooter;
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float minPreferredDistance = 5f;
    [SerializeField] private float maxPreferredDistance = 9f;

    [SerializeField] private float turnSpeed = 8f;
    [SerializeField] private float eyeHeight = 0.5f;
    [SerializeField] private float patrolRadius = 6f;
    [SerializeField] private float patrolWaitTime = 2f;

    [SerializeField] private float dodgeDuration = 0.4f;
    [SerializeField] private float dodgeDistance = 1.5f;


    private EnemyState _state = EnemyState.Patrol;
    private Transform _player;
    private Vector3 _personalSpaceDirection;
    private Vector3 _sideDirection;
    private Vector3 _startPosition;
    private float _patrolWaitTimer;
    private float _preferredDistance;
    private float _repositionTimer;
    private bool _isDodging;

    public bool IsDodging => _isDodging;

    private void Start()
    {
        _player = FindFirstObjectByType<FirstPersonMovement>().transform;
        _startPosition = transform.position;

        enemyHealth.Damaged += _ => EnterEngaged();
        enemyHealth.Dodged += StartDodge;
    }

    private void Update()
    {
        if (GameState.IsPaused || _isDodging) return;

        switch (_state)
        {
            case EnemyState.Patrol:
                UpdatePatrol();
                break;
            case EnemyState.Engaged:
                UpdateEngaged();
                break;
        }
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            EnterEngaged();
            return;
        }

        Patrol();
    }

    private void UpdateEngaged()
    {
        _repositionTimer -= Time.deltaTime;
        if (_repositionTimer <= 0f) PickPersonalSpace();

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);
        MaintainDistance(distanceToPlayer);
        LookAtPlayer();

        if (distanceToPlayer <= attackRange)
        {
            shooter.SetTargetStatus(true);
            shooter.TryShoot(_player);
        }
        else
        {
            shooter.SetTargetStatus(false);
        }
    }

    private void EnterEngaged()
    {
        if (_state == EnemyState.Engaged) return;

        _state = EnemyState.Engaged;
        agent.updateRotation = false;
        _preferredDistance = Random.Range(minPreferredDistance, maxPreferredDistance);

        Vector3 directionFromPlayer = transform.position - _player.position;
        directionFromPlayer.y = 0f;
        _sideDirection = directionFromPlayer.normalized;

        PickPersonalSpace();
    }

    private void PickPersonalSpace()
    {
        float spread = Random.Range(-45f, 45f);
        _personalSpaceDirection = Quaternion.Euler(0f, spread, 0f) * _sideDirection;
        _repositionTimer = Random.Range(3f, 5f);
    }

    private void MaintainDistance(float distanceToPlayer)
    {
        const float tolerance = 1f;

        if (distanceToPlayer > _preferredDistance + tolerance)
        {
            agent.SetDestination(_player.position);
        }
        else if (distanceToPlayer < _preferredDistance - tolerance)
        {
            Vector3 away = transform.position - _player.position;
            away.y = 0f;
            Vector3 target = _player.position + away.normalized * _preferredDistance;

            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }
        else
        {
            agent.ResetPath();
        }
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

        if (directionToPlayer.magnitude > detectionRange) return false;
        if (Vector3.Angle(transform.forward, directionToPlayer) > fieldOfView / 2f) return false;

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

    private void StartDodge()
    {
        StartCoroutine(DodgeRoutine());
    }

    private IEnumerator DodgeRoutine()
    {
        _isDodging = true;
        AgentStopper.SetAgentState(agent, true);
        // agent.isStopped = true;

        Vector3 startPosition = transform.position;
        float side = Random.value < 0.5f ? -1f : 1f;
        Vector3 targetPosition = startPosition + transform.right * side * dodgeDistance;

        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / dodgeDuration);
            yield return null;
        }

        AgentStopper.SetAgentState(agent, false);
        // agent.isStopped = false;
        _isDodging = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);

        //eyesight
        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(eyePosition, Quaternion.Euler(0f, -fieldOfView / 2f, 0f) * transform.forward * detectionRange);
        Gizmos.DrawRay(eyePosition, Quaternion.Euler(0f, fieldOfView / 2f, 0f) * transform.forward * detectionRange);
    }
}