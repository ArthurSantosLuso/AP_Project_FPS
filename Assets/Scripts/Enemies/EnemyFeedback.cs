using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private AudioClip hurtAudio;
    [SerializeField] private AudioClip deathAudio;
    [SerializeField] private GameObject deathEffect;

    private void Start()
    {
        enemyHealth.Damaged += OnDamaged;
        enemyHealth.OnDeath += OnDied;
    }

    private void OnDestroy()
    {
        enemyHealth.Damaged -= OnDamaged;
        enemyHealth.OnDeath -= OnDied;
    }

    private void OnDamaged(float damage)
    {
        AudioManager.Instance.PlaySound(hurtAudio, Random.Range(0.9f, 1.1f));
    }

    private void OnDied(EnemyHealth health)
    {
        AudioManager.Instance.PlaySound(deathAudio, Random.Range(0.9f, 1.1f));

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}