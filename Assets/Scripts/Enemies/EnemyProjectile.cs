using UnityEngine;

public class EnemyProjectile : Parryable
{
    [Tooltip("Speed of the bullet.")]
    [SerializeField] private float speed;
    [Tooltip("Damage of the bullet.")]
    [SerializeField] private float damage;

    // Time that the bullet has been alive
    private float _timeAlive;

    private void Update()
    {
        if (GameState.IsPaused) return;

        _timeAlive += Time.deltaTime;
        if (_timeAlive >= 1.5f)
        {
            // Destroy bullet after its lifetime
            Destroy(gameObject);
            return;
        }

        // Calculate next frame position
        float stepDistance = speed * Time.deltaTime;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        transform.position = origin + direction * stepDistance;
    }

    public override void Parry()
    {
        base.Parry();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verify if collided with something that can be damaged and its not fellow enemies
        if (other.GetComponent<IDamageable>() != null && other.GetComponent<EnemyHealth>() == null)
        {
            // Apply the damage
            other.GetComponent<IDamageable>().Damage(damage);
        }
        // Destroy the bullet
        Destroy(gameObject);
    }
}
