using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyShooter shooter;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float preferredDistance = 7f;
    [SerializeField] private float turnSpeed = 8f;

    private Transform _player;
    private Vector3 _personalSpaceDirection;
    private bool _hasDetectedPlayer;

    private void Start()
    {
        _player = FindFirstObjectByType<FirstPersonMovement>().transform;
        agent.updateRotation = false;
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (!_hasDetectedPlayer)
        {
            if (distanceToPlayer <= detectionRange) DetectPlayer();
            return;
        }

        agent.SetDestination(_player.position);
        LookAtPlayer();

        if (Vector3.Distance(transform.position, _player.position) <= attackRange)
            shooter.TryShoot(_player);
    }

    private void DetectPlayer()
    {
        _hasDetectedPlayer = true;

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

    private void OnDrawGizmosSelected()
    {
        // Draw detection range 
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}