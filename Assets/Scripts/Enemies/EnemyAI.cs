using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : PausableMonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyShooter shooter;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float turnSpeed = 8f;

    private Transform _player;

    private void Start()
    {
        _player = FindFirstObjectByType<FirstPersonMovement>().transform;
        agent.updateRotation = false;
    }

    private void Update()
    {
        agent.SetDestination(_player.position);
        LookAtPlayer();

        if (Vector3.Distance(transform.position, _player.position) <= attackRange)
            shooter.TryShoot(_player);
    }

    private void LookAtPlayer()
    {
        Vector3 direction = _player.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
}