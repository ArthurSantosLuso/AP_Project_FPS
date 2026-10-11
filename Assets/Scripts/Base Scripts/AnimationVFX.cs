using UnityEngine;

/// <summary>
/// Generic VFX: any combination of an Animator trigger, particle systems and a sound.
/// </summary>
public class AnimationVFX : VFX
{
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string triggerName;

    [Header("Particles (optional)")]
    [SerializeField] private ParticleSystem[] particles;

    [Header("Audio (optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Vector2 pitchRange = new Vector2(1f, 1f);

    private int _triggerHash;

    private void Awake()
    {
        _triggerHash = Animator.StringToHash(triggerName);
    }

    public override void Play()
    {
        if (animator != null && !string.IsNullOrEmpty(triggerName))
            animator.SetTrigger(_triggerHash);

        if (particles != null)
            foreach (var p in particles)
                if (p != null) p.Play();

        if (audioSource != null && clips != null && clips.Length > 0)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    public override void Stop()
    {
        if (particles != null)
            foreach (var p in particles)
                if (p != null) p.Stop();
    }
}
