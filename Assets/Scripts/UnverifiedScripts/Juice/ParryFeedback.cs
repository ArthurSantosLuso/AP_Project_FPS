using UnityEngine;

// Sits next to ParryAbility and reacts to the same successful parry trigger.
public class ParryFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem _impactFramePrefab;
    [SerializeField] private GameObject _impactSparkPrefab;
    [SerializeField] private ParryPowerTransfer _powerTransfer;
    [Space]
    [Header("Feel")]
    [SerializeField] private float _effectLifetime = 1.5f;
    [SerializeField] private float _sparkScale = 0.4f;
    [SerializeField] private float _hitStopDuration = 0.16f;
    [SerializeField] private float _shakeTrauma = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Parryable>() == null) return;

        Vector3 impactPoint = other.transform.position;
        SpawnUnscaled(_impactFramePrefab.gameObject, impactPoint, 1f);
        SpawnUnscaled(_impactSparkPrefab, impactPoint, _sparkScale);

        ImpactFrameSequence.Instance?.Play();
        HitStop.Instance?.Freeze(_hitStopDuration);
        CameraShake.Instance?.Shake(_shakeTrauma);
        _powerTransfer?.Charge();
    }

    // Plays in real time, so the effect runs during the hit stop.
    private void SpawnUnscaled(GameObject prefab, Vector3 position, float scale)
    {
        if (prefab == null) return;

        GameObject effect = Instantiate(prefab, position, Quaternion.identity);
        effect.transform.localScale = Vector3.one * scale;
        foreach (ParticleSystem particles in effect.GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule main = particles.main;
            main.useUnscaledTime = true;
        }

        Destroy(effect, _effectLifetime);
    }
}
